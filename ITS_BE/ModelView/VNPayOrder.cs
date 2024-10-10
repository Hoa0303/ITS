namespace ITS_BE.ModelView
{
    public class VNPayOrder
    {
        public long OrderId { get; set; }
        public double Amount { get; set; }
        //public string CodeType { get; set; }
        public string Status { get; set; }
        public string OrderInfor { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
