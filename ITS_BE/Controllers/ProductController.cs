using ITS_BE.Request;
using ITS_BE.Services.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITS_BE.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController(IProductService productService) : ControllerBase
    {
        private readonly IProductService _productService = productService;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PageResquest request)
        {
            try
            {
                return Ok(await _productService.GetAllProduct(request.page, request.pageSize, request.search));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("filters")]
        public async Task<IActionResult> GetFilterProducts([FromQuery] Filters filters)
        {
            try
            {
                var result = await _productService.GetFilterProductAsync(filters);
                return Ok(result);
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


        [HttpGet("findversion")]
        public async Task<IActionResult> FindVersionProducts(string? request)
        {
            try
            {
                var res = await _productService.GetAllProductVersionsAsync(request);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _productService.GetProductById(id);
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

        [HttpGet("color/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetColorById(int id)
        {
            try
            {
                var res = await _productService.GetColorById(id);
                return Ok(res);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("name")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Name()
        {
            try
            {
                var res = await _productService.GetNameProduct();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromForm] ProductRequest request, [FromForm] IFormFileCollection form)

        {
            try
            {
                var product = await _productService.CreateProduct(request, form);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromForm] ProductRequest request, [FromForm] IFormCollection form)
        {
            try
            {
                var img = form.Files;
                var result = await _productService.UpdateProduct(id, request, img);
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


        [HttpPut("updateEnable/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEnable(int id, [FromBody] UpdateEnableRequest request)
        {
            try
            {
                var res = await _productService.UpdateEnableRequest(id, request);
                return Ok(res);
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
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _productService.Delete(id);
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


        [HttpGet("review/{id}")]
        public async Task<IActionResult> GetReview(int id, [FromQuery] PageResquest resquest)
        {
            try
            {
                var res = await _productService.GetReview(id, resquest);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
