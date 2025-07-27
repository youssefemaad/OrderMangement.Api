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

        private ICustomerRepository? _customers;
        private IOrderRepository? _orders;
        private IProductRepository? _products;
        private IInvoiceRepository? _invoices;
        private IUserRepository? _users;

        public UnitOfWork(OrderManagementDbContext context)
        {
            _context = context;
        }

        public ICustomerRepository Customers =>
            _customers ??= new CustomerRepository(_context);

        public IOrderRepository Orders =>
            _orders ??= new OrderRepository(_context);

        public IProductRepository Products =>
            _products ??= new ProductRepository(_context);

        public IInvoiceRepository Invoices =>
            _invoices ??= new InvoiceRepository(_context);

        public IUserRepository Users =>
            _users ??= new UserRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            // Skip transactions for in-memory database
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
