using ITS_BE.DTO;
using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;

namespace ITS_BE.Repository.OrderRepository
{
    public interface IOrderDetailRepository : ICommonRepository<OrderDetail>
    {
        Task<IEnumerable<ProductDTO>> OrderByDescendingBySoldInCurrentMonth(int page, int pageSize);
        Task<int> CountSold();

    }
}