using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;

namespace ITS_BE.Repository.ProductRepository
{
    public interface IProductRepository : ICommonRepository<Product>
    {
        Task<IEnumerable<Product>> GetPageProduct(int page, int pageSize, string search);
        Task<IEnumerable<Product>> GetPageProduct(int page, int pageSize);
        Task<Product?> SingleOrDefaultAsync(int id);
        Task<int> CountAsync(string search);
        Task<Product?> GetProductById(int id);
    }
}
