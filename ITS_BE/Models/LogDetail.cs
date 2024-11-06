namespace ITS_BE.Models
{
    public class LogDetail
    {
        public int Id { get; set; }
        public long LogId { get; set; }
        public Log Log { get; set; }
        public string ProductName { get; set; }
        public string ColorName { get; set; }
        public int Quantity { get; set; }
        public double CostPrice { get; set; }
    }
}
