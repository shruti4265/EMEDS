using AutoMapper;
using EMEDS_Project.Models;

namespace EMEDS_Project.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CartItem, OrderItem>()
                .ForMember(
                    destination => destination.Subtotal,
                    options => options.MapFrom(source => source.TotalPrice)
                );
        }
    }
}