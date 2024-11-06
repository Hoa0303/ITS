using ITS_BE.Models;
using ITS_BE.Response;

namespace ITS_BE.Services.Statistics
{
    public interface IStatisticService
    {
        Task<int> GetCountUser();
        Task<int> GetCountReceipt();
        Task<int> GetCountOrder();
        Task<int> GetCountProduct();
        Task<RevenueResponse> GetRevenueByYear(int year, int? month);
        Task<RevenueDateResponse> GetRevenue(DateTime dateFrom, DateTime dateTo);

        Task<RevenueResponse> GetProductRevenueByYear(int productId, int year, int? month);
        Task<RevenueDateResponse> GetProductRevenue(int productId, DateTime dateFrom, DateTime dateTo);
    }
}
