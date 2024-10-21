﻿using ITS_BE.Enum;

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

    public class ProductOrderDetails
    {
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public string ColorName { get; set; }
        public double OriginPrice { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }
    }
}