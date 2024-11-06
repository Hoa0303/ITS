using ITS_BE.DTO;
using ITS_BE.Request;
using ITS_BE.Response;

namespace ITS_BE.Services.Reviews
{
    public interface IReviewService
    {
        Task DeleteReview(string reviewId);
    }
}
