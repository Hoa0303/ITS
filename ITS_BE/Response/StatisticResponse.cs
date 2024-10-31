namespace ITS_BE.Response
{
    public class StatisticResponse
    {
        public IEnumerable<StatisticData> StatisticData { get; set; }
        public double Total { get; set; }
    }

    public class StatisticDateResponse
    {
        public IEnumerable<StatisticDateData> StatisticDateData { get; set; }
        public double Total { get; set; }
    }

    public class RevenueResponse
    {
        public IEnumerable<StatisticData> Spending { get; set; }
        public IEnumerable<StatisticData> Sales { get; set; }
        public double Total { get; set; }
    }

    public class StatisticData
    {
        public int Time { get; set; }
        public double Statistic { get; set; }
    }

    public class StatisticDateData
    {
        public DateTime Time { get; set; }
        public double Statistic { get; set; }
    }



}
