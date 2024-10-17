using ITS_BE.ModelView;

namespace ITS_BE.Library
{
    public interface IVNPayLibrary
    {
        string createUrl(VNPay vnPay, string vnp_Url, string vnp_HashSecret);
        string CreateSecureHashQueryDr(VNPayQueryDr queryDr, string vnp_HashSecret);
        bool ValidateQueryDrSignature(VNPayQueryDrResponse response, string vnp_SecureHash, string vnp_HashSecret);
        bool ValidateSignature(VNPayRequest request, string vnp_SecureHash, string vnp_HashSecret);
    }
}
