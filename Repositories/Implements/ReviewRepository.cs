using FoodDelivery.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Repositories.Implementations;
public class ReviewRepository : IReviewRepository
{
    private readonly FoodContext _context;
    public ReviewRepository(FoodContext context)
    {
        _context = context;
    }
    public async Task<OrderItem?> GetOrderItemWithOrderAsync(Guid orderItemId)
    {
        return await _context.OrderItems.Include(oi=>oi.Order)
            .FirstOrDefaultAsync(oi => oi.Id == orderItemId);
    }
    public async Task<List<Review>> GetMyReviewAsync(Guid customerId, int page, int pageSize)
    {
        return await _context.Reviews.AsNoTracking()
            .Where(r=>r.CustomerId == customerId && !r.IsHidden)
            .Include(r=>r.Product)
            .OrderByDescending(r=>r.CreatedAt)
            .Skip((page-1)*pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    public async Task<bool> ExistsByOrderItemIdAsync(Guid OrderItem)
    {
        return await _context.Reviews.AnyAsync(r=>r.OrderItemId == r.OrderItemId);
    }
    public async Task AddAsync (Review review)
    {
        await _context.Reviews.AddAsync(review);
    }
    public async Task UpdateAsync (Review review)
    {
        _context.Reviews.Update(review);
        await Task.CompletedTask;
    }
    public async Task DeleteAsync(Review review)
    {
        _context.Reviews.Remove(review);
        await Task.CompletedTask;
    }
    public async Task<int> MyTotalCountReviewAsync(Guid customerId)
    {
        return await _context.Reviews.Where(r=>r.CustomerId== customerId).CountAsync();
    }
    public async Task<Review?> GetByIdAsync(Guid reviewId)
    {
        return await _context.Reviews.FindAsync(reviewId);
    }
    public async Task<List<Review>> GetProductReviewsAsync(Guid productId, int page, int pageSize)
    {
        return await _context.Reviews.AsNoTracking()
            .Where(r=>r.ProductId == productId)
            .Include(r=>r.Customer)
            .OrderByDescending(r=>r.CreatedAt)
            .Skip((page-1)*pageSize)
            .Take(page)
            .ToListAsync();
    }
    public async Task<int> TotalCountReviewByProductAsync(Guid productId)
    {
        return await _context.Reviews.Where(r=>r.ProductId == productId).CountAsync();
    }
    public async Task<List<Review>> GetAllReviewsAsync(int ? rating, bool ? isHidden, int page, int pageSize)
    {
        var query =  _context.Reviews
            .AsNoTracking()
            .Include(r=>r.Customer)
            .Include(r=>r.Product)
            .AsQueryable();
        if (rating.HasValue)
        {
            query = query.Where(r=>r.Rating == rating.Value);
        }
        if (isHidden.HasValue)
        {
            query = query.Where(r=>r.IsHidden == isHidden);
        }
        var reviews = await query
            .OrderByDescending(r=>r.CreatedAt)
            .Skip((page -1 ) * pageSize)
            .Take(pageSize)
            .ToListAsync();  
        return reviews; 
    }

    public async Task<int> TotalCountReviewAsync()
    {
        return await _context.Reviews.CountAsync();
    }
    public IQueryable<Review> GetQueryable() => _context.Reviews.AsQueryable();
    public async Task<List<KeyValuePair<int, int>>> GetStarCountStatisticsAsync(Guid? productId)
    {
        var query = _context.Reviews.AsQueryable();
        if (productId.HasValue)
            query = query.Where(r => r.ProductId == productId);
        return await query
            .GroupBy(r=>r.Rating)
            .Select(g=> new KeyValuePair<int, int >(g.Key, g.Count()))
            .ToListAsync();
    }
}