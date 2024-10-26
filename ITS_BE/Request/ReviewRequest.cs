using System.ComponentModel.DataAnnotations;

namespace ITS_BE.Request
{
    public class ReviewRequest
    {
        public int ProductId { get; set; }

        [Range(0, 5)]
        public int Start { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }
    }
}
