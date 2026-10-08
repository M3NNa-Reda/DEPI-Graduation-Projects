using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Application.Interfaces.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> GetById(int id);
        Task<IEnumerable<TEntity>> GetAll();

        Task Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);

    }
}
