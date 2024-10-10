using ITS_BE.Data;
using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;

namespace ITS_BE.Repository.OrderRepository
{
    public class OrderRepository(MyDbContext dbContext) : CommonRepository<Order>(dbContext), IOrderRepository
    {
        private readonly MyDbContext _dbContext = dbContext;
    }
}