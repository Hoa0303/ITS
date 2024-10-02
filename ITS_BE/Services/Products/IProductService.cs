using ITS_BE.DTO;
using ITS_BE.Request;
using ITS_BE.Response;

namespace ITS_BE.Services.Products
{
    public interface IProductService
    {
        Task<PageRespone<ProductDTO>> GetAllProduct(int page, int pageSize, string? search);
        Task<PageRespone<ProductDTO>> GetFilterProductAsync(Filters filters);
        Task<IEnumerable<VersionDTO>> GetAllProductVersionsAsync(string search);
        Task<ProductDTO> CreateProduct(ProductRequest request, IFormFileCollection images);
        Task<ProductDetailRespone> GetProductById(int id);
        Task<ProductDTO> UpdateProduct(int id, ProductRequest request, IFormFileCollection images);
        Task<bool> UpdateEnableRequest(int id, UpdateEnableRequest request);
        Task Delete(int id);
    }
}