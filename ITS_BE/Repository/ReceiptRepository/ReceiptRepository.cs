using ITS_BE.Data;
using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;

namespace ITS_BE.Repository.ReceiptRepository
{
    public class ReceiptRepository(MyDbContext dbContext) :CommonRepository<Receipt>(dbContext), IReceiptRepository
    {
        private readonly MyDbContext _dbContext = dbContext;
    }
}
