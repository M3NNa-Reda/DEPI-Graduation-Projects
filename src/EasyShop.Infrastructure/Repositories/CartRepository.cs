namespace EasyShop.Infrastructure.Repositories
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        public CartRepository(AppDbContext context) : base(context) { }

        public async Task<Cart?> GetCartWithItemsByUserIdAsync(int userId)
        {
            return await _context.Carts
                 .AsNoTracking()
                 .Include(item => item.CartItems)
                 .FirstOrDefaultAsync(c => c.UserId == userId);
        }



        public async Task RemoveCartItems(IEnumerable<CartItem> cartItems)
        {
              _context.RemoveRange(cartItems);
        }
    }
}
