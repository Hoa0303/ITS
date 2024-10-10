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
            CreateMap<User, UserResponse>().ReverseMap();
            CreateMap<DeliveryAddress, AddressDTO>().ReverseMap();

            CreateMap<Brand, BrandDTO>().ReverseMap();
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Color, ColorDTO>().ReverseMap();

            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Product, VersionDTO>().ReverseMap();
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

            CreateMap<Product_Details, ProductDetailRespone>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Product.Id))
               .ForMember(dest => dest.BrandId, opt => opt.MapFrom(src => src.Product.Brand.Id))
               .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Product.Brand.Name))
               .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Product.Category.Name))
               .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Product.CategoryId))
               .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Product.Discount))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Product.Name));

            CreateMap<Product_Color, ColorResponse>()
                .ForMember(dest => dest.ColorId, opt => opt.MapFrom(src => src.ColorId))
                .ForMember(dest => dest.Prices, opt => opt.MapFrom(src => src.Prices))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl));

            CreateMap<PaymentMethodDTO, PaymentMethod>().ReverseMap();

            CreateMap<OrderDTO, Order>().ReverseMap()
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethodName));
        }
    }
}
