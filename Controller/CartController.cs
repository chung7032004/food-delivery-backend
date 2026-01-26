using FoodDelivery.Common;
using FoodDelivery.DTOs.Cart;
using FoodDelivery.Extensions;
using FoodDelivery.Service.Implementations;
using FoodDelivery.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodDelivery.Controllers;

[ApiController]
[Route("api/[Controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    public CartController (ICartService cartService)
    {
        _cartService = cartService;
    }
    
    [HttpGet]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetCart([FromQuery] int page = 1, int pageSize = 10)
    {
        if(!User.TryGetUserId(out Guid userId))
        {
             return Unauthorized(Result<PagedResponse<CartItemDto>>.Failure("INVALID_TOKEN","Token không hợp lệ hoặc thiếu UserId."));
        }
        var result = await _cartService.GetCartAsync(userId, page, pageSize);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return NotFound(result);
    }
    
    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> AddCartItem([FromBody] CartItemRequest cartItemRequest)
    {
        if(!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(Result.Failure("INVALID_TOKEN","Token không hợp lệ hoặc thiếu UserId."));
        }
        var result = await _cartService.AddCartItemAsync(userId,cartItemRequest);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return result.ErrorCode switch
        {
            "INVALID_QUANTITY" => BadRequest(result),
            "PRODUCT_NOT_FOUND" => NotFound(result),
            "PRODUCT_UNAVAILABLE" => BadRequest(result),
            _ => BadRequest(result)
        };
    }

    [HttpPatch("item/{productId}")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> UpdateQuantity(Guid productId, [FromBody] UpdateQuantityRequest request)
    {
        if(!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(Result.Failure("INVALID_TOKEN","Token không hợp lệ hoặc thiếu UserId."));
        }
        var result = await _cartService.UpdateQuantityCartItemAsync(userId,productId,request.Quantity);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return result.ErrorCode switch
        {
            "INVALID_QUANTITY" => BadRequest(result),
            "PRODUCT_UNAVAILABLE" => BadRequest(result),
            "CART_NOT_FOUND" => NotFound(result),
            "PRODUCT_NOT_FOUND" => NotFound(result),
            "CART_ITEM_NOT_FOUND" => NotFound(result),
            _ => BadRequest(result)
        };
    }
    
    [HttpDelete("item/{productId}")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> DeleteCartItem(Guid productId)
    {
        if(!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(Result.Failure("INVALID_TOKEN","Token không hợp lệ hoặc thiếu UserId."));
        }
        var result = await _cartService.DeleteCartItemAsync(userId,productId);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return result.ErrorCode switch
        {
            "CART_NOT_FOUND" => NotFound(result),
            "CART_ITEM_NOT_FOUND" => NotFound(result),
            _ => BadRequest(result)
        };
    }

    [HttpDelete("items")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> DeleteCartItemRange([FromBody] DeleteCartItemsDto request)
    {
        if(!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(Result.Failure("INVALID_TOKEN","Token không hợp lệ hoặc thiếu UserId."));
        }
        var result = await _cartService.DeleteCartItemRangeAsync(userId,request.ProductIds);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return result.ErrorCode switch
        {
            "EMPTY_PRODUCT_IDS" => BadRequest(result),
            "CART_NOT_FOUND" => NotFound(result),
            "CART_ITEM_NOT_FOUND" => NotFound(result),
            _ => BadRequest(result)
        };
    }
    [HttpDelete()]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> DeleteCart()
    {
        if(!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(Result.Failure("INVALID_TOKEN","Token không hợp lệ hoặc thiếu UserId."));
        }
        var result = await _cartService.DeleteCartAsync(userId);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return NotFound(result);
    }

    [HttpPost("save")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> SaveCart([FromBody] SaveCartRequest? request)
    {
        if(!User.TryGetUserId(out Guid userId))
        {
            return Unauthorized(Result.Failure("INVALID_TOKEN","Token không hợp lệ hoặc thiếu UserId."));
        }
        
        // Handle null request
        if (request == null)
        {
            request = new SaveCartRequest { Items = new() };
        }
        
        var result = await _cartService.SaveCartAsync(userId, request);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }
}