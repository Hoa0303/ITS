using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;
using System.Linq.Expressions;

namespace ITS_BE.Repository.ProductRepository
{
    public interface IProductRepository : ICommonRepository<Product>
    {
        Task<IEnumerable<Product>> GetPageProduct(int page, int pageSize, string search);
        Task<IEnumerable<Product>> GetPageProduct(int page, int pageSize);
        Task<Product?> SingleOrDefaultAsync(int id);
        Task<IEnumerable<Product>> SearchAsync(string search);
        Task<int> CountAsync(string search);
        Task<Product?> GetProductById(int id);
        Task<List<Product>> GetListAsync(Expression<Func<Product, bool>> predicate);
    }
}
