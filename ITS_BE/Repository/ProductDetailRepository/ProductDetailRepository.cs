using ITS_BE.Data;
using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;
using Microsoft.EntityFrameworkCore;

namespace ITS_BE.Repository.ProductDetailRepository
{
    public class ProductDetailRepository : CommonRepository<Product_Details>, IProductDetailRepository
    {
        private readonly MyDbContext _dbContext;
        public ProductDetailRepository(MyDbContext context) : base(context)
        {
            _dbContext = context;
        }
        public async Task<IEnumerable<Product_Details>> GetDetailProductAsync(int ProductId)
            => await _dbContext.Product_Details.Where(e => e.ProductId == ProductId).ToListAsync();
    }
}
