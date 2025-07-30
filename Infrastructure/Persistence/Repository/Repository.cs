using Microsoft.EntityFrameworkCore;
using OrderManagement.Core.DomainLayer.Contracts;
using Persistence.Data;
using System.Linq.Expressions;

namespace Persistence.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly OrderManagementDbContext _context;

        public Repository(OrderManagementDbContext context)
        {
            _context = context;
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await Task.CompletedTask;
        }

        public virtual async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await Task.CompletedTask;
        }
    }
}
