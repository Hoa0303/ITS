using ITS_BE.Data;
using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;
using ITS_BE.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ITS_BE.Repository.FavoriteRepository
{
    public class FavoriterRepository(MyDbContext dbcontext) : CommonRepository<Favorite>(dbcontext), IFavoriterRepository
    {
        private readonly MyDbContext _dbcontext = dbcontext;

        public override async Task<IEnumerable<Favorite>> GetPagedAsync<TKey>(int page, int pageSize, Expression<Func<Favorite, bool>>? expression, Expression<Func<Favorite, TKey>> orderBy)
       => expression == null
           ? await _dbcontext.Favorites
               .Paginate(page, pageSize)
               .Include(e => e.Product)
                   .ThenInclude(e => e.Images)
               .OrderBy(orderBy)
               .ToArrayAsync()
           : await _dbcontext.Favorites
               .Where(expression)
               .Paginate(page, pageSize)
               .Include(e => e.Product)
                   .ThenInclude(e => e.Images)
               .OrderBy(orderBy)
               .ToArrayAsync();
    }
}
