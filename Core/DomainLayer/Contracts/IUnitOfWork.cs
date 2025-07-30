using OrderManagement.Core.DomainLayer.Contracts;

namespace OrderManagement.Core.DomainLayer.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : class;

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
