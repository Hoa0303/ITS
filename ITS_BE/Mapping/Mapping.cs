using AutoMapper;
using ITS_BE.DTO;
using ITS_BE.Models;
using ITS_BE.Request;
using ITS_BE.Response;

namespace ITS_BE.Mapping
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<Brand, BrandDTO>().ReverseMap();
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Color, ColorDTO>().ReverseMap();
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Product, ProductDTO>()
                .ForMember(des => des.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(des => des.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<Details, Product_Details>()
               .ForMember(dest => dest.SizeScreen, opt => opt.MapFrom(src => src.SizeScreen))
               .ForMember(dest => dest.ScanHz, opt => opt.MapFrom(src => src.ScanHz))
               .ForMember(dest => dest.Material, opt => opt.MapFrom(src => src.Material))
               .ForMember(dest => dest.RearCam, opt => opt.MapFrom(src => src.RearCam))
               .ForMember(dest => dest.FrontCam, opt => opt.MapFrom(src => src.FrontCam))
               .ForMember(dest => dest.Cpu, opt => opt.MapFrom(src => src.Cpu))
               .ForMember(dest => dest.Ram, opt => opt.MapFrom(src => src.Ram))
               .ForMember(dest => dest.Rom, opt => opt.MapFrom(src => src.Rom))
               .ForMember(dest => dest.Battery, opt => opt.MapFrom(src => src.Battery))
               .ForMember(dest => dest.size, opt => opt.MapFrom(src => src.size))
               .ForMember(dest => dest.weight, opt => opt.MapFrom(src => src.weight));

            CreateMap<ProductRequest, Product>();

            CreateMap<Product, ProductDetailRespone>()
               .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
               .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
               .ForMember(dest => dest.SizeScreen, opt => opt.MapFrom(src => src.Details.SizeScreen))
               .ForMember(dest => dest.ScanHz, opt => opt.MapFrom(src => src.Details.ScanHz))
               .ForMember(dest => dest.Material, opt => opt.MapFrom(src => src.Details.Material))
               .ForMember(dest => dest.RearCam, opt => opt.MapFrom(src => src.Details.RearCam))
               .ForMember(dest => dest.FrontCam, opt => opt.MapFrom(src => src.Details.FrontCam))
               .ForMember(dest => dest.Cpu, opt => opt.MapFrom(src => src.Details.Cpu))
               .ForMember(dest => dest.Ram, opt => opt.MapFrom(src => src.Details.Ram))
               .ForMember(dest => dest.Rom, opt => opt.MapFrom(src => src.Details.Rom))
               .ForMember(dest => dest.Battery, opt => opt.MapFrom(src => src.Details.Battery))
               .ForMember(dest => dest.size, opt => opt.MapFrom(src => src.Details.size))
               .ForMember(dest => dest.weight, opt => opt.MapFrom(src => src.Details.weight))
               .ForMember(dest => dest.ImageUrls, opt => opt.Ignore())
               .ForMember(dest => dest.Color, opt => opt.Ignore());

            CreateMap<Product_Color, ColorResponse>()
                .ForMember(dest => dest.ColorId, opt => opt.MapFrom(src => src.ColorId))
                .ForMember(dest => dest.Prices, opt => opt.MapFrom(src => src.Prices))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl));
        }
    }
}
