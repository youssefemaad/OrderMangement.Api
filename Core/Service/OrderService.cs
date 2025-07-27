using AutoMapper;
using OrderManagement.Core.DomainLayer.Contracts;
using OrderManagement.Core.DomainLayer.Entities;
using OrderManagement.Core.ServiceAbstraction;
using OrderMangement.Api.Shared.DataTransferObject;

namespace OrderManagement.Core.Service
{
    public class OrderService(IUnitOfWork _unitOfWork, IMapper _mapper) : IOrderService
    {

        public async Task<OrderDto> CreateOrderAsync(OrderDto orderDto)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Validate order
                var isValid = await ValidateOrderAsync(orderDto.OrderItems);
                if (!isValid)
                    throw new InvalidOperationException("Order validation failed. Insufficient stock.");

                // Calculate total with discounts
                var total = await CalculateOrderTotalAsync(orderDto.OrderItems);

                var order = _mapper.Map<Order>(orderDto);
                order.TotalAmount = total;
                order.OrderDate = DateTime.UtcNow;
                order.Status = "Pending";

                var createdOrder = await _unitOfWork.Orders.AddAsync(order);

                // Update stock for each item
                foreach (var item in orderDto.OrderItems)
                {
                    var stockUpdated = await _unitOfWork.Products.UpdateStockAsync(item.ProductId, item.Quantity);
                    if (!stockUpdated)
                        throw new InvalidOperationException($"Failed to update stock for product {item.ProductId}");
                }

                // Generate invoice
                var invoice = new Invoice
                {
                    OrderId = createdOrder.OrderId,
                    InvoiceDate = DateTime.UtcNow,
                    TotalAmount = total
                };

                await _unitOfWork.Invoices.AddAsync(invoice);

                // Save all changes in a single transaction
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return _mapper.Map<OrderDto>(createdOrder);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetOrderWithDetailsAsync(orderId);
            return order == null ? null : _mapper.Map<OrderDto>(order);
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _unitOfWork.Orders.GetAllAsync();
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            var result = await _unitOfWork.Orders.UpdateOrderStatusAsync(orderId, status);
            if (result)
                await _unitOfWork.SaveChangesAsync();
            return result;
        }

        public Task<decimal> CalculateOrderTotalAsync(IEnumerable<OrderItemDto> orderItems)
        {
            decimal subtotal = 0;

            foreach (var item in orderItems)
            {
                var itemTotal = item.Quantity * item.UnitPrice;
                var discountAmount = itemTotal * (item.Discount / 100);
                subtotal += itemTotal - discountAmount;
            }

            // Apply tiered discounts
            decimal discount = 0;
            if (subtotal > 200)
                discount = subtotal * 0.10m; // 10% off for orders over $200
            else if (subtotal > 100)
                discount = subtotal * 0.05m; // 5% off for orders over $100

            return Task.FromResult(subtotal - discount);
        }

        public async Task<bool> ValidateOrderAsync(IEnumerable<OrderItemDto> orderItems)
        {
            foreach (var item in orderItems)
            {
                var isAvailable = await _unitOfWork.Products.IsStockAvailableAsync(item.ProductId, item.Quantity);
                if (!isAvailable)
                    return false;
            }
            return true;
        }
    }
}
