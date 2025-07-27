using System;
using System.Collections.Generic;
using DomainLayer.Entities;


namespace OrderManagement.Core.DomainLayer.Entities
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public ICollection<Order> Orders { get; set; } = [];
    }
}