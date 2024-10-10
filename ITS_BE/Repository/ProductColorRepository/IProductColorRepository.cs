using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;
using System.Linq.Expressions;

namespace ITS_BE.Repository.ProductColorRepository
{
    public interface IProductColorRepository : ICommonRepository<Product_Color>
    {
        Task<Product_Color?> GetFirstColorByProductAsync(int id);
        Task<IEnumerable<Product_Color>> GetColorProductAsync(int ProductId);
    }
}
