using ITS_BE.DTO;
using ITS_BE.Request;
using ITS_BE.Response;
using Microsoft.AspNetCore.Identity;

namespace ITS_BE.Services.AuthSevices
{
    public interface IAuthService
    {
        Task<JwtResponse> Login(LoginRequest loginRequest);
        Task<IdentityResult> Register(RegisterRequest registerRequest);
        Task<bool> SendCode(string email);
        Task<bool> SendPasswordResetTokenAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
        bool VerifyOTP(string email, string token);
    }
}
