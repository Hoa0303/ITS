
using ITS_BE.Models;
using ITS_BE.Repository.OrderRepository;
using ITS_BE.Repository.ProductRepository;
using ITS_BE.Repository.ReceiptRepository;
using ITS_BE.Repository.UserRepository;
using ITS_BE.Response;
using Microsoft.AspNetCore.Identity;

namespace ITS_BE.Services.Statistics
{
    public class StatisticService : IStatisticService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IReceiptRepository _receiptRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public StatisticService(IReceiptRepository receiptRepository, IOrderRepository orderRepository,
            IProductRepository productRepository, IUserRepository userRepository, UserManager<User> userManager)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _receiptRepository = receiptRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<int> GetCountUser()
        {
            var total = await _userRepository.CountAsync();
            return total;
        }

        public async Task<int> GetCountOrder()
        {
            var total = await _orderRepository
                .CountAsync(e => e.OrderStatus == Enum.DeliveryStatusEnum.Done || e.OrderStatus == Enum.DeliveryStatusEnum.Received);
            return total;
        }

        public async Task<int> GetCountProduct()
        {
            var total = await _productRepository.CountAsync(e => e.Enable);
            return total;
        }

        public async Task<int> GetCountReceipt()
        {
            var total = await _receiptRepository.CountAsync();
            return total;
        }

        public async Task<RevenueResponse> GetRevenueByYear(int year, int? month)
        {
            var spending = await _receiptRepository.GetTotalSpendingByYear(year, month);
            var sales = await _orderRepository.GetTotalSalesByYear(year, month);

            var SpendingStatistic = new StatisticResponse
            {
                StatisticData = spending,
                Total = spending.Sum(x => x.Statistic)
            };

            var SalesStatistic = new StatisticResponse
            {
                StatisticData = sales,
                Total = sales.Sum(x => x.Statistic)
            };

            return new RevenueResponse
            {
                Spending = SpendingStatistic,
                Sales = SalesStatistic,
                Total = SalesStatistic.Total - SpendingStatistic.Total
            };
        }

        public async Task<RevenueDateResponse> GetRevenue(DateTime dateFrom, DateTime dateTo)
        {
            var spending = await _receiptRepository.GetTotalSpending(dateFrom, dateTo);
            var sales = await _orderRepository.GetTotalSales(dateFrom, dateTo);

            var spendingData = new StatisticDateResponse
            {
                StatisticDateData = spending,
                Total = spending.Sum(e => e.Statistic)
            };

            var salesData = new StatisticDateResponse
            {
                StatisticDateData = sales,
                Total = sales.Sum(e => e.Statistic)
            };

            return new RevenueDateResponse
            {
                Spending = spendingData,
                Sales = salesData,
                Total = salesData.Total - spendingData.Total
            };
        }

        public async Task<RevenueResponse> GetProductRevenueByYear(int productId, int year, int? month)
        {
            var productSpending = await _receiptRepository.GetTotalProductSpendingByYear(productId, year, month);
            var productSales = await _orderRepository.GetTotalProductSalesByYear(productId, year, month);

            var spendingData = new StatisticResponse
            {
                StatisticData = productSpending,
                Total = productSpending.Sum(x => x.Statistic)
            };

            var salesData = new StatisticResponse
            {
                StatisticData = productSales,
                Total = productSales.Sum(x => x.Statistic)
            };

            return new RevenueResponse
            {
                Spending = spendingData,
                Sales = salesData,
                Total = salesData.Total - spendingData.Total
            };
        }

        public async Task<RevenueDateResponse> GetProductRevenue(int productId, DateTime dateFrom, DateTime dateTo)
        {
            var productSpending = await _receiptRepository.GetTotalProductSpending(productId, dateFrom, dateTo);
            var productSales = await _orderRepository.GetTotalProductSales(productId, dateFrom, dateTo);

            var spendingData = new StatisticDateResponse
            {
                StatisticDateData = productSpending,
                Total = productSpending.Sum(e => e.Statistic)
            };

            var salesData = new StatisticDateResponse
            {
                StatisticDateData = productSales,
                Total = productSales.Sum(e => e.Statistic)
            };

            return new RevenueDateResponse
            {
                Spending = spendingData,
                Sales = salesData,
                Total = salesData.Total - spendingData.Total
            };
        }
    }
}
