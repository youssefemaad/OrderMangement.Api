using OrderMangement.Api.Shared.DataTransferObject;

namespace OrderManagement.Core.ServiceAbstraction
{
    public interface ICustomerService
    {
        Task<CustomerDto> CreateCustomerAsync(CustomerDto customerDto);
        Task<IEnumerable<OrderDto>> GetCustomerOrdersAsync(int customerId);
        Task<CustomerDto?> GetCustomerByIdAsync(int customerId);
    }
}
