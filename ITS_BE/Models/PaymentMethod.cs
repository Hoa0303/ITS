using System.ComponentModel.DataAnnotations;

namespace ITS_BE.Models
{
    public class PaymentMethod
    {
        public int Id { get; set; }

        [MaxLength(10)]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public ICollection<Order> Orders { get; } = new HashSet<Order>();
    }
}
