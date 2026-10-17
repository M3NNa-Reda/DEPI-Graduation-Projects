namespace EasyShop.Application.Interfaces.Repositories
{
    public interface IPaymentTransactionRepository : IGenericRepository<PaymentTransaction>
    {
        Task<IEnumerable<PaymentTransaction>> GetByPaymentIdAsync(int paymentId);
    }
}
