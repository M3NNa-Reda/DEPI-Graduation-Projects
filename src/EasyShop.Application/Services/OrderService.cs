using EasyShop.Application.DTOs.Order;
using EasyShop.Application.Interfaces.Repositories;
using EasyShop.Application.Interfaces.Services;
using EasyShop.Domain.Entities;
using EasyShop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICouponService _couponService;
        private readonly IPaymentService _paymentService ;

        public OrderService(IUnitOfWork unitOfWork,ICouponService couponService, IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _couponService = couponService;
            _paymentService = paymentService;
        }

        public async Task<OrderDetailsDto> CreateOrderAsync(int userId, CreateOrderRequestDto dto)
        {
            // user login and authorized
            // check address
            // cart and items
            // get product
            // product active and we have stock
            // create orderitem
            // dec quantity and calc total price
            // add orderitem
            // check coupon
            // shipping
            // payment
            var user =await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            var address = await _unitOfWork.UserAddresses.GetByIdAsync(dto.AddressId);
            if (address == null || address.UserId != userId)
                throw new KeyNotFoundException("Shipping address not found.");

            var cart= await _unitOfWork.Carts.GetCartWithItemsByUserIdAsync(userId);
            if (cart == null || !cart.CartItems.Any())
            {
                throw new KeyNotFoundException("Cart is empty or not found.");
            }
            var productVariantIds = cart.CartItems.Select(x => x.ProductVariantId).ToList();
            var productVariants = await _unitOfWork.ProductVariants.GetVariantsWithProductByIdsAsync(productVariantIds);
            var orderItems = new List<OrderItem>();
            decimal totalAmountOfItems = 0;
            foreach (var item in cart.CartItems)
            {
                var productVariant = productVariants.FirstOrDefault(p => p.Id == item.ProductVariantId); //one by one
                if(productVariant==null||!productVariant.IsActive)
                    throw new InvalidOperationException($"Product item is unavailable.");
                if(productVariant.StockQuantity<item.Quantity)
                    throw new InvalidOperationException($"Insufficient stock for item ID: {item.ProductVariantId}");

                productVariant.StockQuantity -= item.Quantity;
                var price=productVariant.Price;
                var itemTotalPrice = price * item.Quantity;
                totalAmountOfItems += itemTotalPrice;

                orderItems.Add(new OrderItem
                {
                    ProductName = productVariant.Product.Name,
                    SKU = productVariant.SKU,
                    UnitPrice = productVariant.Price,
                    Quantity = item.Quantity,
                    TotalPrice = itemTotalPrice,
                    ProductVariantId = productVariant.Id

                });

            }
            Coupon coupon = null;
            decimal discount = 0;
            if (!string.IsNullOrEmpty(dto.CouponCode))
            {
                coupon = await _couponService.ValidateCouponAsync(dto.CouponCode, userId, totalAmountOfItems); // يرجع كوبون صالح
                discount = _couponService.CalculateDiscount(coupon, totalAmountOfItems);
                if (discount < 0 || discount > totalAmountOfItems)
                    throw new InvalidOperationException("Invalid coupon discount.");
            }

            // لسه هنحسب الشحن
            decimal shipping = 0;

            decimal totalAmount = (totalAmountOfItems + shipping) - discount;
            var order = new Order
            {
                Status = OrderStatus.Confirmed,
                Subtotal = totalAmountOfItems,
                DiscountAmount = discount,
                ShippingFee = shipping,
                TotalAmount = totalAmount,
                CouponId = coupon?.Id,
                OrderItems = orderItems,
                CreatedAt = DateTime.UtcNow,
                OrderShippingAddress = new OrderShippingAddress
                {
                    FullName = user.FirstName + user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Country = address.Country,
                    City = address.City,
                    Area = address.Area,
                    Street = address.Street,
                    BuildingNumber = address.BuildingNumber,
                    ApartmentNumber = address.ApartmentNumber,
                    PostalCode = address.PostalCode
                }

            };
            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.CompleteAsync();
            if (coupon != null)
            {
                await _couponService.ApplyCouponAsync(coupon, userId, order.Id);
            }

            var payment = await _paymentService.CreatePaymentAsync(order.Id, dto.PaymentMethod, totalAmount);
            await _unitOfWork.Carts.RemoveCartItems(cart.CartItems);

           
            return new OrderDetailsDto();
        }

        public async Task CancelOrderAsync(int userId, int orderId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
               throw new KeyNotFoundException("User not found.");

            var order =await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException("Order not found.");
            if (order.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to cancel this order.");
            if (order.Status==OrderStatus.Processing)
                throw new InvalidOperationException("Cannot cancel an order that is currently being processed.");
            if (order.Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("Order is already cancelled.");
            var items = await _unitOfWork.OrderItems.FindByConditionAsync(o => o.OrderId == orderId);
            var productVariantIds = items.Select(x => x.ProductVariantId).ToList();
            var productVariants = await _unitOfWork.ProductVariants.GetVariantsByIdsAsync(productVariantIds);
            foreach (var item in items)
            {
                var productVariant = productVariants.FirstOrDefault(p => p.Id == item.ProductVariantId);
                if(productVariant != null)
                {
                    if (!productVariant.IsActive && productVariant.StockQuantity == 0) productVariant.IsActive = true;
                    productVariant.StockQuantity += item.Quantity;
                    _unitOfWork.ProductVariants.Update(productVariant);
                }
                
            }
            order.Status=OrderStatus.Cancelled;
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();
        }

       
        // admin
        public async Task<IEnumerable<OrderSummaryDto>> GetRecentOrdersAsync(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if(pageSize < 1) pageSize = 10;
            var orders = await _unitOfWork.Orders.GetRecentOrdersAsync(page, pageSize);
            /*
             public int Id { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalItems { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }*/
            var orderSummaryDtos = new List<OrderSummaryDto>();
            foreach (var order in orders)
            {
                orderSummaryDtos.Add(new OrderSummaryDto
                {
                    Id = order.Id,
                    Status = order.Status,
                    TotalAmount = order.TotalAmount,
                    CreatedAt = order.CreatedAt,
                    TotalItems = order.OrderItems.Count,
                    PaymentStatus = order.Payment.Status
                });
            }

            return orders;
        }

        public Task<OrderDetailsDto?> GetMyOrderByIdAsync(int userId, int orderId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrderSummaryDto>> GetMyOrdersAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<OrderDetailsDto?> GetOrderByIdForAdminAsync(int orderId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateOrderStatusAsync(int adminId, int orderId, UpdateOrderStatusDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
