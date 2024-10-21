using ITS_BE.Models;

namespace ITS_BE.DTO
{
    public class ReceiptDTO
    {
        public long Id { get; set; }
        public string? Note { get; set; }
        public double Total { get; set; }
    }
}
