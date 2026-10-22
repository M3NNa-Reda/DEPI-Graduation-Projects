namespace EasyShop.Application.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<Payment> CreatePaymentAsync(int orderId, PaymentMethod method, decimal amount);
        Task<Payment?> GetPaymentByOrderIdAsync(int orderId);
        Task<Payment?> GetPaymentByIdAsync(int paymentId);
        Task<IEnumerable<Payment>> GetAllPaymentsAsync();
        Task<IEnumerable<PaymentTransaction>> GetTransactionsAsync(int paymentId);
        Task MarkAsPaidAsync(int paymentId);
        Task MarkAsFailedAsync(int paymentId);
    }
}
