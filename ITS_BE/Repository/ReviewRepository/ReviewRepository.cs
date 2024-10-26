using ITS_BE.Data;
using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;
using ITS_BE.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ITS_BE.Repository.ReviewRepository
{
    public class ReviewRepository(MyDbContext dbContext) : CommonRepository<Review>(dbContext), IReviewRepository
    {
        private readonly MyDbContext _dbContext = dbContext;

        public override async Task<IEnumerable<Review>> GetPagedAsync<TKey>(int page, int pageSize, Expression<Func<Review, bool>>? expression, Expression<Func<Review, TKey>> orderBy)
            => expression == null
                ? await _dbContext.Reviews
                    .Paginate(page, pageSize)
                    .OrderBy(orderBy)
                    .Include(e => e.User).ToArrayAsync()
                : await _dbContext.Reviews
                    .Where(expression)
                    .Paginate(page, pageSize)
                    .OrderBy(orderBy)
                    .Include(e => e.User).ToArrayAsync();

        public override async Task<IEnumerable<Review>> GetPagedOrderByDescendingAsync<TKey>(int page, int pageSize, Expression<Func<Review, bool>>? expression, Expression<Func<Review, TKey>> orderByDesc)
            => expression == null
                ? await _dbContext.Reviews
                    .Paginate(page, pageSize)
                    .OrderByDescending(orderByDesc)
                    .Include(e => e.User).ToArrayAsync()
                : await _dbContext.Reviews
                    .Where(expression)
                    .Paginate(page, pageSize)
                    .OrderByDescending(orderByDesc)
                    .Include(e => e.User).ToArrayAsync();
    }
}
