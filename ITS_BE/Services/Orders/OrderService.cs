using AutoMapper;
using ITS_BE.Constants;
using ITS_BE.DTO;
using ITS_BE.Enum;
using ITS_BE.Models;
using ITS_BE.ModelView;
using ITS_BE.Repository.CartItemRepository;
using ITS_BE.Repository.OrderRepository;
using ITS_BE.Repository.ProductColorRepository;
using ITS_BE.Repository.ProductRepository;
using ITS_BE.Request;
using ITS_BE.Response;
using ITS_BE.Services.Payment;
using System.Collections.Concurrent;

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
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, IProductColorRepository productColorRepository, ICartItemRepository cartItemRepository, IPaymentMethodRepository paymentMethodRepository, IOrderDetailRepository orderDetailRepository, IPaymentService paymentService, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _productColorRepository = productColorRepository;
            _cartItemRepository = cartItemRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _orderDetailRepository = orderDetailRepository;
            _paymentService = paymentService;
            _mapper = mapper;
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

                    //cartItem.Product.Sold += cartItem.Quantity;
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

                    return paymentUrl;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PageRespone<OrderDTO>> GetOrderByUserId(string userId, PageResquest resquest)
        {
            var orders = await _orderRepository.GetPagedOrderByDescendingAsync(resquest.page, resquest.pageSize, e => e.UserId == userId, x => x.CreateAt);
            var total = await _orderRepository.CountAsync(e => e.UserId == userId);

            var items = _mapper.Map<IEnumerable<OrderDTO>>(orders);
            return new PageRespone<OrderDTO>
            {
                Items = items,
                TotalItems = total,
                Page = resquest.page,
                PageSize = resquest.pageSize,
            };
        }
    }
}
