using ITS_BE.DTO;
using ITS_BE.Request;
using ITS_BE.Response;

namespace ITS_BE.Services.Orders
{
    public interface IOrderService
    {
        Task<string?> CreateOrder(string userId, OrderRequest request);
        Task<PageRespone<OrderDTO>> GetOrderByUserId(string userId, PageResquest resquest);
    }
}
