using ITS_BE.DTO;
using ITS_BE.Models;
using ITS_BE.Request;
using ITS_BE.Response;

namespace ITS_BE.Services.Receipts
{
    public interface IReceiptService
    {
        Task<ReceiptDTO> CreateReceipt(string userId, ReceiptRequest request);
        Task<PageRespone<ReceiptDTO>> GetAll(int page, int pageSize, string? key);
        Task<IEnumerable<ReceiptDetailResponse>> GetbyId(long id);
    }
}
