using ITS_BE.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace ITS_BE.Repository.TransactionRepository
{
    public class TransactionRepository(MyDbContext dbContext) : ITransactionRepository
    {
        private readonly MyDbContext _dbContext = dbContext;

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _dbContext.Database.BeginTransactionAsync();
        }

        public Task CommitTransactionAsync()
        {
            return _dbContext.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _dbContext.Database.RollbackTransactionAsync();
        }
    }
}
