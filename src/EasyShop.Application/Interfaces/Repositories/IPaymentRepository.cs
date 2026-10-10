using EasyShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Application.Interfaces.Repositories
{
    public interface IPaymentRepository:IGenericRepository<Payment>
    {
        Task<Payment?> GetByOrderIdAsync(int orderId);
    }
}
