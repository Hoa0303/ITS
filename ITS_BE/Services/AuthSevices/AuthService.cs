using AutoMapper;
using ITS_BE.Constants;
using ITS_BE.DTO;
using ITS_BE.Models;
using ITS_BE.Repository.TransactionRepository;
using ITS_BE.Request;
using ITS_BE.Response;
using ITS_BE.Services.Caching;
using ITS_BE.Services.SendEmail;
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
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;
        private readonly ISendEmailService _sendEmailService;
        private readonly ICachingService _cachingService;

        public AuthService(SignInManager<User> signInManager, UserManager<User> userManager,
            IConfiguration config, IMapper mapper, ICachingService caching,
            ISendEmailService sendEmail, ITransactionRepository transactionRepository)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _transactionRepository = transactionRepository;
            _config = config;
            _mapper = mapper;
            _cachingService = caching;
            _sendEmailService = sendEmail;
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
                    var roles = await _userManager.GetRolesAsync(user);

                    return new JwtResponse
                    {
                        Jwt = token,
                        Name = user.FullName,
                        PhoneNumber = user.PhoneNumber,
                        Roles = roles,
                    };
                }
                throw new Exception("User not found");
            }
            else throw new ArgumentException("Password incorrect");
        }

        public async Task<IdentityResult> Register(RegisterRequest registerRequest)
        {
            var isTokenVaild = VerifyOTP(registerRequest.Email, registerRequest.Token);

            if (isTokenVaild)
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
                    throw new Exception(ErrorMessage.INVALID);
                }
                await _userManager.AddToRoleAsync(user, "User");
                return IdentityResult.Success;
            }
            else throw new Exception("Invalid reset token");
            //var result = await _userManager.CreateAsync(user, registerRequest.Password);
            //if (!result.Succeeded)
            //{
            //    throw new Exception(string.Join(";", result.Errors.Select(e => e.Description)));
            //}
            //return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> CreateUser(UserRequest request)
        {
            using (await _transactionRepository.BeginTransactionAsync())
            {
                try
                {
                    var user = new User()
                    {
                        Email = request.Email,
                        NormalizedEmail = request.Email.ToUpper(),
                        UserName = request.Email,
                        NormalizedUserName = request.Email.ToUpper(),
                        PhoneNumber = request.PhoneNumber,
                        FullName = request.Fullname,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                    };
                    var res = await _userManager.CreateAsync(user, request.Password);
                    if (!res.Succeeded)
                    {
                        throw new Exception(ErrorMessage.INVALID);
                    }
                    var roleRes = await _userManager.AddToRolesAsync(user, request.Roles);
                    if (!roleRes.Succeeded)
                    {
                        throw new Exception(ErrorMessage.INVALID);
                    }
                    await _transactionRepository.CommitTransactionAsync();
                    return new UserDTO
                    {
                        Id = user.Id,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        FullName = user.FullName,
                    };
                }
                catch (Exception ex)
                {
                    await _transactionRepository.RollbackTransactionAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<UserDTO> UpdateUser(string userId, UserUpdateRequest request)
        {
            using (await _transactionRepository.BeginTransactionAsync())
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                    {
                        throw new Exception(ErrorMessage.NOT_FOUND);
                    }
                    if (!string.IsNullOrEmpty(request.Fullname))
                    {
                        user.FullName = request.Fullname;
                    }
                    user.PhoneNumber = request.PhoneNumber;

                    var updateRes = await _userManager.UpdateAsync(user);
                    if (!updateRes.Succeeded)
                    {
                        throw new Exception(ErrorMessage.INVALID);
                    }

                    var currentRoles = await _userManager.GetRolesAsync(user);
                    var removeRole = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeRole.Succeeded)
                    {
                        throw new Exception(ErrorMessage.INVALID);
                    }

                    var addRoles = await _userManager.AddToRolesAsync(user, request.Roles);
                    if (!addRoles.Succeeded)
                    {
                        throw new Exception(ErrorMessage.INVALID);
                    }
                    await _transactionRepository.CommitTransactionAsync();
                    return new UserDTO
                    {
                        Id = user.Id,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        FullName = user.FullName,
                    };
                }
                catch (Exception ex)
                {
                    await _transactionRepository.RollbackTransactionAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<UserDTO> GetUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new Exception(ErrorMessage.NOT_FOUND);

            var roles = await _userManager.GetRolesAsync(user);

            return new UserDTO
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Roles = roles,
                PhoneNumber = user.PhoneNumber
            };
        }

        public async Task<bool> SendCode(string email)
        {
            var token = new Random().Next(100000, 999999).ToString();
            _cachingService.Set(email, token);

            var message = $"Your verification code is: {token}";
            await _sendEmailService.SendEmailAsync(email, "Mã xác nhận tài khoản ITS", message);

            return true;
        }

        public async Task<bool> SendPasswordResetTokenAsync(string email)
        {
            var user = _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return false;
            }

            var token = new Random().Next(100000, 999999).ToString();
            _cachingService.Set(email, token);

            var message = $"Your password reset code is: {token}";
            await _sendEmailService.SendEmailAsync(email, "Reset password", message);

            return true;
        }

        public bool VerifyOTP(string email, string token)
        {
            var cachedToken = _cachingService.Get<string>(email);
            if (cachedToken == null || cachedToken != token) return false;
            else return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var isTokenValid = VerifyOTP(email, token);
            if (!isTokenValid) return false;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return false;
            }

            var t = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, t, newPassword);
            await _userManager.UpdateSecurityStampAsync(user);
            _cachingService.Remove(email);
            return result.Succeeded;
        }
    }
}
