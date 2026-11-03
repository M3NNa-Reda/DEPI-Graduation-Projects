using EasyShop.Application.DTOs.Cart;
using EasyShop.Application.Interfaces.Repositories;
using EasyShop.Application.Services;
using EasyShop.Domain.Entities;

namespace EasyShop.Tests;

public class CartServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<ICartRepository> _carts = new();
    private readonly Mock<IProductVariantRepository> _variants = new();
    private readonly Mock<IProductRepository> _products = new();
    private readonly CartService _service;

    public CartServiceTests()
    {
        _uow.SetupGet(u => u.Carts).Returns(_carts.Object);
        _uow.SetupGet(u => u.ProductVariants).Returns(_variants.Object);
        _uow.SetupGet(u => u.Products).Returns(_products.Object);
        _service = new CartService(_uow.Object);
    }

    private void GivenSellableVariant(int variantId = 1, int stock = 5, bool variantActive = true, bool productActive = true)
    {
        _variants.Setup(r => r.GetByIdAsync(variantId))
            .ReturnsAsync(new ProductVariant { Id = variantId, ProductId = 10, StockQuantity = stock, IsActive = variantActive });
        _products.Setup(r => r.GetByIdAsync(10))
            .ReturnsAsync(new Product { Id = 10, IsActive = productActive });
    }

    private void GivenCart(Cart? cart) =>
        _carts.Setup(r => r.GetCartWithItemsByUserIdAsync(7)).ReturnsAsync(cart);

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public async Task AddToCart_rejects_a_quantity_that_is_not_positive(int quantity)
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AddToCartAsync(7, new AddToCartDto { ProductVariantId = 1, Quantity = quantity }));
    }

    [Fact]
    public async Task AddToCart_rejects_an_inactive_variant()
    {
        GivenSellableVariant(variantActive: false);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AddToCartAsync(7, new AddToCartDto { ProductVariantId = 1, Quantity = 1 }));

        Assert.Equal("This product is no longer available.", ex.Message);
    }

    [Fact]
    public async Task AddToCart_rejects_a_variant_whose_product_is_inactive()
    {
        GivenSellableVariant(productActive: false);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AddToCartAsync(7, new AddToCartDto { ProductVariantId = 1, Quantity = 1 }));
    }

    [Fact]
    public async Task AddToCart_creates_the_cart_on_the_first_add()
    {
        GivenSellableVariant(stock: 5);
        GivenCart(null);
        Cart? created = null;
        _carts.Setup(r => r.AddAsync(It.IsAny<Cart>())).Callback<Cart>(c => created = c).Returns(Task.CompletedTask);

        await _service.AddToCartAsync(7, new AddToCartDto { ProductVariantId = 1, Quantity = 2 });

        Assert.NotNull(created);
        Assert.Equal(7, created!.UserId);
        var item = Assert.Single(created.CartItems);
        Assert.Equal(2, item.Quantity);
        _uow.Verify(u => u.CompleteAsync(), Times.AtLeastOnce);
    }

    [Fact]
    public async Task AddToCart_adds_to_the_quantity_of_an_item_already_in_the_cart()
    {
        GivenSellableVariant(stock: 5);
        var cart = new Cart { Id = 1, UserId = 7, CartItems = new List<CartItem> { new() { ProductVariantId = 1, Quantity = 2 } } };
        GivenCart(cart);

        await _service.AddToCartAsync(7, new AddToCartDto { ProductVariantId = 1, Quantity = 2 });

        var item = Assert.Single(cart.CartItems);
        Assert.Equal(4, item.Quantity);
    }

    [Fact]
    public async Task AddToCart_checks_stock_against_what_is_already_in_the_cart()
    {
        GivenSellableVariant(stock: 3);
        var cart = new Cart { Id = 1, UserId = 7, CartItems = new List<CartItem> { new() { ProductVariantId = 1, Quantity = 2 } } };
        GivenCart(cart);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AddToCartAsync(7, new AddToCartDto { ProductVariantId = 1, Quantity = 2 }));

        Assert.Equal("Only 3 item(s) left in stock.", ex.Message);
        Assert.Equal(2, cart.CartItems.Single().Quantity);
    }

    [Fact]
    public async Task UpdateCartItem_rejects_more_than_the_stock()
    {
        GivenSellableVariant(stock: 4);
        GivenCart(new Cart { Id = 1, UserId = 7, CartItems = new List<CartItem> { new() { ProductVariantId = 1, Quantity = 1 } } });

        await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateCartItemAsync(7, 1, 9));
    }

    [Fact]
    public async Task UpdateCartItem_fails_for_an_item_that_is_not_in_the_cart()
    {
        GivenCart(new Cart { Id = 1, UserId = 7, CartItems = new List<CartItem>() });

        await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateCartItemAsync(7, 1, 2));
    }

    [Fact]
    public async Task GetCart_returns_an_empty_cart_when_the_user_has_none()
    {
        GivenCart(null);

        var dto = await _service.GetCartAsync(7);

        Assert.Empty(dto.Items);
        Assert.Equal(0m, dto.Subtotal);
    }

    [Fact]
    public async Task GetCart_adds_up_the_subtotal()
    {
        var product = new Product { Name = "Phone" };
        var cart = new Cart
        {
            Id = 1,
            UserId = 7,
            CartItems = new List<CartItem>
            {
                new() { ProductVariantId = 1, Quantity = 2, AddedAt = DateTime.UtcNow, ProductVariant = new ProductVariant { SKU = "P-1", Price = 100m, Product = product } },
                new() { ProductVariantId = 2, Quantity = 1, AddedAt = DateTime.UtcNow.AddMinutes(1), ProductVariant = new ProductVariant { SKU = "P-2", Price = 50.5m, Product = product } }
            }
        };
        GivenCart(cart);

        var dto = await _service.GetCartAsync(7);

        Assert.Equal(2, dto.Items.Count);
        Assert.Equal(250.5m, dto.Subtotal);
    }
}
