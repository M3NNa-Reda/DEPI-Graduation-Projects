namespace EasyShop.Infrastructure.Repositories
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        public CartRepository(AppDbContext context) : base(context) { }

        public async Task<Cart?> GetCartWithItemsByUserIdAsync(int userId)
        {
            return await _context.Carts
                         .Include(c => c.CartItems)
                         .ThenInclude(ci => ci.ProductVariant)
                         .ThenInclude(pv => pv.Product)
                         .FirstOrDefaultAsync(c => c.UserId == userId);
        }



        public async Task RemoveCartItems(IEnumerable<CartItem> cartItems)
        {
              _context.RemoveRange(cartItems);
        }
    }
}
