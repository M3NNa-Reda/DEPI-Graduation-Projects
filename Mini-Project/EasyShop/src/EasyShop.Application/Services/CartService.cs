using EasyShop.Application.DTOs.Cart;

namespace EasyShop.Application.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CartDto> GetCartAsync(int userId)
        {
            var cart = await _unitOfWork.Carts.GetCartWithItemsByUserIdAsync(userId);

            if (cart == null)
                return new CartDto();

            var cartItems = cart.CartItems
                .OrderBy(item => item.AddedAt)
                .Select(item => new CartItemDto
                {
                    Id = item.Id,
                    ProductVariantId = item.ProductVariantId,
                    ProductName = item.ProductVariant.Product.Name,
                    VariantName = item.ProductVariant.SKU,
                    UnitPrice = item.ProductVariant.Price,
                    Quantity = item.Quantity,
                    TotalPrice = item.ProductVariant.Price * item.Quantity
                })
                .ToList();

            return new CartDto
            {
                Id = cart.Id,
                Items = cartItems,
                Subtotal = cartItems.Sum(i => i.TotalPrice)
            };
        }

        public async Task AddToCartAsync(int userId, AddToCartDto dto)
        {
            if (dto.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            var variant = await GetPurchasableVariantAsync(dto.ProductVariantId);
            var cart = await GetOrCreateCartAsync(userId);

            var existingItem = cart.CartItems
                .FirstOrDefault(i => i.ProductVariantId == dto.ProductVariantId);

            var requestedQuantity = (existingItem?.Quantity ?? 0) + dto.Quantity;
            EnsureStockIsEnough(variant, requestedQuantity);

            if (existingItem != null)
            {
                existingItem.Quantity = requestedQuantity;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductVariantId = dto.ProductVariantId,
                    Quantity = dto.Quantity,
                    AddedAt = DateTime.UtcNow
                });
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> UpdateCartItemAsync(int userId, int productVariantId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            var cart = await GetExistingCartAsync(userId);
            var item = FindItem(cart, productVariantId);

            var variant = await GetPurchasableVariantAsync(productVariantId);
            EnsureStockIsEnough(variant, quantity);

            item.Quantity = quantity;
            cart.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> RemoveFromCartAsync(int userId, int productVariantId)
        {
            var cart = await GetExistingCartAsync(userId);
            var item = FindItem(cart, productVariantId);

            cart.CartItems.Remove(item);
            cart.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Carts.RemoveCartItems(new[] { item });
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task ClearCartAsync(int userId)
        {
            var cart = await _unitOfWork.Carts.GetCartWithItemsByUserIdAsync(userId);
            if (cart == null || !cart.CartItems.Any())
                return;

            var items = cart.CartItems.ToList();
            cart.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Carts.RemoveCartItems(items);
            await _unitOfWork.CompleteAsync();
        }

        private async Task<Cart> GetOrCreateCartAsync(int userId)
        {
            var cart = await _unitOfWork.Carts.GetCartWithItemsByUserIdAsync(userId);
            if (cart != null)
                return cart;

            cart = new Cart
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                CartItems = new List<CartItem>()
            };

            await _unitOfWork.Carts.AddAsync(cart);
            await _unitOfWork.CompleteAsync();
            return cart;
        }

        private async Task<Cart> GetExistingCartAsync(int userId)
        {
            return await _unitOfWork.Carts.GetCartWithItemsByUserIdAsync(userId)
                ?? throw new ArgumentException("Your cart is empty.");
        }

        private static CartItem FindItem(Cart cart, int productVariantId)
        {
            return cart.CartItems.FirstOrDefault(i => i.ProductVariantId == productVariantId)
                ?? throw new ArgumentException("This item is not in your cart.");
        }

        private async Task<ProductVariant> GetPurchasableVariantAsync(int productVariantId)
        {
            var variant = await _unitOfWork.ProductVariants.GetByIdAsync(productVariantId);

            if (variant == null || !variant.IsActive)
                throw new ArgumentException("This product is no longer available.");

            return variant;
        }

        private static void EnsureStockIsEnough(ProductVariant variant, int quantity)
        {
            if (quantity > variant.StockQuantity)
                throw new ArgumentException($"Only {variant.StockQuantity} item(s) left in stock.");
        }
    }
}
