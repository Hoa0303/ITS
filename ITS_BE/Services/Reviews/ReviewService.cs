using AutoMapper;
using ITS_BE.Constants;
using ITS_BE.DTO;
using ITS_BE.Repository.ProductRepository;
using ITS_BE.Repository.ReviewRepository;
using ITS_BE.Request;
using ITS_BE.Response;

namespace ITS_BE.Services.Reviews
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IProductRepository _productRepository;

        public ReviewService(IReviewRepository reviewRepository, IProductRepository productRepository)
        {
            _reviewRepository = reviewRepository;
            _productRepository = productRepository;
        }

        public async Task DeleteReview(string reviewId)
        {
            var review = await _reviewRepository.FindAsync(reviewId);
            if (review != null)
            {
                var product = await _productRepository.FindAsync(review.ProductId);
                if (product != null)
                {
                    var currentStart = product.Rating * product.RatingCount;
                    product.Rating = (currentStart - review.Start) / (product.RatingCount - 1);
                    product.RatingCount -= 1;

                    await _productRepository.UpdateAsync(product);
                }
                await _reviewRepository.DeleteAsync(review);
            }
            else throw new InvalidOperationException(ErrorMessage.NOT_FOUND);
        }
    }
}
