using DomainLayer.Entities;

namespace OrderManagement.Core.DomainLayer.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = [];

        // Navigation properties
        public Customer Customer { get; set; } = null!;
        public Invoice? Invoice { get; set; }
    }
}
