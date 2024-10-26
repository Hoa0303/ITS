
using System.ComponentModel.DataAnnotations;

namespace ITS_BE.Models
{
    public class Review : IBaseEntity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [MaxLength(200)]
        public string? Description { get; set; }

        [Range(1, 5)]
        public int Start { get; set; }

        public int? ProductId { get; set; }
        public Product? Product { get; set; }

        public string? UserId { get; set; }
        public User? User { get; set; }

        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
