namespace ITS_BE.Models
{
    public class OrderDetail
    {
        public long Id { get; set; }

        public long OrderId { get; set; }
        public Order Order { get; set; }

        public int? ProductId { get; set; }
        public Product? Product { get; set; }
        public string ProductName { get; set; }

        public string ColorName { get; set; }
        public int ColorId { get; set; }

        public double OriginPrice { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
    }
}
