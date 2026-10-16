namespace EasyShop.Application.Interfaces.Repositories
{
    public interface IReviewRepository:IGenericRepository<Review>
    {
        Task<IEnumerable<Review>> GetReviewsByOrderItemIdAsync(int orderItemId);
    }
}
