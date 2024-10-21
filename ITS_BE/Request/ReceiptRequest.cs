namespace ITS_BE.Request
{
    public class ReceiptProduct
    {
        public int ProductId { get; set; }
        public int ColorId { get; set; }
        public int Quantity { get; set; }
        public double CostPrice { get; set; }
    }
    public class ReceiptRequest
    {
        public double Total { get; set; }
        public string? Note { get; set; }
        public IEnumerable<ReceiptProduct> ReceiptProducts { get; set; }
    }
}
