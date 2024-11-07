using Microsoft.EntityFrameworkCore.Storage;

namespace ITS_BE.Repository.TransactionRepository
{
    public interface ITransactionRepository
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
