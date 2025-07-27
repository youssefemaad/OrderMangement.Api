using Microsoft.EntityFrameworkCore;
using OrderManagement.Core.DomainLayer.Contracts;
using OrderManagement.Core.DomainLayer.Entities;
using Persistence.Data;

namespace Persistence.Repository
{
    public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(OrderManagementDbContext context) : base(context)
        {
        }

        public async Task<Invoice?> GetInvoiceByOrderIdAsync(int orderId)
        {
            return await _context.Invoices
                .Include(i => i.Order)
                    .ThenInclude(o => o.Customer)
                .Include(i => i.Order)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(i => i.OrderId == orderId);
        }
    }
}
