using Microsoft.EntityFrameworkCore.Storage;
using OrderManagement.Core.DomainLayer.Contracts;
using Persistence.Data;
using Persistence.Repository;

namespace Persistence.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly OrderManagementDbContext _context;
        private IDbContextTransaction? _transaction;
        private readonly Dictionary<string, object> _repositories = new();

        public UnitOfWork(OrderManagementDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : class
        {
            var entityType = typeof(TEntity).Name;
            var keyType = typeof(TKey).Name;
            var repositoryKey = $"{entityType}_{keyType}";

            if (_repositories.ContainsKey(repositoryKey))
            {
                return (IGenericRepository<TEntity, TKey>)_repositories[repositoryKey];
            }

            var repository = new GenericRepository<TEntity, TKey>(_context);
            _repositories.Add(repositoryKey, repository);
            return repository;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            if (_context.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
            {
                _transaction = await _context.Database.BeginTransactionAsync();
            }
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
