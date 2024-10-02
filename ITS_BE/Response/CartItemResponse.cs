namespace ITS_BE.Response
{
    public class CartItemResponse
    {
        public string Id { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public string CategoryName { get; set; }
        public int Rom { get; set; }

        public double OriginPrice { get; set; }
        public int Discount { get; set; }
        public double Price => OriginPrice - OriginPrice * (Discount / 100.0);

        public int Quantity { get; set; }

        public int ColorId { get; set; }
        public string ColorName { get; set; }

        public int InStock { get; set; }

        public string ImageUrl { get; set; }
    }
}
