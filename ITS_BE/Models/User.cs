using Microsoft.AspNetCore.Identity;

namespace ITS_BE.Models
{
    public class User : IdentityUser, IBaseEntity
    {
        public string FullName { get; set; }
        public DeliveryAddress? DeliveryAddress { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public ICollection<Order> Orders { get; } = new HashSet<Order>();
        public ICollection<Receipt> Receipts { get; } = new HashSet<Receipt>();
        public ICollection<Favorite> Favorites { get; } = new HashSet<Favorite>();
        public ICollection<Review> Reviews { get; } = new HashSet<Review>();
    }
}
