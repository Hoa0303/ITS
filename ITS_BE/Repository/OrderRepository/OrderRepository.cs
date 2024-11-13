using ITS_BE.Data;
using ITS_BE.Enumerations;
using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;
using ITS_BE.Response;
using ITS_BE.Services;
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

        public override async Task<IEnumerable<Order>> GetPagedOrderByDescendingAsync<TKey>(int page, int pageSize, Expression<Func<Order, bool>>? expression, Expression<Func<Order, TKey>> orderByDesc)
             => expression == null
            ? await _dbContext.Orders
                .OrderByDescending(orderByDesc)
                .Paginate(page, pageSize)
                .ToArrayAsync()
            : await _dbContext.Orders
                .Where(expression)
                .OrderByDescending(orderByDesc)
                .Paginate(page, pageSize)
                .ToArrayAsync();

        public async Task<IEnumerable<StatisticData>> GetTotalSalesByYear(int year, int? month)
        => month == null
            ? await _dbContext.Orders
                .Where(e => e.ReceivedDate.Year == year &&
                  (e.OrderStatus == DeliveryStatusEnum.Received || e.OrderStatus == DeliveryStatusEnum.Done))
                .GroupBy(e => new { e.ReceivedDate.Month, e.ReceivedDate.Year })
                .Select(g => new StatisticData
                {
                    Time = g.Key.Month,
                    Statistic = g.Sum(x => x.Total)
                }).ToArrayAsync()
            : await _dbContext.Orders
                .Where(e => e.ReceivedDate.Year == year && e.ReceivedDate.Month == month &&
                  (e.OrderStatus == DeliveryStatusEnum.Received || e.OrderStatus == DeliveryStatusEnum.Done))
                .GroupBy(e => new { e.ReceivedDate.Day, e.ReceivedDate.Month })
                .Select(g => new StatisticData
                {
                    Time = g.Key.Day,
                    Statistic = g.Sum(x => x.Total)
                }).ToArrayAsync();

        public async Task<IEnumerable<StatisticDateData>> GetTotalSales(DateTime dateFrom, DateTime dateTo)
        {
            return await _dbContext.Orders
                .Where(e => e.ReceivedDate >= dateFrom && e.ReceivedDate <= dateTo.AddDays(1) &&
                    (e.OrderStatus == DeliveryStatusEnum.Received || e.OrderStatus == DeliveryStatusEnum.Done))
                .GroupBy(e => new { e.ReceivedDate.Date })
                .Select(g => new StatisticDateData
                {
                    Time = g.Key.Date,
                    Statistic = g.Sum(x => x.Total)
                }).ToArrayAsync();
        }

        public async Task<IEnumerable<StatisticData>> GetTotalProductSalesByYear(int productId, int year, int? month)
        => month == null
            ? await _dbContext.Orders
            .Where(e => e.ReceivedDate.Year == year &&
                  (e.OrderStatus == DeliveryStatusEnum.Received || e.OrderStatus == DeliveryStatusEnum.Done))
            .SelectMany(r => r.OrderDetials)
            .Where(e => e.ProductId == productId)
            .GroupBy(e => e.Order.ReceivedDate.Month)
            .Select(g => new StatisticData
            {
                Time = g.Key,
                Statistic = g.Sum(x => x.Price)
            }).ToArrayAsync()
            : await _dbContext.Orders
            .Where(e => e.ReceivedDate.Year == year && e.ReceivedDate.Month == month &&
                  (e.OrderStatus == DeliveryStatusEnum.Received || e.OrderStatus == DeliveryStatusEnum.Done))
            .SelectMany(r => r.OrderDetials)
            .Where(e => e.ProductId == productId)
            .GroupBy(e => e.Order.ReceivedDate.Day)
            .Select(g => new StatisticData
            {
                Time = g.Key,
                Statistic = g.Sum(x => x.Price)
            }).ToArrayAsync();

        public async Task<IEnumerable<StatisticDateData>> GetTotalProductSales(int productId, DateTime dateFrom, DateTime dateTo)
        {
            return await _dbContext.Orders
                .Where(e => e.ReceivedDate >= dateFrom && e.ReceivedDate <= dateTo.AddDays(1) &&
                    (e.OrderStatus == DeliveryStatusEnum.Received || e.OrderStatus == DeliveryStatusEnum.Done))
                .SelectMany(s => s.OrderDetials)
                .Where(e => e.ProductId == productId)
                .GroupBy(gb => gb.Order.ReceivedDate.Date)
                .Select(g => new StatisticDateData
                {
                    Time = g.Key,
                    Statistic = g.Sum(x => x.Price)
                }).ToArrayAsync();
        }
    }
}