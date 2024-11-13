using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ITS_BE.Models
{
    public class UserRole : IdentityUserRole<string>
    {
        public virtual User User { get; set; }
        public virtual Role Role { get; set; } 
    }
}
