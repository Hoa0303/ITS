using System.ComponentModel.DataAnnotations;

namespace ITS_BE.Models
{
    public class Log : IBaseEntity
    {
        public long Id { get; set; }
        [MaxLength(100)]
        public string? Note { get; set; }
        public double Total { get; set; }
        public DateTime EntryDate { get; set; }
        public string? UserId { get; set; }
        public User User { get; set; }

        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public long ReceiptId { get; set; }

        public ICollection<LogDetail> LogDetails { get; } = new HashSet<LogDetail>();
    }
}
