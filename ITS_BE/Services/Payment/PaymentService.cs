using AutoMapper;
using ITS_BE.Constants;
using ITS_BE.DTO;
using ITS_BE.Enumerations;
using ITS_BE.Library;
using ITS_BE.ModelView;
using ITS_BE.Repository.OrderRepository;
using ITS_BE.Services.Caching;
using ITS_BE.Services.Orders;

namespace ITS_BE.Services.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentMethodRepository _paymentmethodRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IConfiguration _configuration;
        private readonly IVNPayLibrary _vnPayLibrary;
        private readonly ICachingService _cachingService;
        private readonly IMapper _mapper;

        public PaymentService(IPaymentMethodRepository paymentmethodRepository, IConfiguration configuration,
            IVNPayLibrary vnPayLibrary, IMapper mapper, ICachingService cachingService,
             IOrderRepository orderRepository)
        {
            _paymentmethodRepository = paymentmethodRepository;
            _orderRepository = orderRepository;
            _configuration = configuration;
            _cachingService = cachingService;
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

        public async Task VNPayCallback(VNPayRequest request)
        {
            string vnp_HashSecret = _configuration["VNpay:vnp_HashSecret"] ?? "";

            long orderId = Convert.ToInt32(request.vnp_TxnRef);
            long vnp_Amount = Convert.ToInt64(request.vnp_Amount) / 100;

            string vnp_ResponseCode = request.vnp_ResponseCode;
            string vnp_TransactionStatus = request.vnp_TransactionStatus;

            string vnp_SecureHash = request.vnp_SecureHash;

            bool checkSignature = _vnPayLibrary.ValidateSignature(request, vnp_SecureHash, vnp_HashSecret);
            if (checkSignature)
            {
                var order = await _orderRepository.FindAsync(orderId)
                    ?? throw new ArgumentException($"Order {orderId}" + ErrorMessage.NOT_FOUND);

                if (order.Total == vnp_Amount)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        order.PaymentTranId = request.vnp_TransactionNo;
                        order.AmountPaid = vnp_Amount;
                        order.OrderStatus = DeliveryStatusEnum.Confirmed;
                        await _orderRepository.UpdateAsync(order);
                        _cachingService.Remove("Order " + orderId);
                    }
                }
                else throw new Exception(ErrorMessage.PAYMENT_FAILED);
            }
            else throw new ArgumentException("Số tiền " + ErrorMessage.INVALID);
        }
    }
}
