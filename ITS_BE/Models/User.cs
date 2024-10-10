using Microsoft.AspNetCore.Identity;

namespace ITS_BE.Models
{
    public class User : IdentityUser, IBaseEntity
    {
        public string FullName { get; set; }
        public DeliveryAddress? DeliveryAddress { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
