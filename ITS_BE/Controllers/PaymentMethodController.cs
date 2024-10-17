using ITS_BE.ModelView;
using ITS_BE.Services.Payment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITS_BE.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentMethodController(IPaymentService paymentService) : ControllerBase
    {
        private readonly IPaymentService _paymentService = paymentService;

        [HttpGet("vnpay-callback")]
        public async Task<IActionResult> VNPayCallBack([FromQuery] VNPayRequest request)
        {
            try
            {
                await _paymentService.VNPayCallback(request);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}
