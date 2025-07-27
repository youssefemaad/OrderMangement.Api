using DomainLayer.Entities;

namespace OrderManagement.Core.DomainLayer.Entities
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public int OrderId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }

        // Navigation property
        public Order Order { get; set; } = null!;
    }
}
