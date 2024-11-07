using AutoMapper;
using ITS_BE.Constants;
using ITS_BE.DTO;
using ITS_BE.Models;
using ITS_BE.Repository.LogRepository;
using ITS_BE.Request;
using ITS_BE.Response;
using MailKit.Search;
using System.Linq.Expressions;

namespace ITS_BE.Services.History
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _logRepository;
        private readonly ILogDetailRepository _logDetailRepository;
        private readonly IMapper _mapper;

        public LogService(ILogRepository logRepository, ILogDetailRepository logDetailRepository, IMapper mapper)
        {
            _logRepository = logRepository;
            _logDetailRepository = logDetailRepository;
            _mapper = mapper;
        }

        public async Task<ReceiptDTO> CreateLog(LogRequest request)
        {
            var log = new Log
            {
                UserId = request.UserId,
                Note = request.Note,
                Total = request.Total,
                EntryDate = request.EntryDate,
                ReceiptId = request.ReceiptId,
            };
            await _logRepository.AddAsync(log);

            var listLogDetail = new List<LogDetail>();

            foreach (var item in request.logProducts)
            {
                var logDetail = new LogDetail
                {
                    LogId = log.Id,
                    ProductName = item.ProductName,
                    ColorName = item.ColorName,
                    Quantity = item.Quantity,
                    CostPrice = item.CostPrice,
                };
                listLogDetail.Add(logDetail);
            }
            await _logDetailRepository.AddAsync(listLogDetail);

            return _mapper.Map<ReceiptDTO>(log);
        }

        public async Task<PageRespone<LogDTO>> GetAll(int page, int pageSize, string? key)
        {
            int total = 0;
            IEnumerable<Log> logs;
            if (string.IsNullOrEmpty(key))
            {
                total = await _logRepository.CountAsync();
                logs = await _logRepository.GetPagedOrderByDescendingAsync(page, pageSize, null, e => e.CreateAt);
            }
            else
            {
                total = await _logRepository.CountAsync();
                logs = await _logRepository
                    .GetPagedOrderByDescendingAsync(page, pageSize, e => e.ReceiptId == long.Parse(key), e => e.CreateAt);
            }

            var items = _mapper.Map<IEnumerable<LogDTO>>(logs);
            return new PageRespone<LogDTO>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = total
            };
        }

        public async Task<IEnumerable<ReceiptDetailResponse>> GetById(long id)
        {
            var log = await _logDetailRepository.GetAsync(e => e.LogId == id);
            if (log != null)
            {
                return _mapper.Map<IEnumerable<ReceiptDetailResponse>>(log);
            }
            else throw new InvalidOperationException(ErrorMessage.NOT_FOUND);
        }
    }
}
