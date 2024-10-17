using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;
using System.Linq.Expressions;

namespace ITS_BE.Repository.OrderRepository
{
    public interface IOrderRepository : ICommonRepository<Order>
    {
        Task<Order?> SingleOrDefaultAsyncInclue(Expression<Func<Order, bool>> expression);
    }
}