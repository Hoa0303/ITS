using ITS_BE.Models;

namespace ITS_BE.Response
{
    public class UserResponse
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool LockedOut { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
