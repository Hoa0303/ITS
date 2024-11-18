using ITS_BE.Services.Statistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITS_BE.Controllers
{
    [Route("api/statistic")]
    [ApiController]
    public class StatisticController(IStatisticService statisticService) : ControllerBase
    {
        private readonly IStatisticService _statisticService = statisticService;

        [HttpGet("count-receipt")]
        [Authorize(Roles = "Admin,Statist")]
        public async Task<IActionResult> GetCountReceipt()
        {
            try
            {
                var res = await _statisticService.GetCountReceipt();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("count-order")]
        [Authorize(Roles = "Admin,Statist")]
        public async Task<IActionResult> GetCountOrder()
        {
            try
            {
                var res = await _statisticService.GetCountOrder();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("count-product")]
        [Authorize(Roles = "Admin,Statist")]
        public async Task<IActionResult> GetCountProduct()
        {
            try
            {
                var res = await _statisticService.GetCountProduct();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("count-user")]
        [Authorize(Roles = "Admin,Statist")]
        public async Task<IActionResult> GetCountUser()
        {
            try
            {
                var res = await _statisticService.GetCountUser();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("total-revenue-by-year")]
        [Authorize(Roles = "Admin,Statist")]
        public async Task<IActionResult> GetRevenueByYear(int year, int? month)
        {
            try
            {
                var res = await _statisticService.GetRevenueByYear(year, month);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("total-revenue")]
        [Authorize(Roles = "Admin,Statist")]
        public async Task<IActionResult> GetRevenue(DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                var res = await _statisticService.GetRevenue(dateFrom, dateTo);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("total-product-revenue-by-year")]
        [Authorize(Roles = "Admin,Statist")]
        public async Task<IActionResult> GetProductRevenueByYear(int year, int? month, int productId)
        {
            try
            {
                var res = await _statisticService.GetProductRevenueByYear(productId, year, month);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("total-product-revenue")]
        [Authorize(Roles = "Admin,Statist")]
        public async Task<IActionResult> GetProductRevenue(DateTime dateFrom, DateTime dateTo, int productId)
        {
            try
            {
                var res = await _statisticService.GetProductRevenue(productId, dateFrom, dateTo);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
