using ITS_BE.DTO;

namespace ITS_BE.Response
{
    public class OrderDetailResponse
    {
        public long Id { get; set; }
        public IEnumerable<ProductOrderDetails> ProductOrderDetails { get; set; }
        public double Total { get; set; }
        public DateTime OrderDate { get; set; }
        public string DeliveryAddress { get; set; }
        public string Receiver { get; set; }
        public string PhoneNumber { get; set; }
        public double AmountPaid { get; set; }
    }
}
