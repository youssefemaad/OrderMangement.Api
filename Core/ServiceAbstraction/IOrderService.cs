using OrderMangement.Api.Shared.DataTransferObject;

namespace OrderManagement.Core.ServiceAbstraction
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(OrderDto orderDto);
        Task<OrderDto?> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);
        Task<decimal> CalculateOrderTotalAsync(IEnumerable<OrderItemDto> orderItems);
        Task<bool> ValidateOrderAsync(IEnumerable<OrderItemDto> orderItems);
    }
}
