namespace EasyShop.Application.Interfaces.Repositories
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        Task<Cart?> GetCartWithItemsByUserIdAsync(int userId);
        Task RemoveCartItems(IEnumerable<CartItem> cartItems);
    }
}
