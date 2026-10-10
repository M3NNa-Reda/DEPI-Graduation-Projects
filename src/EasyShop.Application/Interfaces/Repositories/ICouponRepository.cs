using EasyShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Application.Interfaces.Repositories
{
    public interface ICouponRepository:IGenericRepository<Coupon>
    {
        Task<Coupon?> GetByCodeAsync(string code);
    }
}
