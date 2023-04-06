using AutoMapper;
using GeekBurger.Ingredients.Api.Models;
using GeekBurger.Ingredients.Contracts;
using GeekBurger.Products.Contract;

namespace GeekBurger.Ingredients.Api.AutoMapper
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<Product, ProductResponse>();
            CreateMap<ProductToGet, Product>();
            CreateMap<ItemToGet, Item>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ItemId));
        }
    }
}
