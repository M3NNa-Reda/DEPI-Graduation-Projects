using EasyShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Application.Interfaces.Repositories
{
    public interface IProductVariantRepository:IGenericRepository<ProductVariant>
    {
        Task<IEnumerable<ProductVariant>> GetVariantsWithProductByIdsAsync(IEnumerable<int> variantIds);
        Task<IEnumerable<ProductVariant>> GetVariantsByIdsAsync(IEnumerable<int> variantIds);
    }
}
