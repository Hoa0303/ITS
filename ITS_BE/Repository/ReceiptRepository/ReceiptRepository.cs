using ITS_BE.Data;
using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;
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
    }
}
