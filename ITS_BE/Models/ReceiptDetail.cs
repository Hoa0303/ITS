using Microsoft.EntityFrameworkCore;

namespace ITS_BE.Models
{
    public class ReceiptDetail
    {
        public long Id { get; set; }
        public long ReceiptId { get; set; }
        public Receipt Receipt { get; set; }

        public int? ProductId { get; set; }
        [DeleteBehavior(DeleteBehavior.SetNull)]
        public Product Product { get; set; }
        public string ProductName { get; set; }

        public int ColorId { get; set; }
        public Color Color { get; set; }

        public int Quantity { get; set; }
        public double CostPrice { get; set; }
    }
}
