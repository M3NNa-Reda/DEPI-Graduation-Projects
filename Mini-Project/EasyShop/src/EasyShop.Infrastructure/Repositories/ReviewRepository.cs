namespace EasyShop.Infrastructure.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(AppDbContext context):base(context){}

        public async Task<IEnumerable<Review>> GetReviewsByOrderItemIdAsync(int orderItemId)
        {
            return await _context.Reviews
                .AsNoTracking()
                .Where(r => r.OrderItemId == orderItemId)
                .ToListAsync();
        }
    }
}
