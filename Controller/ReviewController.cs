using Microsoft.AspNetCore.Mvc;
using FoodDelivery.Entities;
using FoodDelivery.Service.Interfaces;
using FoodDelivery.DTOs.Review;
using Microsoft.AspNetCore.Authorization;
using FoodDelivery.Extensions;

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
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto)
    {
        var customerId = User.GetUserId();
        var result = await _reviewService.AddReviewAsync(dto, customerId);

        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            {
                "OrderItemNotFound" => NotFound(result.Message),
                "ProductNotFound" => NotFound(result.Message),
                "AlreadyReviewed" => Conflict(result.Message),
                "Unauthorized" => Forbid(),
                _ => BadRequest(result.Message)
            };
        }

        return Ok(new { message = "Đánh giá thành công!" });
    }
}