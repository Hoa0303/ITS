using ITS_BE.Request;
using ITS_BE.Services.AuthSevices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ITS_BE.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthsController : ControllerBase
    {
        public IAuthService _authService;
        public AuthsController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var result = await _authService.Login(loginRequest);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            try
            {
                var result = await _authService.Register(registerRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] UserRequest request)
        {
            try
            {
                var res = await _authService.CreateUser(request);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut("update/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateRequest request)
        {
            try
            {
                var res = await _authService.UpdateUser(id, request);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUser(string id)
        {
            try
            {
                var res = await _authService.GetUser(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("send-code")]
        public async Task<IActionResult> CreateToken([FromBody] ResetPassRequest request)
        {
            try
            {
                await _authService.SendCode(request.Email);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("send-code-reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPassRequest request)
        {
            try
            {
                await _authService.SendPasswordResetTokenAsync(request.Email);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("confirm-code")]
        public IActionResult VerifyResetToken([FromBody] VerifyOTPRequest request)
        {
            try
            {
                _authService.VerifyOTP(request.Email, request.Token);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ConfirmResetPassword([FromBody] ConfirmResetPasswordRequset requset)
        {
            var res = await _authService.ResetPasswordAsync(requset.Email, requset.Token, requset.NewPass);
            if (!res)
            {
                return BadRequest("Failed to reset password!");
            }
            return Ok("Password have been reset");
        }
    }
}
