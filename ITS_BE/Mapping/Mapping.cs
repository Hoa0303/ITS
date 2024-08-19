using AutoMapper;
using ITS_BE.DTO;
using ITS_BE.Models;

namespace ITS_BE.Mapping
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<UserDTO, User>().ReverseMap();
            //CreateMap<>
            CreateMap<CategoryDTO, Category>().ReverseMap();
            CreateMap<BrandDTO, Brand>().ReverseMap();
        }
    }
}
