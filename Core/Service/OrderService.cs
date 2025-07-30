using AutoMapper;
using OrderManagement.Core.DomainLayer.Contracts;
using OrderManagement.Core.DomainLayer.Entities;
using OrderManagement.Core.ServiceAbstraction;
using OrderMangement.Api.Shared.DataTransferObject;

namespace OrderManagement.Core.Service
{
    public class OrderService(IUnitOfWork unitOfWork, IMapper mapper) : IOrderService
    {
        public async Task<OrderDto> CreateOrderAsync(OrderDto orderDto)
        {
            await unitOfWork.BeginTransactionAsync();
            try
            {
                var isValid = await ValidateOrderAsync(orderDto.OrderItems);
                if (!isValid)
                    throw new InvalidOperationException("Order validation failed. Insufficient stock.");

                var total = await CalculateOrderTotalAsync(orderDto.OrderItems);

                var order = mapper.Map<Order>(orderDto);
                order.TotalAmount = total;
                order.OrderDate = DateTime.UtcNow;
                order.Status = "Pending";

                var orderRepo = unitOfWork.GetRepository<Order, int>();
                var createdOrder = await orderRepo.AddAsync(order);

                var productRepo = unitOfWork.GetRepository<Product, int>();
                foreach (var item in orderDto.OrderItems)
                {
                    var product = await productRepo.GetByIdAsync(item.ProductId);
                    if (product != null && product.Stock >= item.Quantity)
                    {
                        product.Stock -= item.Quantity;
                        await productRepo.UpdateAsync(product);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Failed to update stock for product {item.ProductId}");
                    }
                }

                var invoice = new Invoice
                {
                    OrderId = createdOrder.OrderId,
                    InvoiceDate = DateTime.UtcNow,
                    TotalAmount = total
                };

                var invoiceRepo = unitOfWork.GetRepository<Invoice, int>();
                await invoiceRepo.AddAsync(invoice);

                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitTransactionAsync();

                return mapper.Map<OrderDto>(createdOrder);
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId)
        {
            var orderRepo = unitOfWork.GetRepository<Order, int>();
            var order = await orderRepo.GetByIdAsync(orderId);
            return order == null ? null : mapper.Map<OrderDto>(order);
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orderRepo = unitOfWork.GetRepository<Order, int>();
            var orders = await orderRepo.GetAllAsync();
            return mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            var orderRepo = unitOfWork.GetRepository<Order, int>();
            var order = await orderRepo.GetByIdAsync(orderId);
            if (order == null)
                return false;

            order.Status = status;
            await orderRepo.UpdateAsync(order);
            await unitOfWork.SaveChangesAsync();
            return true;
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

            decimal discount = 0;
            if (subtotal > 200)
                discount = subtotal * 0.10m;
            else if (subtotal > 100)
                discount = subtotal * 0.05m;

            return Task.FromResult(subtotal - discount);
        }

        public async Task<bool> ValidateOrderAsync(IEnumerable<OrderItemDto> orderItems)
        {
            var productRepo = unitOfWork.GetRepository<Product, int>();
            foreach (var item in orderItems)
            {
                var product = await productRepo.GetByIdAsync(item.ProductId);
                if (product == null || product.Stock < item.Quantity)
                    return false;
            }
            return true;
        }
    }
}
