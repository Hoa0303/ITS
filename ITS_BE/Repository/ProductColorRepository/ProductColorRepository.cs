using ITS_BE.Data;
using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ITS_BE.Repository.ProductColorRepository
{
    public class ProductColorRepository : CommonRepository<Product_Color>, IProductColorRepository
    {
        private readonly MyDbContext _dbContext;
        public ProductColorRepository(MyDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Product_Color>> GetColorProductAsync(int ProductId)
            => await _dbContext.Product_Colors.Where(e => e.ProductId == ProductId).ToListAsync();

        public async Task<Product_Color?> GetFirstColorByProductAsync(int id)
         => await _dbContext.Product_Colors.FirstOrDefaultAsync(e => e.ProductId == id);
    }
}
