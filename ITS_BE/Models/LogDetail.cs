using System.ComponentModel.DataAnnotations;

namespace ITS_BE.Models
{
    public class LogDetail
    {
        public int Id { get; set; }
        public long LogId { get; set; }
        public Log Log { get; set; }
        [MaxLength(50)]
        public string ProductName { get; set; }
        [MaxLength(10)]
        public string ColorName { get; set; }
        public int Quantity { get; set; }
        public double CostPrice { get; set; }
    }
}
