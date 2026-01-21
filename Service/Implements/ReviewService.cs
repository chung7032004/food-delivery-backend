using FoodDelivery.Entities;
using FoodDelivery.Service.Interfaces;
using FoodDelivery.DTOs.Review;
using FoodDelivery.Common;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Service.Implements
{
    public class ReviewService : IReviewService
    {
        private readonly FoodContext _context;
        public ReviewService(FoodContext context) { _context = context; }

        public async Task<Result> AddReviewAsync(CreateReviewDto dto, Guid customerId, CancellationToken cancellationToken = default)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
                return Result.Failure("Xếp hạng không hợp lệ", "Xếp hạng phải nằm trong khoảng từ 1 đến 5.");

            var orderItem = await _context.OrderItems
                .Include(oi => oi.Order)
                .FirstOrDefaultAsync(oi => oi.Id == dto.OrderItemId, cancellationToken);

            if (orderItem == null)
                return Result.Failure("OrderItemNotFound", "Không tìm thấy mặt hàng trong đơn hàng.");

            if (orderItem.Order.CustomerId != customerId)
                return Result.Failure("Không được phép", "Mặt hàng đặt hàng không thuộc về người dùng hiện tại.");

            if (orderItem.ProductId != dto.ProductId)
                return Result.Failure("Không được phép", "Mặt hàng đặt hàng không thuộc về người dùng hiện tại.");

            var product = await _context.Products.FindAsync(new object[] { dto.ProductId }, cancellationToken);
            if (product == null)
                return Result.Failure("Sản phẩm không được tìm thấy.", "Không tìm thấy sản phẩm để đánh giá.");

            var already = await _context.Reviews.AnyAsync(r => r.OrderItemId == dto.OrderItemId, cancellationToken);
            if (already)
                return Result.Failure("Đã được xem xét", "Mặt hàng này trong đơn hàng đã được xem xét.");

            var review = new Review
            {
                OrderItemId = dto.OrderItemId,
                ProductId = dto.ProductId,
                CustomerId = customerId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}