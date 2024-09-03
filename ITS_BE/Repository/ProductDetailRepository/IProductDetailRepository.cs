using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;

namespace ITS_BE.Repository.ProductDetailRepository
{
    public interface IProductDetailRepository : ICommonRepository<Product_Details>
    {
        Task<IEnumerable<Product_Details>> GetDetailProductAsync(int ProductId);
    }
}
