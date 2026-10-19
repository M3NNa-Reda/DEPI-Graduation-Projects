namespace EasyShop.Infrastructure.Repositories
{
    public class PaymentRepository:GenericRepository<Payment>,IPaymentRepository
    {
        public PaymentRepository(AppDbContext context) : base(context) { }

        public async Task<Payment?> GetByOrderIdAsync(int orderId)
        {
            return await _context.Payments.AsNoTracking()
                .FirstOrDefaultAsync(x => x.OrderId == orderId);
        }
    }
}
