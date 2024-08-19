using AutoMapper;
using ITS_BE.DTO;
using ITS_BE.Models;
using ITS_BE.Request;
using ITS_BE.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ITS_BE.Services.AuthSevices
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public AuthService(SignInManager<User> signInManager, UserManager<User> userManager, IConfiguration config, IMapper mapper)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
            _mapper = mapper;
        }
        private async Task<string> CreateJwt(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Name, user.FullName ?? "")
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["JWT:Key"] ?? throw new Exception()));

            var jwToken = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(12),
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
                );
            return new JwtSecurityTokenHandler().WriteToken(jwToken);
        }

        public async Task<JwtResponse> Login(LoginRequest loginRequest)
        {
            var result = await _signInManager.PasswordSignInAsync(loginRequest.Username, loginRequest.Password, false, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(loginRequest.Username);
                if (user != null)
                {
                    var token = await CreateJwt(user);
                    return new JwtResponse
                    {
                        Jwt = token,
                        name = user.FullName,
                    };
                }
                throw new Exception("User not found");
            }
            else throw new ArgumentException("User not found");
        }

        public async Task<UserDTO> Register(RegisterRequest registerRequest)
        {
            var user = new User()
            {
                Email = registerRequest.Email,
                NormalizedEmail = registerRequest.Email,
                UserName = registerRequest.Email,
                NormalizedUserName = registerRequest.Email,
                PhoneNumber = registerRequest.PhoneNumber,
                FullName = registerRequest.Name,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
            };
            var result = await _userManager.CreateAsync(user, registerRequest.Password);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(";", result.Errors.Select(e => e.Description)));
            }
            return _mapper.Map<UserDTO>(user);
        }
    }
}
