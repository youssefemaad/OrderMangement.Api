using OrderManagement.Core.DomainLayer.Entities;

namespace OrderManagement.Core.DomainLayer.Contracts
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order?> GetOrderWithDetailsAsync(int orderId);
        Task<IEnumerable<Order>> GetOrdersByCustomerAsync(int customerId);
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);
    }
}