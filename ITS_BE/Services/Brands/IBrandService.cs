using ITS_BE.DTO;

namespace ITS_BE.Services.Brands
{
    public interface IBrandService
    {
        Task<IEnumerable<BrandDTO>> GetAllBrand();
        Task<BrandDTO> CreateBrand(string name, IFormFile img);
        Task<BrandDTO> UpdateBrand(int id, string Name, IFormFile? img);
        Task DeleteBrand(int id);
    }
}
