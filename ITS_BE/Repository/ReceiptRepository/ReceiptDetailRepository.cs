using ITS_BE.Data;
using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ITS_BE.Repository.ReceiptRepository
{
    public class ReceiptDetailRepository(MyDbContext dbContext) : CommonRepository<ReceiptDetail>(dbContext),
        IReceiptDetailRepository
    {
        private readonly MyDbContext _dbContext = dbContext;

        public override async Task<IEnumerable<ReceiptDetail>> GetAsync(Expression<Func<ReceiptDetail, bool>> expression)
        {
            return await _dbContext.ReceiptDetails
                .Include(e=>e.Product)
                .Include(e=>e.Color)
                .AsSingleQuery()
                .Where(expression).ToListAsync();
        }
    }
}
