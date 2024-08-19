using ITS_BE.Request;
using ITS_BE.Services.Categories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITS_BE.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController(ICateroryService cateroryService) : ControllerBase
    {
        private readonly ICateroryService _cateroryService = cateroryService;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var categories = await _cateroryService.GetCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("create")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] NameRequest request)
        {
            try
            {
                var result = await _cateroryService.AddCategory(request.Name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("update/{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] NameRequest request)
        {
            try
            {
                var result = await _cateroryService.UpdateCategory(id, request.Name);
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

        [HttpDelete("delete")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _cateroryService.DeleteCategory(id);
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
