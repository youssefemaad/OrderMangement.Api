using AutoMapper;
using OrderManagement.Core.DomainLayer.Contracts;
using OrderManagement.Core.DomainLayer.Entities;
using OrderManagement.Core.ServiceAbstraction;
using OrderMangement.Api.Shared.DataTransferObject;

namespace OrderManagement.Core.Service
{
    public class CustomerService(IUnitOfWork unitOfWork, IMapper mapper) : ICustomerService
    {
        public async Task<CustomerDto> CreateCustomerAsync(CustomerDto customerDto)
        {
            var customerRepo = unitOfWork.GetRepository<Customer, int>();
            var customer = mapper.Map<Customer>(customerDto);
            var createdCustomer = await customerRepo.AddAsync(customer);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<CustomerDto>(createdCustomer);
        }

        public async Task<IEnumerable<OrderDto>> GetCustomerOrdersAsync(int customerId)
        {
            var orderRepo = unitOfWork.GetRepository<Order, int>();
            var orders = await orderRepo.FindAsync(o => o.CustomerId == customerId);
            return mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(int customerId)
        {
            var customerRepo = unitOfWork.GetRepository<Customer, int>();
            var customer = await customerRepo.GetByIdAsync(customerId);
            return customer == null ? null : mapper.Map<CustomerDto>(customer);
        }
    }
}

