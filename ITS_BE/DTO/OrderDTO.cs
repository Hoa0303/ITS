using ITS_BE.Enum;

namespace ITS_BE.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public double Total { get; set; }
        public double AmountPaid { get; set; }
        public DateTime OrderDate { get; set; }
        public string PaymentMethod { get; set; }
        public DeliveryStatusEnum OrderStatus { get; set; }

        public string? PayBackUrl { get; set; }
    }
}
