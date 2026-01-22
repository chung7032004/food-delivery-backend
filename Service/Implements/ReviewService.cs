using FoodDelivery.Entities;
using FoodDelivery.Service.Interfaces;
using FoodDelivery.DTOs.Review;
using FoodDelivery.Common;
using Microsoft.EntityFrameworkCore;
using FoodDelivery.Repositories;
using FoodDelivery.Repositories.Interfaces;

namespace FoodDelivery.Service.Implements
{
    public class ReviewService : IReviewService
    {
        private readonly FoodContext _context;
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository, FoodContext context) 
        {
            _reviewRepository = reviewRepository; 
            _context = context;
        }

        public async Task<Result> AddReviewAsync(ReviewCreateDto request, Guid customerId)
        {
            if (request.Rating < 1 || request.Rating > 5)
                return Result.Failure("RATING_INVALID", "Xếp hạng phải nằm trong khoảng từ 1 đến 5.");
            var orderItem = await _reviewRepository.GetOrderItemWithOrderAsync(request.OrderItemId);
            if (orderItem == null)
                return Result.Failure("ORDER_ITEM_NOT_FOUND","Không tìm thấy món ăn trong đơn hàng.");
            if (orderItem.Order.CustomerId != customerId)
                return Result.Failure("FORBIDDEN", "Món ăn này không thuộc đơn hàng của bạn.");
            if (await _reviewRepository.ExistsByOrderItemIdAsync(request.OrderItemId))
                return Result.Failure("ORDER_ITEM_ALREADY_REVIEWED", "Món ăn này trong đơn hàng đã được đánh giá.");
            var review = new Review
            {
                OrderItemId = request.OrderItemId,
                ProductId = orderItem.ProductId,
                CustomerId = customerId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };
            await _reviewRepository.AddAsync(review);
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        public async Task<Result<PagedResponse<ReviewHistoryResponseDto>>> GetMyReviewsAsync(Guid customerId, int page, int pageSize)
        {
            pageSize = pageSize > 100 ? 100 : pageSize < 1 ? 10 : pageSize;
            page = page < 1 ? 1 : page;
            var reviews = await _reviewRepository.GetMyReviewAsync(customerId,page,pageSize);
            var reviewsResponse = reviews.Select(r=> new ReviewHistoryResponseDto
            {
                Id = r.Id,
                ProductName = r.Product.Name, 
                ProductImageUrl = r.Product.ImageUrl, 
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                IsHidden = r.IsHidden
            }).ToList();
            var totalCount = await _reviewRepository.MyTotalCountReviewAsync(customerId);
            var responseMeta = new PaginationMeta(
                Page : page,
                PageSize : pageSize,
                TotalCount : totalCount,
                TotalPages : (int)Math.Ceiling((double)totalCount/pageSize)
            );
            var response = new PagedResponse<ReviewHistoryResponseDto>(
                Meta : responseMeta,
                Data : reviewsResponse
            );
            return Result<PagedResponse<ReviewHistoryResponseDto>>.Success(response);
        }
        public async Task<Result<PagedResponse<ProductReviewDto>>> GetReviewsAsync(Guid productId, int page, int pageSize)
        {
            pageSize = pageSize > 100 ? 100 : pageSize < 1 ? 10 : pageSize;
            page = page < 1 ? 1 : page;
            var reviews = await _reviewRepository.GetProductReviewsAsync(productId,page,pageSize);
            var reviewsResponse = reviews.Select(r=> new ProductReviewDto
            {
                CustomerName = r.Customer.FullName, 
                AvatarUrl = r.Customer.AvatarUrl,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            }).ToList();
            var totalCount = await _reviewRepository.TotalCountReviewByProductAsync(productId);
            var responseMeta = new PaginationMeta(
                Page : page,
                PageSize : pageSize,
                TotalCount : totalCount,
                TotalPages : (int)Math.Ceiling((double)totalCount/pageSize)
            );
            var response = new PagedResponse<ProductReviewDto>(
                Meta : responseMeta,
                Data : reviewsResponse
            );
            return Result<PagedResponse<ProductReviewDto>>.Success(response);
        }
        public async Task<Result> UpdateReviewAsync(Guid customerId, Guid reviewId, ReviewUpdateDto request)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if(review == null) 
                return Result.Failure("REVIEW_NOT_FOUND", "Không tìm thấy đánh giá.");
            if(review.CustomerId != customerId) 
                return Result.Failure("FORBIDDEN", "Đánh giá này không thuộc đơn hàng của bạn.");
            var timeLimit = review.CreatedAt.AddDays(3);
            if(DateTime.UtcNow > timeLimit) 
                return Result.Failure("REVIEW_EDIT_EXPIRED","Đã quá thời hạn 3 ngày để chỉnh sửa đánh giá này.");
            review.Comment = request.Comment;
            review.Rating = request.Rating;
            await _reviewRepository.UpdateAsync(review);
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        public async Task<Result> DeleteReviewAsync(Guid customerId, Guid reviewId)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if(review == null) 
                return Result.Failure("REVIEW_NOT_FOUND", "Không tìm thấy đánh giá.");
            if(review.CustomerId != customerId) 
                return Result.Failure("FORBIDDEN", "Bạn không có quyền xóa đánh giá này.");
            await _reviewRepository.DeleteAsync(review);
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        public async Task<Result<PagedResponse<AdminReviewDto>>> GetAllReviewsAsync(int ? rating, bool ? isHidden, int page, int pageSize)
        {
            var reviews = await _reviewRepository.GetAllReviewsAsync(rating,isHidden,page,pageSize);
            var reviewsResponse = reviews.Select(r=> new AdminReviewDto
            {
                Id = r.Id,
                ProductName = r.Product.Name, 
                AvatarUrl = r.Customer.AvatarUrl,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                IsHidden = r.IsHidden,
                CustomerId = r.CustomerId,
                CustomerName = r.Customer.FullName,
                OrderItemId = r.OrderItemId,
            }).ToList();
            var totalCount = await _reviewRepository.TotalCountReviewAsync();
            var responseMeta = new PaginationMeta(
                Page : page,
                PageSize : pageSize,
                TotalCount : totalCount,
                TotalPages : (int)Math.Ceiling((double)totalCount/pageSize)
            );
            var response = new PagedResponse<AdminReviewDto>(
                Meta : responseMeta,
                Data : reviewsResponse
            );
            return Result<PagedResponse<AdminReviewDto>>.Success(response);
        }
        public async Task<Result> HideReviewAsync(Guid reviewId)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if(review == null)
            {
                return Result.Failure("REVIEW_NOT_FOUND","Không tìm thấy đánh giá.");
            }
            if (review.IsHidden)
            {
                return Result.Failure("REVIEW_ALREADY_HIDDEN","Đánh giá này đã bị ẩn trước đó rồi");
            }
            review.IsHidden = true;
            await _reviewRepository.UpdateAsync(review);
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        public async Task<ReviewReportDto> GetReviewReportAsync(Guid? productId)
        {
            var query = _reviewRepository.GetQueryable();
            if (productId.HasValue)
            {
                query = query.Where(q=>q.ProductId == productId);
            }
            var totalReviews = await query.CountAsync();
            var averageRating = totalReviews > 0 ? await query.AverageAsync(r=>r.Rating) : 0;
            var hiddenCount = await query.CountAsync(r=>r.IsHidden);
            var startGroups = await _reviewRepository.GetStarCountStatisticsAsync(productId);
            var starCounts = new Dictionary<int, int>();
            for(var i = 1 ; i<=5; i++)
            {
                starCounts[i] = startGroups.FirstOrDefault(s=>s.Key == i).Value;
            }
            return new ReviewReportDto
            {
                TotalReviews = totalReviews,
                AverageRating = Math.Round(averageRating, 1),
                StarCounts = starCounts,
                HiddenReviewsCount = hiddenCount
            };
        }
        public async Task<Result> DeleteAsync(Guid reviewId)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if(review == null)
            {
                return Result.Failure("REVIEW_NOT_FOUND","Không tìm thấy đánh giá.");
            }
            await _reviewRepository.DeleteAsync(review);
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        
    }        
}