using ITS_BE.Response;

namespace ITS_BE.Services.Statistics
{
    public interface IStatisticService
    {
        Task<int> GetCountUser();
        Task<int> GetCountReceipt();
        Task<int> GetCountOrder();
        Task<int> GetCountProduct();
        Task<StatisticResponse<int>> GetTotalSpending(int year, int? month);
        Task<StatisticResponse<int>> GetTotalSales(int year, int? month);
        Task<StatisticResponse<DateTime>> GetTotalSpending(DateTime dateFrom, DateTime dateTo);
        Task<StatisticResponse<DateTime>> GetTotalSales(DateTime dateFrom, DateTime dateTo);
        Task<StatisticResponse<int>> GetRevenue(int year, int? month);
    }
}
