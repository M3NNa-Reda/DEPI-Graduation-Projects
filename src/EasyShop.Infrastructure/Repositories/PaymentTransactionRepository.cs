namespace EasyShop.Infrastructure.Repositories
{
    public class PaymentTransactionRepository : GenericRepository<PaymentTransaction>, IPaymentTransactionRepository
    {
        public PaymentTransactionRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<PaymentTransaction>> GetByPaymentIdAsync(int paymentId)
        {
            return await _context.PaymentTransactions
                .AsNoTracking()
                .Where(pt => pt.PaymentId == paymentId)
                .OrderByDescending(pt => pt.CreatedAt)
                .ToListAsync();
        }
    }
}