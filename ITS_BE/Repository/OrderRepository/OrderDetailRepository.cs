using ITS_BE.Data;
using ITS_BE.DTO;
using ITS_BE.Enumerations;
using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;
using ITS_BE.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ITS_BE.Repository.OrderRepository
{
    public class OrderDetailRepository(MyDbContext dbContext) : CommonRepository<OrderDetail>(dbContext), IOrderDetailRepository
    {
        private readonly MyDbContext _dbContext = dbContext;

        public async Task<int> CountSold()
        {
            var currentMonth = DateTime.Today.Month;
            var currentYear = DateTime.Today.Year;

            var totalCount = await _dbContext.OrderDetials
                .Where(od => od.ProductId != null &&
                    od.Order.OrderDate.Month == currentMonth &&
                    od.Order.OrderDate.Year == currentYear &&
                    (od.Order.OrderStatus == DeliveryStatusEnum.Received || od.Order.OrderStatus == DeliveryStatusEnum.Done))
                .GroupBy(od => od.ProductId)
                .CountAsync();

            return totalCount;
        }

        public override async Task<IEnumerable<OrderDetail>> GetAsync(Expression<Func<OrderDetail, bool>> expression)
        {
            return await _dbContext.OrderDetials
                .Where(expression)
                .Include(e => e.Product)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductDTO>> OrderByDescendingBySoldInCurrentMonth(int page, int pageSize)
        {
            var currentMonth = DateTime.Today.Month;
            var currentYear = DateTime.Today.Year;

            var res = await _dbContext.OrderDetials
                .Where(od => od.ProductId != null &&
                    od.Order.OrderDate.Month == currentMonth &&
                    od.Order.OrderDate.Year == currentYear &&
                    (od.Order.OrderStatus == DeliveryStatusEnum.Received || od.Order.OrderStatus == DeliveryStatusEnum.Done))
                .GroupBy(od => od.ProductId)
                .Select(g => new ProductDTO
                {
                    Id = g.Key ?? 0,
                    Name = g.FirstOrDefault() != null ? g.First().ProductName : "",
                    Sold = g.Sum(od => od.Quantity)
                })
            .OrderByDescending(p => p.Sold)
            .Paginate(page, pageSize)
            .ToListAsync();

            return res;
        }
    }
}
