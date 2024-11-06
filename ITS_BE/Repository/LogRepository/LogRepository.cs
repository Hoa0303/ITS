using ITS_BE.Data;
using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;
using ITS_BE.Services;
using System.Linq.Expressions;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ITS_BE.Repository.LogRepository
{
    public class LogRepository(MyDbContext dbContext) : CommonRepository<Log>(dbContext), ILogRepository
    {
        private readonly MyDbContext _dbContext = dbContext;

        public override async Task<IEnumerable<Log>> GetPagedOrderByDescendingAsync<TKey>(int page, int pageSize, Expression<Func<Log, bool>>? expression, Expression<Func<Log, TKey>> orderByDesc)
        => expression == null
            ? await _dbContext.Logs
                .Paginate(page, pageSize)
                .OrderByDescending(orderByDesc)
                .Include(e=> e.User)
                .ToArrayAsync()
            : await _dbContext.Logs
                .Where(expression)
                .Paginate(page, pageSize)
                .OrderByDescending(orderByDesc)
                .Include(e => e.User)
                .ToArrayAsync();
    }
}
