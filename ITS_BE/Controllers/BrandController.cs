using ITS_BE.Request;
using ITS_BE.Services.Brands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITS_BE.Controllers
{
    [Route("api/brands")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;
        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllBrand()
        {
            try
            {
                var result = await _brandService.GetAllBrand();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateBrand([FromForm] NameRequest request, [FromForm] IFormFileCollection files)
        {
            try
            {
                var img = files.First();
                var brand = await _brandService.CreateBrand(request.Name, img);
                return Ok(brand);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateBrand(int id, [FromForm] NameRequest request, [FromForm] IFormCollection files)
        {
            try
            {
                var img = files.Files.FirstOrDefault();
                var brand = await _brandService.UpdateBrand(id, request.Name, img);
                return Ok(brand);
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
        public async Task<IActionResult> DeletaBrand(int id)
        {
            try
            {
                await _brandService.DeleteBrand(id);
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
