﻿using ITS_BE.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace ITS_BE.Models
{
    public class Order : IBaseEntity
    {
        public long Id { get; set; }
        public double Total { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ReceivedDate {  get; set; }

        [MaxLength(100)]
        public string DeliveryAddress { get; set; }
        public int DistrictId { get; set; } //for GHN
        public string WardCode { get; set; } //for GHN

        [MaxLength(50)]
        public string Receiver { get; set; }
        [MaxLength(15)]
        public string PhoneNumber { get; set; }
        
        public double AmountPaid { get; set; }
        public string? PaymentTranId { get; set; }

        public string? UserId { get; set; }
        public User? User { get; set; }

        public bool Reviewed { get; set; }

        public int? PaymentMethodId { get; set; }
        public string PaymentMethodName { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }

        public string? ShippingCode { get; set; }
        public DateTime? Expected_delivery_time { get; set; }

        public DeliveryStatusEnum? OrderStatus { get; set; } = DeliveryStatusEnum.Processing;

        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public ICollection<OrderDetail> OrderDetials { get; } = new HashSet<OrderDetail>();
    }
}
