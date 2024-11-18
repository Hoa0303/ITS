using ITS_BE.Request;
using ITS_BE.Services.History;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITS_BE.Controllers
{
    [Route("api/log")]
    [ApiController]
    [Authorize]
    public class LogController(ILogService logService) : ControllerBase
    {
        private readonly ILogService _logService = logService;

        [HttpGet]
        [Authorize(Roles = "Admin,Stocker")]
        public async Task<IActionResult> GetAll([FromQuery] PageResquest request)
        {
            try
            {
                var res = await _logService.GetAll(request.page, request.pageSize, request.search);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Stocker")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var res = await _logService.GetById(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
