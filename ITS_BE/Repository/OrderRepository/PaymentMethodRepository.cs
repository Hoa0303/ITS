using ITS_BE.Data;
using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;
using Microsoft.EntityFrameworkCore;

namespace ITS_BE.Repository.OrderRepository
{
    public class PaymentMethodRepository(MyDbContext dbContext) : CommonRepository<PaymentMethod>(dbContext), IPaymentMethodRepository
    {
        private readonly MyDbContext _dbContext = dbContext;
        public override async Task<IEnumerable<PaymentMethod>> GetAllAsync()
            => await _dbContext.PaymentMethods.OrderBy(p => p.Id).ToListAsync();
    }
}
