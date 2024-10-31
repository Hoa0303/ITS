
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

        public async Task<StatisticResponse> GetTotalSpendingByYear(int year, int? month)
        {
            var res = await _receiptRepository.GetTotalSpendingByYear(year, month);
            return new StatisticResponse
            {
                StatisticData = res,
                Total = res.Sum(x => x.Statistic)
            };
        }

        public async Task<StatisticResponse> GetTotalSalesByYear(int year, int? month)
        {
            var res = await _orderRepository.GetTotalSalesByYear(year, month);
            return new StatisticResponse
            {
                StatisticData = res,
                Total = res.Sum(e => e.Statistic)
            };
        }

        public async Task<StatisticDateResponse> GetTotalSpending(DateTime dateFrom, DateTime dateTo)
        {
            var res = await _receiptRepository.GetTotalSpending(dateFrom, dateTo);
            return new StatisticDateResponse
            {
                StatisticDateData = res,
                Total = res.Sum(x => x.Statistic)
            };
        }

        public async Task<StatisticDateResponse> GetTotalSales(DateTime dateFrom, DateTime dateTo)
        {
            var res = await _orderRepository.GetTotalSales(dateFrom, dateTo);
            return new StatisticDateResponse
            {
                StatisticDateData = res,
                Total = res.Sum(x => x.Statistic)
            };
        }

        public async Task<RevenueResponse> GetRevenue(int year, int? month)
        {
            var spending = await _receiptRepository.GetTotalSpendingByYear(year, month);
            var sales = await _orderRepository.GetTotalSalesByYear(year, month);

            return new RevenueResponse
            {
                Spending = spending,
                Sales = sales,
                Total = 0
            };
        }
    }
}
