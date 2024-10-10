using ITS_BE.Request;
using ITS_BE.Services.Orders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ITS_BE.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController(IOrderService orderService) : ControllerBase
    {
        private readonly IOrderService _orderService = orderService;

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Unauthorized();
                }
                var order = await _orderService.CreateOrder(userId, request);
                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetOrderByUserId([FromQuery] PageResquest resquest)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Unauthorized();
                }
                var orders = await _orderService.GetOrderByUserId(userId, resquest);
                return Ok(orders);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
