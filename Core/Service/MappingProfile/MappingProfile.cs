using AutoMapper;
using OrderManagement.Core.DomainLayer.Entities;
using OrderMangement.Api.Shared.DataTransferObject;

namespace OrderManagement.Core.Service.MappingProfile
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<OrderItem, OrderItemDto>().ReverseMap();
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Invoice, InvoiceDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
