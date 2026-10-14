using System;
using System.Collections.Generic;
using System.Text;

using EasyShop.Domain.Entities;

namespace EasyShop.Application.Interfaces.Services
{
    public interface ICouponService
    {
        Task<Coupon> ValidateCouponAsync(string code, int userId, decimal orderAmount);
        decimal CalculateDiscount(Coupon coupon, decimal orderAmount);
        bool CheckUsageLimit(Coupon coupon);
        bool CheckExpiration(Coupon coupon);
        Task ApplyCouponAsync(Coupon coupon, int userId, int orderId);
    }
}
