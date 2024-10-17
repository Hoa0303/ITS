using ITS_BE.Data;
using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ITS_BE.Repository.OrderRepository
{
    public class OrderRepository(MyDbContext dbContext) : CommonRepository<Order>(dbContext), IOrderRepository
    {
        private readonly MyDbContext _dbContext = dbContext;

        public Task<Order?> SingleOrDefaultAsyncInclue(Expression<Func<Order, bool>> expression)
        {
            return _dbContext.Orders
                .Include(e => e.OrderDetials)
                .SingleOrDefaultAsync(expression);
        }
    }
}