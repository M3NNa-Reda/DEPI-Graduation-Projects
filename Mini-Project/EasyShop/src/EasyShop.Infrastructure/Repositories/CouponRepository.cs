using EasyShop.Application.Interfaces.Repositories;
using EasyShop.Domain.Entities;
using EasyShop.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Infrastructure.Repositories
{
    public class CouponRepository : GenericRepository<Coupon>, ICouponRepository
    {
        public CouponRepository(AppDbContext context):base(context) { }
        public async Task<Coupon?> GetByCodeAsync(string code)
        {
            return await _context.Coupons
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == code);
        }
    }
}
