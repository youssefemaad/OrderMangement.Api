using OrderMangement.Api.Shared.DataTransferObject;

namespace OrderManagement.Core.ServiceAbstraction
{
    public interface IInvoiceService
    {
        Task<InvoiceDto?> GetInvoiceByIdAsync(int invoiceId);
        Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync();
        Task<InvoiceDto> GenerateInvoiceAsync(int orderId);
    }
}
