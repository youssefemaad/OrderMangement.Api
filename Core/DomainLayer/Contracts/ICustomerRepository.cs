using OrderManagement.Core.DomainLayer.Entities;

namespace OrderManagement.Core.DomainLayer.Contracts
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetCustomerWithOrdersAsync(int customerId);
        Task<IEnumerable<Order>> GetCustomerOrdersAsync(int customerId);
    }
}