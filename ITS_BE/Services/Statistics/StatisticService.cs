
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

        public async Task<StatisticResponse<int>> GetTotalSpending(int year, int? month)
        {
            if (month.HasValue)
            {
                var dailySpending = new StatisticResponse<int>();
                double totalSpendingAmount = 0;

                var lastDayOfMonth = DateTime.DaysInMonth(year, month.Value);

                for (int day = 1; day <= lastDayOfMonth; day++)
                {
                    DateTime startDate = new DateTime(year, month.Value, day);
                    DateTime endDate = startDate.AddDays(1).AddTicks(-1);

                    var receipts = await _receiptRepository.GetAsync(e => e.EntryDate >= startDate && e.EntryDate <= endDate);

                    double totalDailySpending = receipts.Sum(receipt => receipt.Total);

                    dailySpending.StatisticData[day] = totalDailySpending;
                    totalSpendingAmount += totalDailySpending;
                }

                dailySpending.Total = totalSpendingAmount;
                return dailySpending;
            }
            else
            {
                var monthlySpending = new StatisticResponse<int>();
                double totalSpendingAmount = 0;

                for (int monthIndex = 1; monthIndex <= 12; monthIndex++)
                {
                    DateTime startDate = new DateTime(year, monthIndex, 1);
                    DateTime endDate = startDate.AddMonths(1).AddTicks(-1);

                    var receipts = await _receiptRepository.GetAsync(e => e.EntryDate >= startDate && e.EntryDate <= endDate);

                    double totalMonthlySpending = receipts.Sum(receipt => receipt.Total);

                    monthlySpending.StatisticData[monthIndex] = totalMonthlySpending;
                    totalSpendingAmount += totalMonthlySpending;
                }

                monthlySpending.Total = totalSpendingAmount;
                return monthlySpending;
            }
        }

        public async Task<StatisticResponse<int>> GetTotalSales(int year, int? month)
        {
            if (month.HasValue)
            {
                var dailySales = new StatisticResponse<int>();
                double totalSalesAmount = 0;

                var lastDayOfMonth = DateTime.DaysInMonth(year, month.Value);
                for (int day = 1; day <= lastDayOfMonth; day++)
                {
                    DateTime startDate = new DateTime(year, month.Value, day);
                    DateTime endDate = startDate.AddDays(1).AddTicks(-1);

                    var orders = await _orderRepository
                        .GetAsync(e => (e.UpdateAt >= startDate && e.UpdateAt <= endDate) &&
                         (e.OrderStatus == Enum.DeliveryStatusEnum.Received || e.OrderStatus == Enum.DeliveryStatusEnum.Done));

                    double totalDailySales = orders.Sum(order => order.Total);

                    dailySales.StatisticData[day] = totalDailySales;
                    totalSalesAmount += totalDailySales;
                }
                dailySales.Total = totalSalesAmount;
                return dailySales;
            }
            else
            {
                var monthlySales = new StatisticResponse<int>();
                double totalSalesAmount = 0;

                for (int monthIndex = 1; monthIndex <= 12; monthIndex++)
                {
                    DateTime startDate = new DateTime(year, monthIndex, 1);
                    DateTime endDate = startDate.AddMonths(1).AddTicks(-1);

                    var orders = await _orderRepository
                        .GetAsync(e => e.UpdateAt >= startDate && e.UpdateAt <= endDate &&
                        (e.OrderStatus == Enum.DeliveryStatusEnum.Received || e.OrderStatus == Enum.DeliveryStatusEnum.Done));

                    double totalMonthlySales = orders.Sum(order => order.Total);

                    monthlySales.StatisticData[monthIndex] = totalMonthlySales;
                    totalSalesAmount += totalMonthlySales;
                }
                monthlySales.Total = totalSalesAmount;
                return monthlySales;
            }
        }

        public async Task<StatisticResponse<DateTime>> GetTotalSpending(DateTime dateFrom, DateTime dateTo)
        {
            var dailySpending = new StatisticResponse<DateTime>();
            double totalSpendingAmount = 0;

            for (var date = dateFrom.Date; date <= dateTo.Date; date = date.AddDays(1))
            {
                DateTime startDate = date;
                DateTime endDate = startDate.AddDays(1).AddTicks(-1);

                var receipts = await _receiptRepository.GetAsync(e => e.EntryDate >= startDate && e.EntryDate <= endDate);

                double totalDailySpending = receipts.Sum(receipt => receipt.Total);

                dailySpending.StatisticData[date] = totalDailySpending;
                totalSpendingAmount += totalDailySpending;
            }
            dailySpending.Total = totalSpendingAmount;
            return dailySpending;
        }

        public async Task<StatisticResponse<DateTime>> GetTotalSales(DateTime dateFrom, DateTime dateTo)
        {
            var dailySales = new StatisticResponse<DateTime>();
            double totalSalesAmount = 0;

            for (var date = dateFrom.Date; date <= dateTo.Date; date = date.AddDays(1))
            {
                DateTime startDate = date;
                DateTime endDate = startDate.AddDays(1).AddTicks(-1);

                var orders = await _orderRepository
                    .GetAsync(e => (e.UpdateAt >= startDate && e.UpdateAt <= endDate) &&
                    (e.OrderStatus == Enum.DeliveryStatusEnum.Received || e.OrderStatus == Enum.DeliveryStatusEnum.Done));

                double dailyTotal = orders.Sum(receipt => receipt.Total);
                dailySales.StatisticData[date] = dailyTotal;

                totalSalesAmount += dailyTotal;
            }

            dailySales.Total = totalSalesAmount;
            return dailySales;
        }

        public async Task<StatisticResponse<int>> GetRevenue(int year, int? month)
        {
            if (month.HasValue)
            {
                var dailyRevenue = new StatisticResponse<int>();
                double totalRevenueAmount = 0;

                var lastDayOfMonth = DateTime.DaysInMonth(year, month.Value);
                for (int day = 1; day <= lastDayOfMonth; day++)
                {
                    DateTime startDate = new DateTime(year, month.Value, day);
                    DateTime endDate = startDate.AddDays(1).AddTicks(-1);

                    var orders = await _orderRepository
                        .GetAsync(e => (e.UpdateAt >= startDate && e.UpdateAt <= endDate) &&
                         (e.OrderStatus == Enum.DeliveryStatusEnum.Received || e.OrderStatus == Enum.DeliveryStatusEnum.Done));

                    var receipts = await _receiptRepository.GetAsync(e => e.EntryDate >= startDate && e.EntryDate <= endDate);

                    double totalMonthlySpending = orders.Sum(order => order.Total) - receipts.Sum(receipt => receipt.Total);

                    dailyRevenue.StatisticData[day] = totalMonthlySpending;
                    totalRevenueAmount += totalMonthlySpending;
                }

                dailyRevenue.Total = totalRevenueAmount;
                return dailyRevenue;
            }
            else
            {
                var monthlyRevenue = new StatisticResponse<int>();
                double totalRevennue = 0;

                for (int monthIndex = 1; monthIndex <= 12; monthIndex++)
                {
                    DateTime startDate = new DateTime(year, monthIndex, 1);
                    DateTime endDate = startDate.AddMonths(1).AddTicks(-1);

                    var orders = await _orderRepository
                        .GetAsync(e => (e.UpdateAt >= startDate && e.UpdateAt <= endDate) &&
                         (e.OrderStatus == Enum.DeliveryStatusEnum.Received || e.OrderStatus == Enum.DeliveryStatusEnum.Done));

                    var receipts = await _receiptRepository.GetAsync(e => e.EntryDate >= startDate && e.EntryDate <= endDate);

                    double totalMonthlySpending = orders.Sum(order => order.Total) - receipts.Sum(receipt => receipt.Total);

                    monthlyRevenue.StatisticData[monthIndex] = totalMonthlySpending;
                    totalRevennue += totalMonthlySpending;
                }
                monthlyRevenue.Total = totalRevennue;
                return monthlyRevenue;
            }
        }
    }
}
