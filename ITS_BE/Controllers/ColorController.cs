using ITS_BE.Request;
using ITS_BE.Services.Colors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITS_BE.Controllers
{
    [Route("api/colors")]
    [ApiController]
    public class ColorController(IColorService colorService) : ControllerBase
    {
        private readonly IColorService _colorService = colorService;

        [HttpGet]
        public async Task<IActionResult> GetColors()
        {
            try
            {
                var colors = await _colorService.GetColors();
                return Ok(colors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetColor(int id)
        {
            try
            {
                var colors = await _colorService.GetColorById(id);
                return Ok(colors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
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
