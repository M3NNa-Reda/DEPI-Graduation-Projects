using EasyShop.Application.DTOs.Order;
using EasyShop.Application.DTOs.Shipment;
using EasyShop.Application.Interfaces.Repositories;
using EasyShop.Application.Interfaces.Services;
using EasyShop.Application.Services;
using EasyShop.Domain.Entities;
using EasyShop.Domain.Enums;

namespace EasyShop.Tests;

public class OrderServiceTests
{
    private const int UserId = 7;
    private const int AddressId = 3;

    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<ICouponService> _coupons = new();
    private readonly Mock<IPaymentService> _payments = new();
    private readonly Mock<IShipmentService> _shipments = new();

    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IGenericRepository<UserAddress>> _addresses = new();
    private readonly Mock<ICartRepository> _carts = new();
    private readonly Mock<IProductVariantRepository> _variants = new();
    private readonly Mock<IOrderRepository> _orders = new();
    private readonly Mock<IGenericRepository<InventoryTransaction>> _inventory = new();
    private readonly Mock<IGenericRepository<OrderItem>> _orderItems = new();
    private readonly Mock<IGenericRepository<OrderStatusHistory>> _history = new();

    private readonly ProductVariant _variant;
    private readonly Cart _cart;
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        _uow.SetupGet(u => u.Users).Returns(_users.Object);
        _uow.SetupGet(u => u.UserAddresses).Returns(_addresses.Object);
        _uow.SetupGet(u => u.Carts).Returns(_carts.Object);
        _uow.SetupGet(u => u.ProductVariants).Returns(_variants.Object);
        _uow.SetupGet(u => u.Orders).Returns(_orders.Object);
        _uow.SetupGet(u => u.InventoryTransactions).Returns(_inventory.Object);
        _uow.SetupGet(u => u.OrderItems).Returns(_orderItems.Object);
        _uow.SetupGet(u => u.OrderStatusHistories).Returns(_history.Object);

        _variant = new ProductVariant
        {
            Id = 1,
            SKU = "PHONE-BLK",
            Price = 100m,
            StockQuantity = 5,
            IsActive = true,
            Product = new Product { Id = 10, Name = "Phone", IsActive = true }
        };
        _cart = new Cart
        {
            Id = 1,
            UserId = UserId,
            CartItems = new List<CartItem> { new() { ProductVariantId = 1, Quantity = 2 } }
        };

        _users.Setup(r => r.GetByIdAsync(UserId)).ReturnsAsync(new ApplicationUser { Id = UserId, FirstName = "Ali", LastName = "Hassan", PhoneNumber = "01000000000" });
        _addresses.Setup(r => r.GetByIdAsync(AddressId)).ReturnsAsync(new UserAddress { Id = AddressId, UserId = UserId, Country = "Egypt", City = "Zagazig", Area = "Downtown", Street = "El Gomhoria" });
        _carts.Setup(r => r.GetCartWithItemsByUserIdAsync(UserId)).ReturnsAsync(_cart);
        _variants.Setup(r => r.GetVariantsWithProductByIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync(new[] { _variant });
        _orders.Setup(r => r.AddAsync(It.IsAny<Order>())).Callback<Order>(o => o.Id = 55).Returns(Task.CompletedTask);

        _service = new OrderService(_uow.Object, _coupons.Object, _payments.Object, _shipments.Object);
    }

    private static CreateOrderRequestDto Request(string? coupon = null) =>
        new() { AddressId = AddressId, PaymentMethod = PaymentMethod.CashOnDelivery, CouponCode = coupon };

    [Fact]
    public async Task CreateOrder_adds_shipping_to_the_subtotal()
    {
        var order = await _service.CreateOrderAsync(UserId, Request());

        Assert.Equal(200m, order.Subtotal);
        Assert.Equal(OrderService.FlatShippingFee, order.ShippingFee);
        Assert.Equal(270m, order.TotalAmount);
        Assert.Equal(55, order.Id);
    }

    [Fact]
    public async Task CreateOrder_takes_the_coupon_discount_off_the_total()
    {
        var coupon = new Coupon { Id = 9, Code = "SAVE30" };
        _coupons.Setup(c => c.ValidateCouponAsync("SAVE30", UserId, 200m)).ReturnsAsync(coupon);
        _coupons.Setup(c => c.CalculateDiscount(coupon, 200m)).Returns(30m);

        var order = await _service.CreateOrderAsync(UserId, Request("SAVE30"));

        Assert.Equal(30m, order.DiscountAmount);
        Assert.Equal(240m, order.TotalAmount);
        Assert.Equal("SAVE30", order.CouponCode);
        _coupons.Verify(c => c.ApplyCouponAsync(coupon, UserId, 55), Times.Once);
    }

    [Fact]
    public async Task CreateOrder_deducts_stock_and_records_the_sale()
    {
        await _service.CreateOrderAsync(UserId, Request());

        Assert.Equal(3, _variant.StockQuantity);
        _inventory.Verify(r => r.AddAsync(It.Is<InventoryTransaction>(t =>
            t.ProductVariantId == 1 && t.Quantity == 2 && t.TransactionType == TransactionType.Sale && t.ReferenceId == 55)), Times.Once);
    }

    [Fact]
    public async Task CreateOrder_creates_the_payment_and_the_shipment_and_empties_the_cart()
    {
        await _service.CreateOrderAsync(UserId, Request());

        _payments.Verify(p => p.CreatePaymentAsync(55, PaymentMethod.CashOnDelivery, 270m), Times.Once);
        _shipments.Verify(s => s.CreateShipmentAsync(It.Is<CreateShipmentDto>(d => d.OrderId == 55 && d.ShippingCost == OrderService.FlatShippingFee)), Times.Once);
        _carts.Verify(r => r.RemoveCartItems(_cart.CartItems), Times.Once);
    }

    [Fact]
    public async Task CreateOrder_commits_the_transaction_when_everything_succeeds()
    {
        await _service.CreateOrderAsync(UserId, Request());

        _uow.Verify(u => u.BeginTransactionAsync(), Times.Once);
        _uow.Verify(u => u.CommitTransactionAsync(), Times.Once);
        _uow.Verify(u => u.RollbackTransactionAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateOrder_rolls_everything_back_when_a_step_fails()
    {
        _payments.Setup(p => p.CreatePaymentAsync(It.IsAny<int>(), It.IsAny<PaymentMethod>(), It.IsAny<decimal>()))
            .ThrowsAsync(new InvalidOperationException("Payment failed."));

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateOrderAsync(UserId, Request()));

        _uow.Verify(u => u.RollbackTransactionAsync(), Times.Once);
        _uow.Verify(u => u.CommitTransactionAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateOrder_rejects_a_cart_that_asks_for_more_than_the_stock()
    {
        _variant.StockQuantity = 1;

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateOrderAsync(UserId, Request()));

        _uow.Verify(u => u.BeginTransactionAsync(), Times.Never);
        Assert.Equal(1, _variant.StockQuantity);
    }

    [Fact]
    public async Task CreateOrder_rejects_an_inactive_product()
    {
        _variant.Product.IsActive = false;

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateOrderAsync(UserId, Request()));
    }

    [Fact]
    public async Task CreateOrder_rejects_an_address_that_belongs_to_someone_else()
    {
        _addresses.Setup(r => r.GetByIdAsync(AddressId)).ReturnsAsync(new UserAddress { Id = AddressId, UserId = 999 });

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateOrderAsync(UserId, Request()));
    }

    [Fact]
    public async Task CreateOrder_rejects_an_empty_cart()
    {
        _cart.CartItems.Clear();

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateOrderAsync(UserId, Request()));
    }

    [Fact]
    public async Task CancelOrder_returns_the_stock_and_writes_a_history_entry()
    {
        var order = new Order { Id = 55, UserId = UserId, Status = OrderStatus.Pending };
        _orders.Setup(r => r.GetByIdAsync(55)).ReturnsAsync(order);
        _orderItems.Setup(r => r.FindByConditionAsync(It.IsAny<System.Linq.Expressions.Expression<Func<OrderItem, bool>>>()))
            .ReturnsAsync(new[] { new OrderItem { OrderId = 55, ProductVariantId = 1, Quantity = 2 } });
        _variants.Setup(r => r.GetVariantsByIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync(new[] { _variant });

        await _service.CancelOrderAsync(UserId, 55);

        Assert.Equal(OrderStatus.Cancelled, order.Status);
        Assert.Equal(7, _variant.StockQuantity);
        _history.Verify(r => r.AddAsync(It.Is<OrderStatusHistory>(h =>
            h.OldStatus == OrderStatus.Pending && h.NewStatus == OrderStatus.Cancelled && h.ChangedByUserId == UserId)), Times.Once);
    }

    [Theory]
    [InlineData(OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Shipped)]
    [InlineData(OrderStatus.Delivered)]
    public async Task CancelOrder_is_only_possible_while_the_order_is_pending(OrderStatus status)
    {
        _orders.Setup(r => r.GetByIdAsync(55)).ReturnsAsync(new Order { Id = 55, UserId = UserId, Status = status });

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CancelOrderAsync(UserId, 55));
    }

    [Fact]
    public async Task CancelOrder_refuses_an_order_that_belongs_to_another_customer()
    {
        _orders.Setup(r => r.GetByIdAsync(55)).ReturnsAsync(new Order { Id = 55, UserId = 999, Status = OrderStatus.Pending });

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.CancelOrderAsync(UserId, 55));
    }
}
