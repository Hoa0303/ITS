using ITS_BE.DTO;
using ITS_BE.Request;
using ITS_BE.Response;
using Microsoft.AspNetCore.Identity;

namespace ITS_BE.Services.AuthSevices
{
    public interface IAuthService
    {
        Task<JwtResponse> Login(LoginRequest loginRequest);
        Task<UserDTO> Register(RegisterRequest registerRequest);
    }
}
