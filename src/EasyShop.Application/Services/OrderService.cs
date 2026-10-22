using EasyShop.Application.DTOs.Shipment;

namespace EasyShop.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICouponService _couponService;
        private readonly IPaymentService _paymentService ;
        private readonly IShipmentService _shipmentService ;

        public OrderService(IUnitOfWork unitOfWork,
            ICouponService couponService,
            IPaymentService paymentService,
            IShipmentService shipmentService)
        {
            _unitOfWork = unitOfWork;
            _couponService = couponService;
            _paymentService = paymentService;
            _shipmentService = shipmentService;
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
            var productVariantIds = cart.CartItems.Select(x => x.ProductVariantId).Distinct().ToList();
            var productVariants = await _unitOfWork.ProductVariants.GetVariantsWithProductByIdsAsync(productVariantIds);
            var variantsById = productVariants.ToDictionary(x => x.Id);
            var orderItems = new List<OrderItem>();
            decimal subtotal = 0;
            foreach (var item in cart.CartItems)
            {
                if (item.Quantity <= 0)
                    throw new InvalidOperationException("Quantity must be greater than zero.");

                if (!variantsById.TryGetValue( item.ProductVariantId, out var variant)
                    || !variant.IsActive)
                    throw new InvalidOperationException("Product variant is unavailable.");
                if (variant.StockQuantity < item.Quantity)
                    throw new InvalidOperationException($"Insufficient stock for variant ID: {variant.Id}.");

                var itemTotalPrice = variant.Price * item.Quantity;

                subtotal += itemTotalPrice;

                orderItems.Add(new OrderItem
                {
                    ProductVariantId = variant.Id,
                    ProductName = variant.Product.Name,
                    SKU = variant.SKU,
                    UnitPrice = variant.Price,
                    Quantity = item.Quantity,
                    TotalPrice = itemTotalPrice
                });
            }

        
            Coupon coupon = null;
            decimal discount = 0;
            if (!string.IsNullOrEmpty(dto.CouponCode))
            {
                coupon = await _couponService.ValidateCouponAsync(dto.CouponCode, userId, subtotal); // يرجع كوبون صالح
                discount = _couponService.CalculateDiscount(coupon, subtotal);
                if (discount < 0 || discount > subtotal)
                    throw new InvalidOperationException("Invalid coupon discount.");
            }

            decimal shipping = 70m;
            decimal totalAmount = (subtotal + shipping) - discount;
            var order = new Order
            {
                UserId = userId,
                Status = OrderStatus.Confirmed,
                Subtotal = subtotal,
                DiscountAmount = discount,
                ShippingFee = shipping,
                TotalAmount = totalAmount,
                CouponId = coupon?.Id,
                OrderItems = orderItems,
                CreatedAt = DateTime.UtcNow,
                OrderShippingAddress = new OrderShippingAddress
                {
                    FullName = $"{user.FirstName} {user.LastName}".Trim(),
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    Country = address.Country,
                    City = address.City,
                    Area = address.Area,
                    Street = address.Street,
                    BuildingNumber = address.BuildingNumber,
                    ApartmentNumber = address.ApartmentNumber,
                    PostalCode = address.PostalCode
                }

            };
             await _unitOfWork.BeginTransactionAsync();

            try
            {
                foreach (var cartItem in cart.CartItems)
                {
                    var variant = variantsById[cartItem.ProductVariantId];

                    variant.StockQuantity -= cartItem.Quantity;
                }

                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.CompleteAsync();


                foreach (var cartItem in cart.CartItems)
                {
                    await _unitOfWork.InventoryTransactions.AddAsync(
                        new InventoryTransaction
                        {
                            ProductVariantId = cartItem.ProductVariantId,
                            Quantity = cartItem.Quantity,
                            TransactionType = TransactionType.Sale,
                            ReferenceType = "Order",
                            ReferenceId = order.Id,
                            CreatedAt = DateTime.UtcNow
                        });
                }

                if (coupon != null)
                {
                    await _couponService.ApplyCouponAsync(
                        coupon,
                        userId,
                        order.Id);
                }

                await _paymentService.CreatePaymentAsync(
                    order.Id,
                    dto.PaymentMethod,
                    totalAmount);

                await _shipmentService.CreateShipmentAsync(
                    new CreateShipmentDto
                    {
                        OrderId = order.Id,
                        Carrier = "Default",
                        TrackingNumber = null,
                        ShippingCost = shipping
                    });

                await _unitOfWork.Carts.RemoveCartItems(cart.CartItems);

                await _unitOfWork.CompleteAsync();

                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }

            return new OrderDetailsDto
            {
                Id = order.Id,
                Status = order.Status,
                Subtotal = order.Subtotal,
                DiscountAmount = order.DiscountAmount,
                ShippingFee = order.ShippingFee,
                TotalAmount = order.TotalAmount,
                CouponCode = coupon?.Code,
                ShippingAddress =
                    $"{address.Street}, {address.Area}, {address.City}, {address.Country}",
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,

                Items = order.OrderItems.Select(item => new OrderItemDto
                {
                    ProductVariantId = item.ProductVariantId,
                    ProductName = item.ProductName,
                    SKU = item.SKU,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice
                }).ToList()
            };
           
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
            if (order.Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("Order is already cancelled.");
            if (order.Status!=OrderStatus.Pending)
                throw new InvalidOperationException("Cannot cancel an order that is currently being processed.");
            
            var items = await _unitOfWork.OrderItems.FindByConditionAsync(o => o.OrderId == orderId);
            var productVariantIds = items.Select(x => x.ProductVariantId).ToList();
            var productVariants = await _unitOfWork.ProductVariants.GetVariantsByIdsAsync(productVariantIds);
            foreach (var item in items)
            {
                var productVariant = productVariants.FirstOrDefault(p => p.Id == item.ProductVariantId);
                if(productVariant != null)
                {
                    //if (!productVariant.IsActive && productVariant.StockQuantity == 0) productVariant.IsActive = true; مش دايما صح
                    productVariant.StockQuantity += item.Quantity;
                    _unitOfWork.ProductVariants.Update(productVariant);
                }
                
            }
            var oldStatus = order.Status;
            order.Status=OrderStatus.Cancelled;
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.OrderStatusHistories.AddAsync(
                new OrderStatusHistory
                {
                    OrderId = order.Id,
                    OldStatus = oldStatus,
                    NewStatus = OrderStatus.Cancelled,
                    ChangedAt = DateTime.UtcNow,
                    ChangedByUserId = userId,
                    Notes = $"Order cancelled by customer Id: {userId}."
                });
            await _unitOfWork.CompleteAsync();
        }

       
        public async Task<IEnumerable<OrderSummaryDto>> GetRecentOrdersAsync(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if(pageSize < 1) pageSize = 10;
            var orders = await _unitOfWork.Orders.GetRecentOrdersAsync(page, pageSize);
            return orders.Select(order => new OrderSummaryDto
            {
                Id = order.Id,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                TotalItems = order.OrderItems?.Count ?? 0,
            });

        }

        public async Task<OrderDetailsDto?> GetMyOrderByIdAsync(int userId, int orderId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            var order = await _unitOfWork.Orders.GetOrderWithItemsByUserIdAsync(userId, orderId);
            if (order == null)
                throw new KeyNotFoundException("Order not found.");
            if (order.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to see this order.");

            var couponId=order.CouponId;
            Coupon? coupon = null;
            if (couponId != null)
                coupon = await _unitOfWork.Coupons.GetByIdAsync((int)couponId);
            var shippingAddress = await _unitOfWork.Orders.GetShippingAddressByOrderIdAsync(orderId);

            return new OrderDetailsDto
            {
                Id = order.Id,
                Status = order.Status,
                Subtotal = order.Subtotal,
                DiscountAmount = order.DiscountAmount,
                ShippingFee=order.ShippingFee,
                TotalAmount = order.TotalAmount,
                CouponCode = coupon?.Code,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                ShippingAddress = shippingAddress,
                Items = order.OrderItems.Select(i => new OrderItemDto
                {
                    ProductVariantId = i.ProductVariantId,   // نعمل لينك يرجعه لل product
                    ProductName = i.ProductName,
                    SKU = i.SKU,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    TotalPrice = i.TotalPrice
                }).ToList()
            };

        }

        public async Task<IEnumerable<OrderSummaryDto>> GetMyOrdersAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");
            var orders= await _unitOfWork.Orders.GetOrdersByUserIdAsync(userId);
            return orders.Select(order => new OrderSummaryDto
            {
                Id = order.Id,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                TotalItems = order.OrderItems?.Count ?? 0,
            });

        }
        // للادمن بس
        public async Task<OrderDetailsDto?> GetOrderByIdForAdminAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException("Order not found.");
            var couponId = order.CouponId;
            Coupon? coupon = null;
            if (couponId != null)
                coupon = await _unitOfWork.Coupons.GetByIdAsync((int)couponId);
            var shippingAddress = await _unitOfWork.Orders.GetShippingAddressByOrderIdAsync(orderId);

            return new OrderDetailsDto
            {
                Id = order.Id,
                Status = order.Status,
                Subtotal = order.Subtotal,
                DiscountAmount = order.DiscountAmount,
                ShippingFee = order.ShippingFee,
                TotalAmount = order.TotalAmount,
                CouponCode = coupon?.Code,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                ShippingAddress = shippingAddress,
                Items = order.OrderItems.Select(i => new OrderItemDto
                {
                    ProductVariantId = i.ProductVariantId,  
                    ProductName = i.ProductName,
                    SKU = i.SKU,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    TotalPrice = i.TotalPrice
                }).ToList()
            };

        }

        // للادمن بس
        public async Task UpdateOrderStatusAsync(int adminId, int orderId, UpdateOrderStatusDto dto)
        {
            var admin = await _unitOfWork.Users.GetByIdAsync(adminId);
            if (admin == null)
                throw new KeyNotFoundException("Admin not found.");

            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException("Order not found.");
            if (order.Status == dto.Status)
                throw new InvalidOperationException($"Order is already {dto.Status}.");

            var orderStatusHistory = new OrderStatusHistory
            {
                OldStatus = order.Status,
                NewStatus = dto.Status,
                Notes = dto.Notes,
                ChangedAt = DateTime.UtcNow,
                OrderId = orderId,
                ChangedByUserId = adminId
            };
            order.Status=dto.Status;
            order.UpdatedAt= DateTime.UtcNow;
             _unitOfWork.Orders.Update(order); 
            await _unitOfWork.OrderStatusHistories.AddAsync(orderStatusHistory);
            await _unitOfWork.CompleteAsync();
        }
    }
}
