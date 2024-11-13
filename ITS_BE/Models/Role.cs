using Microsoft.AspNetCore.Identity;

namespace ITS_BE.Models
{
    public class Role : IdentityRole
    {
        public virtual ICollection<UserRole> UserRoles { get; }
    }
}
