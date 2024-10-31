using ITS_BE.Response;

namespace ITS_BE.Services.Statistics
{
    public interface IStatisticService
    {
        Task<int> GetCountUser();
        Task<int> GetCountReceipt();
        Task<int> GetCountOrder();
        Task<int> GetCountProduct();

        Task<StatisticResponse> GetTotalSpendingByYear(int year, int? month);
        Task<StatisticResponse> GetTotalSalesByYear(int year, int? month);
        Task<StatisticDateResponse> GetTotalSpending(DateTime dateFrom, DateTime dateTo);
        Task<StatisticDateResponse> GetTotalSales(DateTime dateFrom, DateTime dateTo);
        Task<RevenueResponse> GetRevenue(int year, int? month);
    }
}
