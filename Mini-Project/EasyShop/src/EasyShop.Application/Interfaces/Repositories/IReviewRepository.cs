using EasyShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Application.Interfaces.Repositories
{
    public interface IReviewRepository:IGenericRepository<Review>
    {
        Task<IEnumerable<Review>> GetReviewsByOrderItemIdAsync(int orderItemId);
    }
}
