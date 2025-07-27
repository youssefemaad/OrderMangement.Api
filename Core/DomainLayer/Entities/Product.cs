using DomainLayer.Entities;

namespace OrderManagement.Core.DomainLayer.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        // Navigation property
        public ICollection<OrderItem> OrderItems { get; set; } = [];
    }
}
