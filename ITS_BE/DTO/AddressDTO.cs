using System.ComponentModel.DataAnnotations;

namespace ITS_BE.DTO
{
    public class AddressDTO
    {
        public string Name { get; set; }
        public string? PhoneNumber { get; set; }
        public int? Province_code { get; set; }
        public string? Province_name { get; set; }

        public int? District_code { get; set; }
        public string? District_name { get; set; }

        public int? Ward_code { get; set; }
        public string? Ward_name { get; set; }
        public string? Detail { get; set; }
    }
}
