using ITS_BE.DTO;
using ITS_BE.ModelView;

namespace ITS_BE.Services.Payment
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentMethodDTO>> GetPaymentMethods();
        Task<string?> IsActivePaymentMethod(int id);
        string GetPaymentUrl(VNPayOrder order, string ipAddr, string? locale = null);
        Task VNPayCallback(VNPayRequest request);
    }
}
