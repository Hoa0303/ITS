using ITS_BE.Data;
using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;

namespace ITS_BE.Repository.ReceiptRepository
{
    public class ReceiptDetailRepository(MyDbContext dbContext) : CommonRepository<ReceiptDetail>(dbContext), 
        IReceiptDetailRepository
    {
        private readonly MyDbContext _dbContext = dbContext;
    }
}
