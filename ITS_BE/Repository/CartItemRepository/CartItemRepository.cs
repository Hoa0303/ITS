using ITS_BE.Data;
using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ITS_BE.Repository.CartItemRepository
{
    public class CartItemRepository(MyDbContext context) : CommonRepository<CartItem>(context), ICartItemRepository
    {
        private readonly MyDbContext _context = context;

        public override async Task<IEnumerable<CartItem>> GetAsync(Expression<Func<CartItem, bool>> expression)
        {
            return await _context.CartItems
                .Include(e => e.Product)
                    .ThenInclude(e => e.Product_Colors)
                    .ThenInclude(e => e.Color)
                .Include(e => e.Product)
                    .ThenInclude(e => e.Category)
                .Include(e => e.Product)
                    .ThenInclude(e => e.Details)
                .AsSingleQuery()
                .Where(expression).ToListAsync();
        }
        public async Task<CartItem?> SingleOrDefaultAsyncInclude(Expression<Func<CartItem, bool>> expression)
        {
            return await _context.CartItems
                .Include(e => e.Product)
                    .ThenInclude(e => e.Product_Colors)
                    .ThenInclude(e => e.Color)
                .Include(e => e.Product)
                    .ThenInclude(e => e.Category)
                .Include(e => e.Product)
                    .ThenInclude(e => e.Details)
                 .AsSingleQuery()
                 .SingleOrDefaultAsync(expression);
        }
    }
}
