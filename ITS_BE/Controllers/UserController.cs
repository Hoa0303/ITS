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

        [HttpGet("employee")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetEmployee([FromQuery] PageResquest resquest)
        {
            try
            {
                return Ok(await _userService.GetEmployee(resquest.page, resquest.pageSize, resquest.search));
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


        [HttpGet("infor")]
        public async Task<IActionResult> GetInforUser()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Unauthorized();
                }
                var res = await _userService.GetUserInfo(userId);
                return Ok(res);
            }
            catch (AggregateException ex)
            {
                return NotFound(ex.Message);
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


        [HttpPut("infor")]
        public async Task<IActionResult> UpdateInfo([FromBody] UserInfoRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Unauthorized();
                }
                var res = await _userService.UpdateUserInfo(userId, request);
                return Ok(res);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("avatar")]
        public async Task<IActionResult> UpdateImage([FromForm] IFormFileCollection files)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Unauthorized();
                }
                var img = files.First();
                var res = await _userService.UpdateImage(userId, img);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("avatar")]
        public async Task<IActionResult> GetImage()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Unauthorized();
                }
                var res = await _userService.GetImage(userId);
                return Ok(res);
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


        [HttpPost("favorite")]
        public async Task<IActionResult> AddFavorite([FromBody] IdRequest<int> request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Unauthorized();
                }
                await _userService.AddFavorite(userId, request.Id);
                return Created();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("favorite")]
        public async Task<IActionResult> GetFavorite()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Unauthorized();
                }
                var favorites = await _userService.GetFavorites(userId);
                return Ok(favorites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("favorite/products")]
        public async Task<IActionResult> ProductFavorite([FromQuery] PageResquest resquest)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Unauthorized();
                }
                var products = await _userService.GetProductFavorites(userId, resquest);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete("favorite/{id}")]
        public async Task<IActionResult> DeleteFavorite(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Unauthorized();
                }
                await _userService.DeleteFavorite(userId, id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
