using ITS_BE.Data;
using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;
using ITS_BE.Response;
using ITS_BE.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ITS_BE.Repository.ReceiptRepository
{
    public class ReceiptRepository(MyDbContext dbContext) : CommonRepository<Receipt>(dbContext), IReceiptRepository
    {
        private readonly MyDbContext _dbContext = dbContext;

        public override async Task<IEnumerable<Receipt>> GetPagedOrderByDescendingAsync<TKey>(int page, int pageSize, Expression<Func<Receipt, bool>>? expression, Expression<Func<Receipt, TKey>> orderByDesc)
        => expression == null
            ? await _dbContext.Receipts
                .Paginate(page, pageSize)
                .OrderByDescending(orderByDesc)
                .Include(e => e.User)
                .ToArrayAsync()
            : await _dbContext.Receipts
                .Where(expression)
                .Paginate(page, pageSize)
                .OrderByDescending(orderByDesc)
                .Include(e => e.User)
                .ToArrayAsync();

        public async Task<IEnumerable<StatisticDateData>> GetTotalSpending(DateTime dateFrom, DateTime dateTo)
        {
            return await _dbContext.Receipts
                .Where(e => e.EntryDate >= dateFrom && e.EntryDate <= dateTo)
                .GroupBy(e => new { e.EntryDate.Date })
                .Select(g => new StatisticDateData
                {
                    Time = g.Key.Date,
                    Statistic = g.Sum(x => x.Total)
                }).ToArrayAsync();
        }

        public async Task<IEnumerable<StatisticData>> GetTotalSpendingByYear(int year, int? month)
        => month == null
            ? await _dbContext.Receipts
                .Where(e => e.EntryDate.Year == year)
                .GroupBy(e => new { e.EntryDate.Month, e.EntryDate.Year })
                .Select(g => new StatisticData
                {
                    Time = g.Key.Month,
                    Statistic = g.Sum(e => e.Total)
                }).ToArrayAsync()
            : await _dbContext.Receipts
                .Where(e => e.EntryDate.Year == year && e.EntryDate.Month == month)
                .GroupBy(e => new { e.EntryDate.Day, e.EntryDate.Month })
                .Select(g => new StatisticData
                {
                    Time = g.Key.Day,
                    Statistic = g.Sum(x => x.Total)
                }).ToArrayAsync();
    }
}
