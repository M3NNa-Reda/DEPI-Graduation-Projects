

using EasyShop.Domain.Entities;
using EasyShop.Domain.Enums;

namespace EasyShop.Application.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<Payment> CreatePaymentAsync(int orderId, PaymentMethod method, decimal amount);
        Task<Payment?> GetPaymentByOrderIdAsync(int orderId);
        Task MarkAsPaidAsync(int paymentId);
        Task MarkAsFailedAsync(int paymentId);
    }
}
