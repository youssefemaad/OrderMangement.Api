using Microsoft.EntityFrameworkCore;
using OrderManagement.Core.DomainLayer.Contracts;
using Persistence.Data;
using System.Linq.Expressions;

namespace Persistence.Repository
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> where TEntity : class
    {
        protected readonly OrderManagementDbContext _context;

        public GenericRepository(OrderManagementDbContext context)
        {
            _context = context;
        }

        public virtual async Task<TEntity?> GetByIdAsync(TKey id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _context.Set<TEntity>().Where(predicate).ToListAsync();
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            return entity;
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
            await Task.CompletedTask;
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            await Task.CompletedTask;
        }

        public virtual async Task<bool> ExistsAsync(TKey id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            return entity != null;
        }

        public virtual async Task<int> CountAsync()
        {
            return await _context.Set<TEntity>().CountAsync();
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _context.Set<TEntity>().CountAsync(predicate);
        }
    }
}