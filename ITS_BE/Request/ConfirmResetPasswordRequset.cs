namespace ITS_BE.Request
{
    public class ConfirmResetPasswordRequset
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPass { get; set; }
    }
}
