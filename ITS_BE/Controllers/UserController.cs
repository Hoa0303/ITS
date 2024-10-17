using ITS_BE.DTO;
using ITS_BE.Request;
using ITS_BE.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ITS_BE.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUser([FromQuery] PageResquest resquest)
        {
            try
            {
                return Ok(await _userService.GetAllAsync(resquest.page, resquest.pageSize, resquest.search));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut("lock-out/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> LockOut(string id, [FromBody] InActiveUser request)
        {
            try
            {
                await _userService.LockOut(id, request.EndDate);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("address")]
        public async Task<IActionResult> GetAddress()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Unauthorized();
                }
                var address = await _userService.GetUserAddress(userId);
                return Ok(address);
            }
            catch (AggregateException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut("address")]
        public async Task<IActionResult> UpdateAddress([FromBody] AddressDTO address)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Unauthorized();
                }
                var res = await _userService.UpdateOrAddUserAddress(userId, address);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
