using ITS_BE.Request;
using ITS_BE.Services.Products;
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

        [HttpGet("get/{id}")]
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

        [HttpPost("create")]
        //public async Task<IActionResult> Create([FromForm] ProductRequest request, [FromForm] IFormCollection form)
        public async Task<IActionResult> Create([FromForm] ProductRequest request, [FromForm] IFormFileCollection form) //Test trên swagger

        {
            try
            {
                //var image = form.Files;
                //var image = form; //Test trên swagger
                var product = await _productService.CreateProduct(request, form);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] ProductRequest request, IFormFileCollection form)
        {
            try
            {
                var result = await _productService.UpdateProduct(id, request, form);
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


    }
}
