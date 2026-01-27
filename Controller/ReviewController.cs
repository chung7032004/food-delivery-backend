using Microsoft.AspNetCore.Mvc;
using FoodDelivery.Entities;
using FoodDelivery.Service.Interfaces;
using FoodDelivery.DTOs.Review;
using Microsoft.AspNetCore.Authorization;
using FoodDelivery.Extensions;
using FoodDelivery.Common;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{

    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateReview([FromBody] ReviewCreateDto request)
    {
        var customerId = User.GetUserId();
        var result = await _reviewService.AddReviewAsync(request, customerId);

        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            {
                   
                "ORDER_ITEM_NOT_FOUND" => NotFound(result),
                "RATING_INVALID" => BadRequest(result),
                "ORDER_ITEM_ALREADY_REVIEWED" => Conflict(result),
                "FORBIDDEN" => Forbid(),
                _ => BadRequest(result)
            };
        }
        return Ok(result);
    }
    //GET	/api/customers/me/reviews
    [HttpGet("me/reviews")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMyReviews([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if(!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(Result<PagedResponse<ReviewHistoryResponseDto>>.Failure("INVALID_TOKEN","Token không hợp lệ hoặc thiếu UserId."));
        }
        var result = await _reviewService.GetMyReviewsAsync(userId,page,pageSize);
        return Ok(result);
    }
    //PUT	/api/reviews/{id}
    [HttpPut("{reviewId}")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> UpdateReview(Guid reviewId, [FromBody] ReviewUpdateDto request)
    {
        if(!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(Result.Failure("INVALID_TOKEN","Token không hợp lệ hoặc thiếu UserId."));
        }
        var result = await _reviewService.UpdateReviewAsync(userId,reviewId,request);
        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            {
                   
                "REVIEW_NOT_FOUND" => NotFound(result),
                "REVIEW_EDIT_EXPIRED" => BadRequest(result),
                "FORBIDDEN" => Forbid(),
                _ => BadRequest(result)
            };
        }
        return Ok(result);
    }
    //DELETE	/api/reviews/{id}
    [HttpDelete("{reviewId}")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> DeleteReview(Guid reviewId)
    {
        if(!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(Result.Failure("INVALID_TOKEN","Token không hợp lệ hoặc thiếu UserId."));
        }
        var result = await _reviewService.DeleteReviewAsync(userId,reviewId);
        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            {
                   
                "REVIEW_NOT_FOUND" => NotFound(result),
                "FORBIDDEN" => Forbid(),
                _ => BadRequest(result)
            };
        }
        return Ok(result);
    }
    //GET	/api/products/{productId}/reviews
    [HttpGet("/api/products/{productId}/reviews")]
    [AllowAnonymous] 
    public async Task<IActionResult> GetProductReviews(Guid productId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _reviewService.GetReviewsAsync(productId,page,pageSize);
        return Ok(result);
    }
    //GET	/api/admin/reviews
    [HttpGet("/api/admin/reviews")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllReviews(
        [FromQuery] int? rating,      
        [FromQuery] bool? isHidden,   
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20)
    {
        var result = await _reviewService.GetAllReviewsAsync(rating,isHidden,page,pageSize);
        return Ok(result);
    }
    //PATCH	/api/admin/reviews/{id}/hide
    [HttpPatch("/api/admin/reviews/{reviewId}/hidden")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> HiddenReview(Guid reviewId)
    {
        var result = await _reviewService.HiddenReviewAsync(reviewId);
        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            {
                "REVIEW_NOT_FOUND" => NotFound(result),
                "REVIEW_ALREADY_HIDDEN" => BadRequest(result),
                _ => BadRequest(result)
            };
        }
        return Ok(result);
    }
    //GET	/api/admin/reports/reviews
    [HttpGet("/api/admin/reports/reviews")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetReviewReport([FromQuery] Guid? productId)
    {
        var report = await _reviewService.GetReviewReportAsync(productId);
        return Ok(report);
    }
    //DELETE	/api/admin/reviews/{id}
    [HttpDelete("/api/admin/reviews/{reviewId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminDeleteReview(Guid reviewId)
    {
        var result = await _reviewService.DeleteAsync(reviewId);
        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            { 
                "REVIEW_NOT_FOUND" => NotFound(result),
                _ => BadRequest(result)
            };
        }
        return Ok(result);
    }
}