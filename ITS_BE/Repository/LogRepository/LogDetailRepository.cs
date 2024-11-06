using ITS_BE.Data;
using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;

namespace ITS_BE.Repository.LogRepository
{
    public class LogDetailRepository(MyDbContext dbContext): CommonRepository<LogDetail>(dbContext), ILogDetailRepository
    {
        private readonly MyDbContext _dbContext = dbContext;
    }
}
