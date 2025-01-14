﻿namespace ITS_BE.Request
{
    public class RegisterRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Token { get; set; }
    }

    public class UserRequest
    {
        public string Email { get; set; }
        public string? Fullname { get; set; }
        public string Password { get; set; }
        public string? PhoneNumber { get; set; }
        public IList<string> Roles { get; set; }
    }

    public class UserUpdateRequest
    {
        public string? Fullname { get; set; }
        public string? PhoneNumber { get; set; }
        public IList<string> Roles { get; set; }
    }
}
