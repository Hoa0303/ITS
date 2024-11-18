using ITS_BE.Data;
using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;
using ITS_BE.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ITS_BE.Repository.ProductRepository
{
    public class ProductRepository : CommonRepository<Product>, IProductRepository
    {
        private readonly MyDbContext _context;
        public ProductRepository(MyDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<int> CountAsync(string search)
        {
            return await _context.Products
                .Where(e => e.Name.Contains(search) || e.Id.ToString().Equals(search))
                .CountAsync();
        }
        public async Task<IEnumerable<Product>> GetPageProduct(int page, int pageSize, string search)
        {
            return await _context.Products
                .Where(e => e.Name.Contains(search) || e.Id.ToString().Equals(search))
                .Include(e => e.Brand)
                .Include(e => e.Category)
                .OrderBy(e => e.Name)
                .Paginate(page, pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetPageProduct(int page, int pageSize)
        {
            return await _context.Products
                .Include(e => e.Brand)
                .Include(e => e.Category)
                .OrderBy(e => e.Name)
                .Paginate(page, pageSize)                
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> OrderByDescendingBySold(int page, int pageSize)
        {
            return await _context.Products
                .Include(e => e.Brand)
                .Include(e => e.Category)
                .OrderByDescending(e => e.Sold)
                .Paginate(page, pageSize)
                .ToListAsync();
        }

        public override async Task<Product?> SingleOrDefaultAsync(Expression<Func<Product, bool>> expression)
        {
            return await _context.Products
               .Include(e => e.Brand)
               .Include(e => e.Category)
               .Include(e => e.Details)
               .Include(e => e.Images)
               .Include(e => e.Product_Colors)
               .AsSingleQuery()
               .SingleOrDefaultAsync(expression);
        }

        public async Task<Product?> GetProductById(int id)
        {
            return await _context.Products
                .Include(e => e.Brand)
                .Include(e => e.Category)
                .Include(e => e.Details)
                .Include(e => e.Images)
                .Include(e => e.Product_Colors)
                .AsSingleQuery()
                .SingleOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Product?> SingleOrDefaultAsync(int id)
        {
            return await _context.Products.Include(e => e.Images).SingleOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Product>> SearchAsync(string search)
        {
            return await _context.Products
                .Where(p => p.Name.Contains(search) || (p.Details.line != "Default" && p.Details.line.Contains(search)))
                .ToListAsync();
        }

        public async Task<List<Product>> GetListAsync(Expression<Func<Product, bool>> predicate)
        {
            return await _context.Products
                .Include(e => e.Brand)
                .Include(e => e.Category)
                .Include(e => e.Details)
                .Include(e => e.Product_Colors)
                .Where(predicate).ToListAsync();
        }

    }
}
