using ITS_BE.Request;
using ITS_BE.Services.Receipts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ITS_BE.Controllers
{
    [Route("api/receipt")]
    [ApiController]
    public class ReceiptController(IReceiptService receiptService) : ControllerBase
    {
        private readonly IReceiptService _receiptService = receiptService;

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] ReceiptRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Unauthorized();
                }
                var res = await _receiptService.CreateReceipt(userId, request);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery] PageResquest request)
        {
            try
            {
                var res = await _receiptService.GetAll(request.page, request.pageSize, request.search);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var res = await _receiptService.GetbyId(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
