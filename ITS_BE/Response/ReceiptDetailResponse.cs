namespace ITS_BE.Response
{
    public class ReceiptDetailResponse
    {
        public long Id { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; }

        public int ColorId { get; set; }
        public string ColorName { get; set; }

        public double CostPrice { get; set; }
        public int Quantity { get; set; }
    }
}
