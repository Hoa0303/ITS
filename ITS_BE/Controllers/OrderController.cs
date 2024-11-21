using ITS_BE.Enumerations;
using ITS_BE.Request;
using ITS_BE.Services.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ITS_BE.Controllers
{
    [Route("api/orders")]
    [ApiController]
    [Authorize]
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


        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                var role = User.FindAll(ClaimTypes.Role).Select(e => e.Value);
                var admin = role.Contains("Admin");
                if (admin)
                {
                    await _orderService.CancelOrder(id);
                    return Ok();
                }
                else
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (userId == null)
                    {
                        return Unauthorized();
                    }
                    await _orderService.CancelOrder(id, userId);
                    return Ok();
                }
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


        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin,Employee,Statist")]
        public async Task<IActionResult> GetOrderWithStatus(DeliveryStatusEnum status, [FromQuery] PageResquest resquest)
        {
            try
            {
                var res = await _orderService.GetWithOrderStatus(status, resquest);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("user/{status}")]
        public async Task<IActionResult> GetUserOrderWithStatus(DeliveryStatusEnum status, [FromQuery] PageResquest resquest)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Unauthorized();
                }
                var res = await _orderService.GetWithOrderStatus(userId, status, resquest);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("get-all")]
        [Authorize(Roles = "Admin,Employee,Statist")]
        public async Task<IActionResult> GetAll([FromQuery] PageResquest request)
        {
            try
            {
                return Ok(await _orderService.GetAllOrder(request.page, request.pageSize, request.search));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> UpdateStatus(long id)
        {
            try
            {
                await _orderService.UpdateStatusOrder(id);
                return NoContent();
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


        [HttpPut("shipping/{id}")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> ShippingOrder(long id, OrderShippingRequest request)
        {
            try
            {
                await _orderService.ShippingOrder(id, request);
                return NoContent();
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetailOrder(long id)
        {
            try
            {
                var role = User.FindAll(ClaimTypes.Role).Select(e => e.Value);
                var admin = role.Any(e=>e.Equals("Admin") || e.Equals("Statist") || e.Equals("Employee") );
                if (admin)
                {
                    var order = await _orderService.GetOrderDetail(id);
                    return Ok(order);
                }
                else
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (userId == null)
                    {
                        return Unauthorized();
                    }
                    var res = await _orderService.GetOrderDetail(id, userId);
                    return Ok(res);
                }
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


        [HttpPost("review/{id}")]
        public async Task<IActionResult> ReviewOrder(long id, [FromForm] IEnumerable<ReviewRequest> reviews)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Unauthorized();
                }
                await _orderService.Review(id, userId, reviews);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidDataException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut("received/{id}")]
        public async Task<IActionResult> ReceivedOrder(long id)
        {
            try
            {
                await _orderService.Received(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
