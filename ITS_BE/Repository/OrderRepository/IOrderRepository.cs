using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;
using ITS_BE.Response;
using System.Linq.Expressions;

namespace ITS_BE.Repository.OrderRepository
{
    public interface IOrderRepository : ICommonRepository<Order>
    {
        Task<Order?> SingleOrDefaultAsyncInclue(Expression<Func<Order, bool>> expression);
        Task<IEnumerable<StatisticData>> GetTotalSalesByYear(int year, int? month);
        Task<IEnumerable<StatisticDateData>> GetTotalSales(DateTime dateFrom, DateTime dateTo);

    }
}