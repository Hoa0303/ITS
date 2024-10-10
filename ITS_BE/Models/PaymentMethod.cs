namespace ITS_BE.Models
{
    public class PaymentMethod
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public ICollection<Order> Orders { get; } = new HashSet<Order>();
    }
}
