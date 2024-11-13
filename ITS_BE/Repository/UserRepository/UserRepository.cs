using ITS_BE.Data;
using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;
using ITS_BE.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ITS_BE.Repository.UserRepository
{
    public class UserRepository(MyDbContext dbContext)
        : CommonRepository<User>(dbContext), IUserRepository
    {
        private readonly MyDbContext _dbContext = dbContext;

        public override async Task<IEnumerable<User>> GetPagedOrderByDescendingAsync<TKey>(int page, int pageSize, Expression<Func<User, bool>>? expression, Expression<Func<User, TKey>> orderByDesc)
        => expression == null
            ? await _dbContext.Users
                .Paginate(page, pageSize)
                .OrderByDescending(orderByDesc)
                .Include(x => x.UserRoles)
                    .ThenInclude(e => e.Role)
                .ToArrayAsync()
            : await _dbContext.Users
                .Where(expression)
                .Paginate(page, pageSize)
                .OrderByDescending(orderByDesc)
                .Include(x => x.UserRoles)
                    .ThenInclude(e => e.Role)
                .ToArrayAsync();
    }
}
