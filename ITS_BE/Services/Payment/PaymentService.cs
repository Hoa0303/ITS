using AutoMapper;
using ITS_BE.DTO;
using ITS_BE.Library;
using ITS_BE.ModelView;
using ITS_BE.Repository.OrderRepository;

namespace ITS_BE.Services.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentMethodRepository _paymentmethodRepository;
        private readonly IConfiguration _configuration;
        private readonly IVNPayLibrary _vnPayLibrary;
        private readonly IMapper _mapper;

        public PaymentService(IPaymentMethodRepository paymentmethodRepository, IConfiguration configuration, IVNPayLibrary vnPayLibrary, IMapper mapper)
        {
            _paymentmethodRepository = paymentmethodRepository;
            _configuration = configuration;
            _vnPayLibrary = vnPayLibrary;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PaymentMethodDTO>> GetPaymentMethods()
        {
            var res = await _paymentmethodRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PaymentMethodDTO>>(res);
        }

        public string GetPaymentUrl(VNPayOrder order, string ipAddr, string? locale = null)
        {
            string? vnp_ReturnUrl = _configuration["VNPay:vnp_ReturnUrl"];
            string? vnp_Url = _configuration["VNPay:vnp_Url"];
            string? vnp_TmnCode = _configuration["VNPay:vnp_TmnCode"];
            string? vnp_HashSecret = _configuration["VNpay:vnp_HashSecret"];

            if (string.IsNullOrEmpty(vnp_ReturnUrl) || string.IsNullOrEmpty(vnp_Url)
                || string.IsNullOrEmpty(vnp_TmnCode) || string.IsNullOrEmpty(vnp_HashSecret))
            {
                throw new ArgumentException("Thiếu tham số");
            }

            var vnpay = new VNPay
            {
                vnp_Amount = (order.Amount * 100).ToString(),
                vnp_Command = "pay",
                vnp_CreateDate = order.CreateDate.ToString("yyyyMMddHHmmss"),
                vnp_CurrCode = "VND",
                vnp_ExpireDate = order.CreateDate.AddMinutes(15).ToString("yyyyMMddHHmmss"),
                vnp_IpAddr = ipAddr,
                vnp_Locale = locale ?? "vn",
                vnp_OrderType = "110000",
                vnp_OrderInfo = order.OrderInfor,
                vnp_ReturnUrl = vnp_ReturnUrl,
                vnp_TmnCode = vnp_TmnCode,
                
                vnp_TxnRef = order.OrderId.ToString(),
                vnp_Version = "2.1.0"
            };

            return _vnPayLibrary.createUrl(vnpay, vnp_Url, vnp_HashSecret);
        }

        public async Task<string?> IsActivePaymentMethod(int id)
        {
            var res = await _paymentmethodRepository.SingleOrDefaultAsync(x => x.Id == id && x.IsActive);
            return res?.Name;
        }
    }
}
