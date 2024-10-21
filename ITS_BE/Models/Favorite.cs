using Microsoft.EntityFrameworkCore;

namespace ITS_BE.Models
{
    [PrimaryKey(nameof(UserId), nameof(ProductId))]
    public class Favorite : IBaseEntity
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
