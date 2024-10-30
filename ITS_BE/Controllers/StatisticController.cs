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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

        [HttpGet("total-spending-by-year")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTotalSpendingByYear(int year, int? month)
        {
            try
            {
                var result = await _statisticService.GetTotalSpending(year, month);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("total-spending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTotalSpending(DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                var res = await _statisticService.GetTotalSpending(dateFrom, dateTo);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("total-sales-by-year")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTotalSalesByYear(int year, int? month)
        {
            try
            {
                var result = await _statisticService.GetTotalSales(year, month);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("total-sales")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTotalSales(DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                var res = await _statisticService.GetTotalSales(dateFrom, dateTo);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("total-revenue")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTotalRevenue(int year, int? month)
        {
            try
            {
                var res = await _statisticService.GetRevenue(year, month);
                return Ok(res);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
