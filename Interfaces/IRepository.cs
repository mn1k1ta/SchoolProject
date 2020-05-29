using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SchoolProject.Interfaces
{
    public interface IRepository<TEntity>
        where TEntity : class
    {
        void Create(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task<ICollection<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(int id);
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> GetWhere(Expression<Func<TEntity, bool>> expression);     
        Task<ICollection<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> expression);
        
    }
}
