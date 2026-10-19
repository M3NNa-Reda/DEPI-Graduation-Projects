using EasyShop.Application.DTOs.Review;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Application.Interfaces.Services
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(int productId);
        Task<ReviewDto?> GetReviewByIdAsync(int reviewId);
        Task AddReviewAsync(int userId, AddReviewDto dto);
        Task UpdateReviewAsync(int userId, int reviewId, AddReviewDto dto);
        Task DeleteReviewAsync(int userId, int reviewId);
        Task<IEnumerable<ReviewDto>> GetUserReviewsAsync(int userId);

    }
}
