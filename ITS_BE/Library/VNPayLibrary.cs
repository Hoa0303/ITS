using ITS_BE.Constants;
using ITS_BE.ModelView;
using Newtonsoft.Json;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace ITS_BE.Library
{
    public class VNPayLibrary : IVNPayLibrary
    {
        private string HmacSHA512(string hashsecret, string data)
        {
            var hash = new StringBuilder();
            var keyBytes = Encoding.UTF8.GetBytes(hashsecret);
            var dataBytes = Encoding.UTF8.GetBytes(data);

            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hashValue = hmac.ComputeHash(dataBytes);
                foreach (var kv in hashValue)
                {
                    hash.Append(kv.ToString("x2"));
                }
            }
            return hash.ToString();
        }
        public string createUrl(VNPay vnPay, string vnp_Url, string vnp_HashSecret)
        {
            var json = JsonConvert.SerializeObject(vnPay);
            var dictionary = JsonConvert.DeserializeObject<IDictionary<string, string>>(json);

            if (dictionary != null)
            {
                var data = dictionary.Where(e => !string.IsNullOrEmpty(e.Value))
                                     .OrderBy(e => e.Key)
                                     .Select(x => $"{WebUtility.UrlEncode(x.Key)}={WebUtility.UrlEncode(x.Value)}");

                var dataString = string.Join("&", data);

                var vnp_SecureHash = HmacSHA512(vnp_HashSecret, dataString);
                var requestUrl = $"{vnp_Url}?{dataString}&vnp_SecureHash={vnp_SecureHash}";
                return requestUrl;
            }
            throw new Exception(ErrorMessage.INVALID);
        }
    }
}
