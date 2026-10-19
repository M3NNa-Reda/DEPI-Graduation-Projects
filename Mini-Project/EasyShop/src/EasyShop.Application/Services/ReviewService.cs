using EasyShop.Application.DTOs.Review;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task AddReviewAsync(int userId, AddReviewDto dto)
        {
            if(dto.Rating < 1 || dto.Rating >5)
                throw new ArgumentException("Rating must be between 1 and 5.");

            if(string.IsNullOrWhiteSpace(dto.Comment))
                throw new ArgumentException("Comment cannot be empty.");


            var orderItem = await _unitOfWork.OrderItems.GetByIdAsync(dto.OrderItemId);

            if(orderItem == null)
                throw new ArgumentException("Order item not found.");

            var review = new Review
            {
                UserId = userId,
                OrderItemId = dto.OrderItemId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.CompleteAsync();
        }
        public async Task<ReviewDto?> GetReviewByIdAsync(int reviewId)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
            
            if(review == null)
                return null;

            return new ReviewDto
            {
                Id = review.Id,
                OrderItemId = review.OrderItemId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            };
        }
        public async Task<IEnumerable<ReviewDto>> GetUserReviewsAsync(int userId)
        {
            var review = await _unitOfWork.Reviews.FindByConditionAsync(r=>r.UserId== userId);

            return review.Select(review => new ReviewDto
            {
                Id = review.Id,
                OrderItemId = review.OrderItemId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            });
        }
        public async Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(int productId)
        {
            var review = await _unitOfWork.Reviews
                .FindByConditionAsync(r => r.OrderItem.ProductVariant.ProductId == productId);

            return review.Select(review => new ReviewDto
            {
                Id = review.Id,
                OrderItemId = review.OrderItemId,
                ProductId = productId,
                ProductName = review.OrderItem.ProductVariant.Product.Name,
                UserName = review.ApplicationUser.FirstName ?? string.Empty,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            });
        }
        public async Task UpdateReviewAsync(int userId, int reviewId, AddReviewDto dto)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);

            if (review == null)
                throw new ArgumentException("Review not found.");
           
            if(review.UserId != userId)
                throw new UnauthorizedAccessException(
                      "You cannot update another user's review.");

            if(dto.Rating < 1 || dto.Rating > 5 )
                throw new ArgumentException("Rating must be between 1 and 5.");

            if (string.IsNullOrWhiteSpace(dto.Comment))
                throw new ArgumentException("Comment cannot be empty.");

            review.Rating = dto.Rating;
            review.Comment = dto.Comment;
            review.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Reviews.Update(review);
            await _unitOfWork.CompleteAsync();
        }
        public async Task DeleteReviewAsync(int userId, int reviewId)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);

            if (review == null)
                throw new ArgumentException("Review not found.");

            if (review.UserId != userId)
                throw new UnauthorizedAccessException(
                    "You cannot delete another user's review.");
            _unitOfWork.Reviews.Delete(review);
            await _unitOfWork.CompleteAsync();
        }
    }
}
