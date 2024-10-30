namespace ITS_BE.Response
{
    public class StatisticResponse<TKey>
    {
        public Dictionary<TKey, double> StatisticData { get; set; }
        public double Total { get; set; }
        public StatisticResponse()
        {
            StatisticData = new Dictionary<TKey, double>();
        }
    }
}
