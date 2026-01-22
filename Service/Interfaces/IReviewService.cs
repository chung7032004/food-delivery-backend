using FoodDelivery.Common;
using FoodDelivery.DTOs.Review;

namespace FoodDelivery.Service.Interfaces
{
    public interface IReviewService
    {
        Task<Result> AddReviewAsync(ReviewCreateDto request, Guid customerId);
        Task<Result<PagedResponse<ReviewHistoryResponseDto>>> GetMyReviewsAsync(Guid customerId, int page, int pageSize);
        Task<Result<PagedResponse<ProductReviewDto>>> GetReviewsAsync(Guid productId, int page, int pageSize);
        Task<Result> UpdateReviewAsync(Guid customerId, Guid reviewId, ReviewUpdateDto request) ;
        Task<Result> DeleteReviewAsync(Guid customerId, Guid reviewId);
        Task<Result<PagedResponse<AdminReviewDto>>> GetAllReviewsAsync(int ? rating, bool ? isHidden, int page, int pageSize);
        Task<Result> HideReviewAsync(Guid reviewId);
        Task<ReviewReportDto> GetReviewReportAsync(Guid? productId);
        Task<Result> DeleteAsync(Guid reviewId);
    }
}