
using System.ComponentModel.DataAnnotations;

namespace ITS_BE.Models
{
    public class DeliveryAddress : IBaseEntity
    {
        [Key]
        public string UserId { get; set; }
        public User User { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(15)]
        public string? PhoneNumber { get; set; }

        public int? Province_code { get; set; }
        [MaxLength(50)]
        public string? Province_name { get; set; }

        public int? District_code { get; set; }
        [MaxLength(50)]
        public string? District_name { get; set; }

        public int? Ward_code { get; set; }
        [MaxLength(50)]
        public string? Ward_name { get; set; }

        [MaxLength(100)]
        public string? Detail { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
