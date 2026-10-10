using EasyShop.Application.Interfaces.Repositories;
using EasyShop.Domain.Entities;
using EasyShop.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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
