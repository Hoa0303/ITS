using AutoMapper;
using ITS_BE.DTO;
using ITS_BE.Models;
using ITS_BE.Repository.ProductColorRepository;
using ITS_BE.Repository.ProductRepository;
using ITS_BE.Repository.ReceiptRepository;
using ITS_BE.Request;
using ITS_BE.Response;
using MailKit.Search;

namespace ITS_BE.Services.Receipts
{
    public class ReceiptService : IReceiptService
    {
        private readonly IReceiptRepository _receiptRepository;
        private readonly IReceiptDetailRepository _receiptDetailRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductColorRepository _productColorRepository;
        private readonly IMapper _mapper;

        public ReceiptService(IReceiptRepository receiptRepository, IReceiptDetailRepository receiptDetailRepository,
            IMapper mapper, IProductRepository productRepository, IProductColorRepository productColorRepository)
        {
            _receiptRepository = receiptRepository;
            _receiptDetailRepository = receiptDetailRepository;
            _productRepository = productRepository;
            _productColorRepository = productColorRepository;
            _productColorRepository = productColorRepository;
            _mapper = mapper;
        }

        public async Task<ReceiptDTO> CreateReceipt(string userId, ReceiptRequest request)
        {
            try
            {
                var receipt = new Receipt
                {
                    UserId = userId,
                    Note = request.Note,
                    Total = request.Total,
                };
                await _receiptRepository.AddAsync(receipt);

                var listProductColorUpdate = new List<Product_Color>();
                var listReceiptDetail = new List<ReceiptDetail>();

                foreach (var item in request.ReceiptProducts)
                {
                    var productColor = await _productColorRepository
                        .SingleOrDefaultAsync(e => e.ProductId == item.ProductId && e.ColorId == item.ColorId);
                    if (productColor != null)
                    {
                        productColor.Quantity += item.Quantity;
                        listProductColorUpdate.Add(productColor);
                    }

                    var receiptDetail = new ReceiptDetail
                    {
                        ReceiptId = receipt.Id,
                        ProductId = item.ProductId,
                        ColorId = item.ColorId,
                        Quantity = item.Quantity,
                        CostPrice = item.CostPrice,
                    };
                    listReceiptDetail.Add(receiptDetail);
                }
                await _productColorRepository.UpdateAsync(listProductColorUpdate);
                await _receiptDetailRepository.AddAsync(listReceiptDetail);

                return _mapper.Map<ReceiptDTO>(receipt);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PageRespone<ReceiptDTO>> GetAll(int page, int pageSize, string? key)
        {
            int total;
            IEnumerable<Receipt> receipts;
            if (string.IsNullOrEmpty(key))
            {
                total = await _receiptRepository.CountAsync();
                receipts = await _receiptRepository.GetPagedOrderByDescendingAsync(page, pageSize, null, e => e.CreateAt);
            }
            else
            {
                total = await _receiptRepository.CountAsync();
                receipts = await _receiptRepository.GetPagedOrderByDescendingAsync(page, pageSize, null, e => e.CreateAt);
            }

            var item = _mapper.Map<IEnumerable<ReceiptDTO>>(receipts);
            return new PageRespone<ReceiptDTO>
            {
                Items = item,
                Page = page,
                PageSize = pageSize,
                TotalItems = total
            };
        }
    }
}
