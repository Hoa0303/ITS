using Microsoft.AspNetCore.Identity;

namespace ITS_BE.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
    }
}
