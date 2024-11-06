using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;
using ITS_BE.Response;
using System.Linq.Expressions;

namespace ITS_BE.Repository.ReceiptRepository
{
    public interface IReceiptRepository : ICommonRepository<Receipt>
    {
        Task<IEnumerable<StatisticData>> GetTotalSpendingByYear(int year, int? month);
        Task<IEnumerable<StatisticDateData>> GetTotalSpending(DateTime dateFrom, DateTime dateTo);

        Task<IEnumerable<StatisticData>> GetTotalProductSpendingByYear(int productId, int year, int? month);
        Task<IEnumerable<StatisticDateData>> GetTotalProductSpending(int productId, DateTime dateFrom, DateTime dateTo);
    }
}
