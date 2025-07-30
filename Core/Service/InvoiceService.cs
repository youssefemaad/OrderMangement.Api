using AutoMapper;
using OrderManagement.Core.DomainLayer.Contracts;
using OrderManagement.Core.DomainLayer.Entities;
using OrderManagement.Core.ServiceAbstraction;
using OrderMangement.Api.Shared.DataTransferObject;

namespace OrderManagement.Core.Service
{
    public class InvoiceService(IUnitOfWork unitOfWork, IMapper mapper) : IInvoiceService
    {
        public async Task<InvoiceDto?> GetInvoiceByIdAsync(int invoiceId)
        {
            var invoiceRepo = unitOfWork.GetRepository<Invoice, int>();
            var invoice = await invoiceRepo.GetByIdAsync(invoiceId);
            return invoice == null ? null : mapper.Map<InvoiceDto>(invoice);
        }

        public async Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync()
        {
            var invoiceRepo = unitOfWork.GetRepository<Invoice, int>();
            var invoices = await invoiceRepo.GetAllAsync();
            return mapper.Map<IEnumerable<InvoiceDto>>(invoices);
        }

        public async Task<InvoiceDto> GenerateInvoiceAsync(int orderId)
        {
            var orderRepo = unitOfWork.GetRepository<Order, int>();
            var order = await orderRepo.GetByIdAsync(orderId);
            if (order == null)
                throw new ArgumentException("Order not found");

            var invoice = new Invoice
            {
                OrderId = orderId,
                InvoiceDate = DateTime.UtcNow,
                TotalAmount = order.TotalAmount
            };

            var invoiceRepo = unitOfWork.GetRepository<Invoice, int>();
            var createdInvoice = await invoiceRepo.AddAsync(invoice);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<InvoiceDto>(createdInvoice);
        }
    }
}
