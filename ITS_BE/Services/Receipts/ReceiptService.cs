using AutoMapper;
using ITS_BE.Constants;
using ITS_BE.DTO;
using ITS_BE.Models;
using ITS_BE.Repository.ProductColorRepository;
using ITS_BE.Repository.ProductRepository;
using ITS_BE.Repository.ReceiptRepository;
using ITS_BE.Request;
using ITS_BE.Response;
using ITS_BE.Services.History;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;

namespace ITS_BE.Services.Receipts
{
    public class ReceiptService : IReceiptService
    {
        private readonly UserManager<User> _userManager;
        private readonly IReceiptRepository _receiptRepository;
        private readonly IReceiptDetailRepository _receiptDetailRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductColorRepository _productColorRepository;
        private readonly ILogService _logService;
        private readonly IMapper _mapper;

        public ReceiptService(IReceiptRepository receiptRepository, IReceiptDetailRepository receiptDetailRepository,
            IMapper mapper, IProductRepository productRepository, IProductColorRepository productColorRepository,
            UserManager<User> userManager, ILogService logService)
        {
            _receiptRepository = receiptRepository;
            _userManager = userManager;
            _receiptDetailRepository = receiptDetailRepository;
            _productRepository = productRepository;
            _productColorRepository = productColorRepository;
            _productColorRepository = productColorRepository;
            _logService = logService;
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
                    EntryDate = request.EntryDate,
                };
                await _receiptRepository.AddAsync(receipt);

                var listProductColorUpdate = new List<Product_Color>();
                var listReceiptDetail = new List<ReceiptDetail>();

                foreach (var item in request.ReceiptProducts)
                {
                    var productColor = await _productColorRepository
                        .SingleOrDefaultAsync(e => e.ProductId == item.ProductId && e.ColorId == item.ColorId);
                    var product = await _productRepository.SingleOrDefaultAsync(e=>e.Id == item.ProductId);
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
                        ProductName = product?.Name ?? "",
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
                bool isLong = long.TryParse(key, out long idSearch);

                Expression<Func<Receipt, bool>> expression =
                    e => e.Id.Equals(idSearch)
                    || (!isLong && e.User.FullName != null && e.User.FullName.Contains(key));

                total = await _receiptRepository.CountAsync();
                receipts = await _receiptRepository.GetPagedOrderByDescendingAsync(page, pageSize, expression, e => e.CreateAt);
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

        public async Task<IEnumerable<ReceiptDetailResponse>> GetbyId(long id)
        {
            var receipt = await _receiptDetailRepository.GetAsync(e => e.ReceiptId == id);
            if (receipt != null)
            {
                return _mapper.Map<IEnumerable<ReceiptDetailResponse>>(receipt);
            }
            else throw new InvalidOperationException(ErrorMessage.NOT_FOUND);
        }

        public async Task<ReceiptDTO> UpdateReceipt(long receiptId, ReceiptRequest request)
        {
            try
            {
                var receipt = await _receiptRepository.FindAsync(receiptId)
                    ?? throw new ArgumentException(ErrorMessage.NOT_FOUND);

                var logRequest = new LogRequest
                {
                    UserId = receipt.UserId,
                    Note = receipt.Note,
                    Total = receipt.Total,
                    EntryDate = receipt.EntryDate,
                    ReceiptId = receiptId,
                    logProducts = (await _receiptDetailRepository
                    .GetAsync(e => e.ReceiptId == receiptId))
                    .Select(d => new LogProduct
                    {
                        ProductName = d.Product.Name,
                        ColorName = d.Color.Name,
                        CostPrice = d.CostPrice,
                        Quantity = d.Quantity,
                    }).ToList()
                };

                await _logService.CreateLog(logRequest);

                receipt.Note = request.Note;
                receipt.Total = request.Total;
                receipt.EntryDate = request.EntryDate;

                await _receiptRepository.UpdateAsync(receipt);

                var listReceiptDetailUpdate = new List<ReceiptDetail>();
                var listProductColorUpdate = new List<Product_Color>();
                var receiptDetail = await _receiptDetailRepository.GetAsync(e => e.ReceiptId == receiptId);

                foreach (var item in request.ReceiptProducts)
                {
                    int quantityOld = 0;

                    var existingDetail = receiptDetail
                        .FirstOrDefault(d => d.ProductId == item.ProductId && d.ColorId == item.ColorId);
                    if (existingDetail != null)
                    {
                        quantityOld = existingDetail.Quantity;
                        existingDetail.Quantity = item.Quantity;
                        existingDetail.CostPrice = item.CostPrice;
                        listReceiptDetailUpdate.Add(existingDetail);
                    }

                    var productColor = await _productColorRepository
                        .SingleOrDefaultAsync(d => d.ProductId == item.ProductId && d.ColorId == item.ColorId);

                    if (productColor != null)
                    {
                        productColor.Quantity += (item.Quantity - quantityOld);
                        listProductColorUpdate.Add(productColor);
                    }
                }

                await _productColorRepository.UpdateAsync(listProductColorUpdate);
                await _receiptDetailRepository.UpdateAsync(listReceiptDetailUpdate);

                return _mapper.Map<ReceiptDTO>(receipt);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
