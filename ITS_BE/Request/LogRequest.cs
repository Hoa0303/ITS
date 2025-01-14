﻿namespace ITS_BE.Request
{
    public class LogRequest
    {
        public string? UserId { get; set; }
        public double Total { get; set; }
        public string? Note { get; set; }
        public DateTime EntryDate { get; set; }
        public long ReceiptId { get; set; }
        public IEnumerable<LogProduct> logProducts { get; set; }
    }

    public class LogProduct
    {
        public string ProductName { get; set; }
        public string ColorName { get; set; }
        public double CostPrice { get; set; }
        public int Quantity { get; set; }
    }
}
