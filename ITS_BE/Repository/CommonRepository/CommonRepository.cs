using ITS_BE.Data;
using ITS_BE.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ITS_BE.Repository.CommonRepository
{
    public class CommonRepository<T>(MyDbContext context) : ICommonRepository<T> where T : class
    {
        private readonly MyDbContext _context = context;

        public virtual async Task AddAsync(T entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task AddAsync(IEnumerable<T> entities)
        {
            await _context.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<int> CountAsync() => await _context.Set<T>().CountAsync();

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> expression)
            => await _context.Set<T>().
                Where(expression)
                .CountAsync();

        public virtual async Task DeleteAsync(T entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(params object?[]? keyValues)
        {
            var entity = await _context.FindAsync<T>(keyValues) 
                ?? throw new ArgumentException($"Entity with specified keys not found.");

            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(IEnumerable<T> entities)
        {
            _context.RemoveRange(entities);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<T?> FindAsync(params object?[]? keyValues)
        {
            return await _context.FindAsync<T>(keyValues);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> expression)
           => await _context.Set<T>().Where(expression).ToArrayAsync();

        public virtual async Task<IEnumerable<T>> GetPagedAsync<TKey>(int page, int pageSize, Expression<Func<T, bool>>? expression, Expression<Func<T, TKey>> orderBy)
            => expression == null
            ? await _context.Set<T>().Paginate(page, pageSize).OrderBy(orderBy).ToArrayAsync()
            : await _context.Set<T>().Where(expression).Paginate(page, pageSize).OrderBy(orderBy).ToArrayAsync();

        public virtual async Task<IEnumerable<T>> GetPagedOrderByDescendingAsync<TKey>(int page, int pageSize, Expression<Func<T, bool>>? expression, Expression<Func<T, TKey>> orderByDesc)
            => expression == null
            ? await _context.Set<T>()
                .Paginate(page, pageSize)
                .OrderByDescending(orderByDesc)
                .ToArrayAsync()
            : await _context.Set<T>()
                .Where(expression)
                .Paginate(page, pageSize)
                .OrderByDescending(orderByDesc)
                .ToArrayAsync();

        public async Task<T> SingleAsync(Expression<Func<T, bool>> expression)
            => await _context.Set<T>().SingleAsync(expression);

        public virtual async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> expression)
            => await _context.Set<T>().SingleOrDefaultAsync(expression);

        public virtual async Task UpdateAsync(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(IEnumerable<T> entities)
        {
            _context.UpdateRange(entities);
            await _context.SaveChangesAsync();
        }
    }
}
