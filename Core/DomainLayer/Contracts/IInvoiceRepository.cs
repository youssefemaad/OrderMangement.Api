using OrderManagement.Core.DomainLayer.Entities;

namespace OrderManagement.Core.DomainLayer.Contracts
{
    public interface IInvoiceRepository : IRepository<Invoice>
    {
        Task<Invoice?> GetInvoiceByOrderIdAsync(int orderId);
    }
}