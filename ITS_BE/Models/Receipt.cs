
namespace ITS_BE.Models
{
    public class Receipt : IBaseEntity
    {
        public long Id { get; set; }
        public string? Note { get; set; }
        public string? UserId { get; set; }
        public User? User { get; set; }
        public double Total { get; set; }
        public DateTime EntryDate { get; set; } = DateTime.Now;

        public ICollection<ReceiptDetail> ReceiptDetails { get; } = new HashSet<ReceiptDetail>();
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
