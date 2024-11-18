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
        Task<UserDTO> CreateUser(UserRequest request);
        Task<UserDTO> UpdateUser(string userId, UserUpdateRequest request);
        Task<UserDTO> GetUser(string userId);
        Task<bool> SendCode(string email);
        Task<bool> SendPasswordResetTokenAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
        bool VerifyOTP(string email, string token);
    }
}
