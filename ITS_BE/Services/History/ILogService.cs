using ITS_BE.DTO;
using ITS_BE.Request;
using ITS_BE.Response;

namespace ITS_BE.Services.History
{
    public interface ILogService
    {
        Task<ReceiptDTO> CreateLog(LogRequest request);
        Task<PageRespone<LogDTO>> GetAll(int page, int pageSize, string? key);
    }
}
