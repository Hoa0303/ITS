using ITS_BE.DTO;
using ITS_BE.Models;
using ITS_BE.Request;
using ITS_BE.Response;

namespace ITS_BE.Services.Orders
{
    public interface IOrderService
    {
        Task<string?> CreateOrder(string userId, OrderRequest request);
        Task CancelOrder(long orderId, string userId);
        Task<PageRespone<OrderDTO>> GetOrderByUserId(string userId, PageResquest resquest);
        Task<OrderDetailResponse> GetOrderDetail(long orderId, string userId);
        Task Review(long orderId, string userId, IEnumerable<ReviewRequest> reviews);


        //Admin
        Task<PageRespone<OrderDTO>> GetAllOrder(int page, int pageSize, string? key);
        Task<OrderDetailResponse> GetOrderDetail(long orderId);
        Task UpdateStatusOrder(long orderId);
        Task ShippingOrder(long orderId, OrderShippingRequest request);
        Task CancelOrder(long orderId);
        Task DeleteOrder(long orderId);
    }
}
