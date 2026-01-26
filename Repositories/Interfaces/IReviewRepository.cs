using FoodDelivery.Entities;

namespace FoodDelivery.Repositories;
public interface IReviewRepository
{
    Task<OrderItem?> GetOrderItemWithOrderAsync(Guid orderItemId);
    Task<Review?> GetByIdAsync(Guid reviewId);
    Task<List<Review>> GetMyReviewAsync(Guid customerId, int page, int pageSize);
    Task<List<Review>> GetProductReviewsAsync(Guid productId, int page, int pageSize);
    Task<List<Review>> GetAllReviewsAsync(int ? rating, bool ? isHidden, int page, int pageSize);
    Task<bool> ExistsByOrderItemIdAsync(Guid OrderItem);
    Task AddAsync (Review review);
    Task UpdateAsync (Review review);
    Task DeleteAsync(Review review);
    Task<int> MyTotalCountReviewAsync(Guid customerId);
    Task<int> TotalCountReviewAsync();
    Task<int> TotalCountReviewByProductAsync(Guid productId);
    IQueryable<Review> GetQueryable();
    Task<List<KeyValuePair<int, int>>> GetStarCountStatisticsAsync(Guid? productId);
}