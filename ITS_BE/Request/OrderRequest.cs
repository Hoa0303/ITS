using ITS_BE.Enum;
using System.ComponentModel.DataAnnotations;

namespace ITS_BE.Request
{
    public class OrderRequest
    {
        public double Total { get; set; }

        [MaxLength(80, ErrorMessage = "Thông tin quá dài")]
        public string Receiver { get; set; }

        [MaxLength(150, ErrorMessage = "Thông tin địa chỉ quá dài")]
        public string DeliveryAddress { get; set; }
        public int DistrictId { get; set; } //for GHN
        public string WardCode { get; set; } //for GHN
        public string PhoneNumber { get; set; }
        public IEnumerable<string> CartIds { get; set; }
        public int PaymentMethodId { get; set; }
        public string? UserIp { get; set; }
    }

    public class OrderShippingRequest
    {
        public int Weight { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int length { get; set; }
        public DeliveryRequestEnum DeliveryRequestEnum { get; set;}
    }
}
