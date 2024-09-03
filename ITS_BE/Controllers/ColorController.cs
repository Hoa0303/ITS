using ITS_BE.Request;
using ITS_BE.Services.Colors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITS_BE.Controllers
{
    [Route("api/color")]
    [ApiController]
    public class ColorController : ControllerBase
    {
        private readonly IColorService _colorService;

        public ColorController(IColorService colorService) => _colorService = colorService;

        [HttpGet]
        public async Task<IActionResult> GetColor()
        {
            try
            {
                var color = await _colorService.GetColors();
                return Ok(color);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateColor([FromBody] NameRequest request)
        {
            try
            {
                var color = await _colorService.AddColorsAsync(request.Name);
                return Ok(color);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateColor(int id, [FromBody] NameRequest request)
        {
            try
            {
                var result = await _colorService.UpdateColorsAsync(id, request.Name);
                return Ok(result);
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

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteColor(int id)
        {
            try
            {
                await _colorService.DeleteColorsAsync(id);
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
    }
}
