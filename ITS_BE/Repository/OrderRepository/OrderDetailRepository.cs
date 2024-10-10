using ITS_BE.Data;
using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ITS_BE.Repository.OrderRepository
{
    public class OrderDetailRepository(MyDbContext dbContext) : CommonRepository<OrderDetail>(dbContext), IOrderDetailRepository
    {
        private readonly MyDbContext _dbContext = dbContext;

        public override async Task<IEnumerable<OrderDetail>> GetAsync(Expression<Func<OrderDetail, bool>> expression)
        {
            return await _dbContext.OrderDetials
                .Where(expression)
                .Include(e => e.Product)
                .ToListAsync();
        }
    }
}
