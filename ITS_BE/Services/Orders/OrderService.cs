using AutoMapper;
using ITS_BE.Constants;
using ITS_BE.DTO;
using ITS_BE.Enum;
using ITS_BE.Library;
using ITS_BE.Models;
using ITS_BE.ModelView;
using ITS_BE.Repository.CartItemRepository;
using ITS_BE.Repository.OrderRepository;
using ITS_BE.Repository.ProductColorRepository;
using ITS_BE.Repository.ProductRepository;
using ITS_BE.Request;
using ITS_BE.Response;
using ITS_BE.Services.Caching;
using ITS_BE.Services.Payment;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace ITS_BE.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductColorRepository _productColorRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IConfiguration _configuration;
        private readonly IPaymentService _paymentService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ICachingService _cachingService;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository,
            IProductRepository productRepository, IProductColorRepository productColorRepository,
            ICartItemRepository cartItemRepository, IPaymentMethodRepository paymentMethodRepository,
            IOrderDetailRepository orderDetailRepository, IPaymentService paymentService,
            IServiceScopeFactory serviceScopeFactory, ICachingService cachingService,
            IConfiguration configuration, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _productColorRepository = productColorRepository;
            _cartItemRepository = cartItemRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _orderDetailRepository = orderDetailRepository;
            _configuration = configuration;
            _paymentService = paymentService;
            _cachingService = cachingService;
            _serviceScopeFactory = serviceScopeFactory;
            _mapper = mapper;
        }
        struct OrderCache
        {
            public string Url { get; set; }
            public long OrderId { get; set; }
            public string? vnp_IpAddr { get; set; }
            public string? vnp_CreateDate { get; set; }
            public string? vnp_OrderInfo { get; set; }
        }

        public async Task<string?> CreateOrder(string userId, OrderRequest request)
        {
            try
            {
                var now = DateTime.Now;
                var order = new Order
                {
                    UserId = userId,
                    DeliveryAddress = request.DeliveryAddress,
                    DistrictId = request.DistrictId,
                    WardCode = request.WardCode,
                    PhoneNumber = request.PhoneNumber,
                    OrderDate = now,
                    Receiver = request.Receiver,
                    Total = request.Total
                };
                var method = await _paymentMethodRepository.SingleOrDefaultAsync(x => x.Id == request.PaymentMethodId && x.IsActive)
                    ?? throw new ArgumentException(ErrorMessage.NOT_FOUND);

                order.PaymentMethodId = request.PaymentMethodId;
                order.PaymentMethodName = method.Name;

                await _orderRepository.AddAsync(order);

                double total = 0;

                var cartItems = await _cartItemRepository.GetAsync(e => e.UserId == userId && request.CartIds.Contains(e.Id));
                var listProductColorUpdate = new List<Product_Color>();
                var listProductUpdate = new List<Product>();
                var listOrderDetail = new List<OrderDetail>();

                foreach (var cartItem in cartItems)
                {
                    var color = await _productColorRepository
                        .SingleAsync(e => e.ProductId == cartItem.ProductId && e.ColorId == cartItem.ColorId);

                    if (color.Quantity < cartItem.Quantity)
                    {
                        throw new InvalidDataException(ErrorMessage.SOLDOUT);
                    }

                    var price = color.Prices - color.Prices * (cartItem.Product.Discount / 100.00);
                    price *= cartItem.Quantity;
                    total += price;

                    cartItem.Product.Sold += cartItem.Quantity;
                    listProductUpdate.Add(cartItem.Product);

                    color.Quantity -= cartItem.Quantity;
                    listProductColorUpdate.Add(color);

                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.Id,
                        ProductId = cartItem.ProductId,
                        ProductName = cartItem.Product.Name,
                        ColorId = cartItem.ColorId,
                        ColorName = color.Color.Name,
                        Quantity = cartItem.Quantity,
                        OriginPrice = color.Prices,
                        Price = price,
                        ImageUrl = color.ImageUrl,
                    };
                    listOrderDetail.Add(orderDetail);
                }

                await _orderDetailRepository.AddAsync(listOrderDetail);
                await _productRepository.UpdateAsync(listProductUpdate);
                await _productColorRepository.UpdateAsync(listProductColorUpdate);
                await _cartItemRepository.DeleteAsync(cartItems);

                if (method.Name == PaymentMethodEnum.VNPay.ToString())
                {
                    var orderInfor = new VNPayOrder
                    {
                        OrderId = order.Id,
                        Amount = total,
                        CreateDate = order.OrderDate,
                        Status = "0",
                        OrderInfor = "Đơn hàng: " + order.Id
                    };
                    var IpAddr = "127.0.0.1";
                    var paymentUrl = _paymentService.GetPaymentUrl(orderInfor, IpAddr);

                    var orderCache = new OrderCache()
                    {
                        OrderId = order.Id,
                        Url = paymentUrl,
                        vnp_CreateDate = order.OrderDate.ToString("yyyyMMddHHmmss"),
                        vnp_IpAddr = IpAddr,
                        vnp_OrderInfo = orderInfor.OrderInfor,
                    };

                    var cacheOpts = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(15)
                    };
                    cacheOpts.RegisterPostEvictionCallback(OnVNPayDeadline, this);
                    _cachingService.Set("Order " + order.Id, orderCache, cacheOpts);
                    return paymentUrl;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteOrder(long orderId)
        {
            var order = await _orderRepository.SingleOrDefaultAsync(e => e.Id == orderId);
            if (order != null)
            {
                await _orderRepository.DeleteAsync(order);
            }
            else throw new ArgumentException($"Id {orderId} " + ErrorMessage.NOT_FOUND);
        }

        public async Task<PageRespone<OrderDTO>> GetAllOrder(int page, int pageSize, string? key)
        {
            int total;
            IEnumerable<Order> orders;
            if (string.IsNullOrEmpty(key))
            {
                total = await _orderRepository.CountAsync();
                orders = await _orderRepository.GetPagedOrderByDescendingAsync(page, pageSize, null, e => e.CreateAt);
            }
            else
            {
                bool isLong = long.TryParse(key, out long idSearch);

                Expression<Func<Order, bool>> expression =
                    e => e.Id.Equals(idSearch)
                    || (!isLong && e.PaymentMethodName != null && e.PaymentMethodName.Contains(key));

                total = await _orderRepository.CountAsync(expression);
                orders = await _orderRepository.GetPagedOrderByDescendingAsync(page, pageSize, expression, e => e.CreateAt);
            }

            var item = _mapper.Map<IEnumerable<OrderDTO>>(orders);
            return new PageRespone<OrderDTO>
            {
                Items = item,
                Page = page,
                PageSize = pageSize,
                TotalItems = total
            };
        }

        private async void OnVNPayDeadline(object key, object? value, EvictionReason reason, object? state)
        {
            if (value != null)
            {
                using var _scope = _serviceScopeFactory.CreateScope();
                var orderRepository = _scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                var vnPayLibary = _scope.ServiceProvider.GetRequiredService<IVNPayLibrary>();
                var configuration = _scope.ServiceProvider.GetRequiredService<IConfiguration>();

                var data = (OrderCache)value;
                var vnp_QueryDrUrl = configuration["VNPay:vnp_QueryDrUrl"] ?? throw new Exception(ErrorMessage.NOT_FOUND);
                var vnp_HashSecret = configuration["VNPay:vnp_HashSecret"] ?? throw new Exception(ErrorMessage.NOT_FOUND);
                var vnp_TmnCode = configuration["VNPay:vnp_TmnCode"] ?? throw new Exception(ErrorMessage.NOT_FOUND);

                var queryDr = new VNPayQueryDr
                {
                    vnp_Command = "querydr",
                    vnp_RequestId = data.OrderId.ToString(),
                    vnp_Version = "2.1.0",
                    vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    vnp_TransactionDate = data.vnp_CreateDate,
                    vnp_IpAddr = data.vnp_IpAddr,
                    vnp_OrderInfo = data.vnp_OrderInfo,
                    vnp_TmnCode = vnp_TmnCode,
                    vnp_TxnRef = data.OrderId.ToString(),

                };
                var checksum = vnPayLibary.CreateSecureHashQueryDr(queryDr, vnp_HashSecret);

                var queryDrWithHash = new
                {
                    queryDr.vnp_Command,
                    queryDr.vnp_RequestId,
                    queryDr.vnp_Version,
                    queryDr.vnp_CreateDate,
                    queryDr.vnp_TransactionDate,
                    queryDr.vnp_IpAddr,
                    queryDr.vnp_OrderInfo,
                    queryDr.vnp_TmnCode,
                    queryDr.vnp_TxnRef,
                    vnPayLibary = checksum
                };

                using var httpClient = new HttpClient();

                var res = await httpClient.PostAsJsonAsync(vnp_QueryDrUrl, queryDrWithHash);
                VNPayQueryDrResponse? queryDrResponse = await res.Content.ReadFromJsonAsync<VNPayQueryDrResponse?>();

                if (queryDrResponse != null)
                {
                    bool checkSingature = vnPayLibary.ValidateQueryDrSignature(queryDrResponse, queryDrResponse.vnp_SecureHash, vnp_HashSecret);
                    if (checkSingature && queryDrResponse.vnp_ResponseCode == "00")
                    {
                        var order = await orderRepository.FindAsync(data.OrderId);
                        if (order != null)
                        {
                            long vnp_Amount = Convert.ToInt64(queryDrResponse.vnp_Amount) / 100;

                            if (queryDrResponse.vnp_TransactionStatus == "00" && vnp_Amount == order.Total)
                            {
                                order.PaymentTranId = queryDrResponse.vnp_TransactionNo;
                                order.AmountPaid = vnp_Amount;
                                order.OrderStatus = DeliveryStatusEnum.Confirmed;
                            }
                            else
                            {
                                order.OrderStatus = DeliveryStatusEnum.Canceled;
                            }
                            await orderRepository.UpdateAsync(order);
                        }
                    }
                }
            }
        }

        public async Task<PageRespone<OrderDTO>> GetOrderByUserId(string userId, PageResquest resquest)
        {
            var orders = await _orderRepository.GetPagedOrderByDescendingAsync(resquest.page, resquest.pageSize, e => e.UserId == userId, x => x.CreateAt);
            var total = await _orderRepository.CountAsync(e => e.UserId == userId);

            var items = _mapper.Map<IEnumerable<OrderDTO>>(orders).Select(x =>
            {
                x.PayBackUrl = _cachingService.Get<OrderCache?>("Order " + x.Id)?.Url;
                return x;
            });

            return new PageRespone<OrderDTO>
            {
                Items = items,
                TotalItems = total,
                Page = resquest.page,
                PageSize = resquest.pageSize,
            };
        }

        public async Task<OrderDetailResponse> GetOrderDetail(long orderId, string userId)
        {
            var order = await _orderRepository.SingleOrDefaultAsyncInclue(e => e.Id == orderId && e.UserId == userId);
            if (order != null)
            {
                return _mapper.Map<OrderDetailResponse>(order);
            }
            else throw new InvalidOperationException(ErrorMessage.NOT_FOUND);
        }

        public async Task<OrderDetailResponse> GetOrderDetail(long orderId)
        {
            var order = await _orderRepository.SingleOrDefaultAsyncInclue(e => e.Id == orderId);
            if (order != null)
            {
                return _mapper.Map<OrderDetailResponse>(order);
            }
            else throw new InvalidOperationException(ErrorMessage.NOT_FOUND);
        }

        public async Task CancelOrder(long orderId, string userId)
        {
            var order = await _orderRepository.SingleOrDefaultAsync(e => e.Id == orderId && e.UserId == userId);
            if (order != null)
            {
                if (order.OrderStatus.Equals(DeliveryStatusEnum.Proccessing)
                    || order.OrderStatus.Equals(DeliveryStatusEnum.Confirmed))
                {
                    order.OrderStatus = DeliveryStatusEnum.Canceled;
                    

                    _cachingService.Remove("Order " + orderId);
                    await _orderRepository.UpdateAsync(order);
                }
                else throw new Exception(ErrorMessage.ERROR);
            }
            else throw new ArgumentException($"Id {orderId} " + ErrorMessage.NOT_FOUND);
        }

        public async Task UpdateStatusOrder(long orderId)
        {
            var order = await _orderRepository.SingleOrDefaultAsync(e => e.Id == orderId);
            if (order != null)
            {
                if (!order.OrderStatus.Equals(DeliveryStatusEnum.Canceled)
                    || !order.OrderStatus.Equals(DeliveryStatusEnum.Done))
                {
                    order.OrderStatus += 1;
                    await _orderRepository.UpdateAsync(order);
                }
                else throw new Exception(ErrorMessage.ERROR);
            }
            else throw new ArgumentException($"Id {orderId} " + ErrorMessage.NOT_FOUND);
        }

        public async Task CancelOrder(long orderId)
        {
            var order = await _orderRepository.SingleOrDefaultAsync(e => e.Id == orderId);
            if (order != null)
            {
                if (order.OrderStatus.Equals(DeliveryStatusEnum.Proccessing)
                    || order.OrderStatus.Equals(DeliveryStatusEnum.Confirmed))
                {
                    order.OrderStatus = DeliveryStatusEnum.Canceled;

                    _cachingService.Remove("Order " + orderId);
                    await _orderRepository.UpdateAsync(order);
                }
                else throw new Exception(ErrorMessage.ERROR);
            }
            else throw new ArgumentException($"Id {orderId} " + ErrorMessage.NOT_FOUND);
        }

        public async Task ShippingOrder(long orderId, OrderShippingRequest request)
        {
            var order = await _orderRepository.SingleOrDefaultAsyncInclue(e => e.Id == orderId)
                ?? throw new Exception(ErrorMessage.NOT_FOUND);

            var token = _configuration["GHN:Token"];
            var shop_id = _configuration["GHN:ShopId"];
            var url = _configuration["GHN:Url"] + "/create";

            if (token == null || shop_id == null || url == null)
            {
                throw new Exception(ErrorMessage.INVALID);
            }
            string to_name = order.Receiver;
            string to_phone = order.PhoneNumber;
            string to_address = order.DeliveryAddress;
            string to_ward_code = order.WardCode;
            int to_district_id = order.DistrictId;
            int weight = request.Weight;
            int height = request.Height;
            int length = request.length;
            int width = request.Width;
            string required_note = request.DeliveryRequestEnum.ToString();

            double cod_amount = order.AmountPaid < order.Total ? order.Total : 0;

            var Items = order.OrderDetials.Select(d => new
            {
                name = d.ProductName,
                quantity = d.Quantity,
                price = d.Price,
            }).ToArray();

            var data = new
            {
                to_name,
                to_phone,
                to_address,
                to_ward_code,
                to_district_id,
                weight,
                height,
                length,
                width,
                required_note,
                service_type_id = 2,
                payment_type_id = 1,
                note = "Hàng dễ vỡ xin nhẹ tay",
                Items
            };

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("ShopId", shop_id);
            httpClient.DefaultRequestHeaders.Add("Token", token);

            var res = await httpClient.PostAsJsonAsync(url, data);
            var dataRes = await res.Content.ReadFromJsonAsync<GHNResponse>();

            if (!res.IsSuccessStatusCode)
            {
                throw new Exception(dataRes?.Message ?? ErrorMessage.INVALID);
            }

            order.ShippingCode = dataRes?.Data?.OrderCode;
            order.Expected_delivery_time = dataRes?.Data?.Expected_delivery_time;
            order.OrderStatus = DeliveryStatusEnum.Shipping;
            await _orderRepository.UpdateAsync(order);
        }
    }
}
