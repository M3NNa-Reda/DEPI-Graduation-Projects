namespace EasyShop.Application.Interfaces.Repositories
{
    public interface IPaymentRepository:IGenericRepository<Payment>
    {
        Task<Payment?> GetByOrderIdAsync(int orderId);
    }
}
