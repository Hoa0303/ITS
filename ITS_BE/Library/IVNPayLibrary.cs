using ITS_BE.ModelView;

namespace ITS_BE.Library
{
    public interface IVNPayLibrary
    {
        string createUrl(VNPay vnPay, string vnp_Url, string vnp_HashSecret);
    }
}
