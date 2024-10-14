using AutoMapper;
using ECommerceAPI.DTO;
using ECommerceAPI.Models;

namespace ECommerceAPI.Mappings
{
    public class MyMappingProfile : Profile
    {
        public MyMappingProfile()
        {
            CreateMap<CustomerDTO, Customer>(MemberList.None).ReverseMap();

            CreateMap<CustomerResponseDTO, CustomerDTO>(MemberList.None).ReverseMap();

            CreateMap<ProductDTO, ProductResponseDTO>(MemberList.None).ReverseMap();
            CreateMap<Product, ProductDTO>(MemberList.None).ReverseMap();


            CreateMap<Order, OrderDTO>(MemberList.None).ReverseMap();
            CreateMap<OrderItem, OrderItemDetailsDTO>(MemberList.None).ReverseMap();
            CreateMap<Order, CreateOrderResponseDTO>(MemberList.None).ReverseMap();


            CreateMap<Payment, PaymentDTO>(MemberList.None).ReverseMap();
            CreateMap<Payment, PaymentResponseDTO>(MemberList.None).ReverseMap();
        }
    }
}
