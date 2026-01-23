using System.Security.Claims;
using FoodDelivery.Common;
using FoodDelivery.DTOs.Order;
using FoodDelivery.Entities;
using FoodDelivery.Extensions;
using FoodDelivery.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    [HttpPost("buy-now")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> CreateOrderBuyNow([FromBody] BuyNowRequestDto request)
    {
        if(!User.TryGetUserId(out Guid customerId))
        {
            return Unauthorized(Result<CreateOrderResponseDto>.Failure("INVALID_TOKEN","Phiên dùng không hợp lệ."));
        }
        var result = await _orderService.BuyNowAsync(customerId,request);
        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            {
                "INVALID_QUANTITY" =>BadRequest(result),
                "PRODUCT_NOT_FOUND" => NotFound(result),
                "ADDRESS_NOT_FOUND" => NotFound(result),
                "PRODUCT_UNAVAILABLE" => BadRequest(result),
                "TOO_FAR" => BadRequest(result),
                "RESTAURANT_NOT_CONFIGURED" =>Conflict(result),
                _ => BadRequest(result),
            };
        }

        return CreatedAtAction(nameof(GetOrderDetail),new { orderId = result.Data!.OrderId},result);
    }

    [HttpPost("checkout")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> CheckoutCartItems([FromBody] CheckoutRequestDto request)
    {
        if(!User.TryGetUserId(out Guid customerId))
        {
            return Unauthorized(Result<CreateOrderResponseDto>.Failure("INVALID_TOKEN","Phiên dùng không hợp lệ."));
        }
        var result = await _orderService.CheckoutAsync(customerId,request);
        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            {
                  
                "CART_EMPTY" => BadRequest(result),
                "ADDRESS_NOT_FOUND" => NotFound(result),
                "INVALID_QUANTITY" => BadRequest(result),
                "FORBIDDEN" => Forbid(),
                "PRODUCT_UNAVAILABLE" => BadRequest(result),
                "TOO_FAR" => BadRequest(result),
                "RESTAURANT_NOT_CONFIGURED" =>Conflict(result),
                _ => BadRequest(result),
            };
        }
        return CreatedAtAction(nameof(GetOrderDetail),new { orderId = result.Data!.OrderId},result);
    }

    [HttpGet("history")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMyOrderHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if(!User.TryGetUserId(out Guid customerId))
        {
            return Unauthorized(Result<CreateOrderResponseDto>.Failure("INVALID_TOKEN","Phiên dùng không hợp lệ."));
        }
         var result = await _orderService.GetMyOrderAsync(customerId, page, pageSize);
        if (!result.IsSuccess)
            return BadRequest(result); 
        return Ok(result);
    }
    [HttpGet("{orderId}")]
    [Authorize(Roles = "Admin,Customer")]
    public async Task<IActionResult> GetOrderDetail(Guid orderId)
    {
        if(!User.TryGetUserId(out Guid customerId))
        {
            return Unauthorized(Result<OrderDetailResponse>.Failure("INVALID_TOKEN","Phiên dùng không hợp lệ."));
        }
        var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
        
        var result = await _orderService.GetOrderDetailAsync(customerId,roles,orderId);
        if (!result.IsSuccess)
            return result.ErrorCode switch
            {
                "ORDER__NOT_FOUND" => NotFound(result),
                "ORDER__FORBIDDEN" => BadRequest(result),
                _ => BadRequest(result)
            };
        return Ok(result);
    }
    [HttpPost("{orderId}/cancel")]
    [Authorize(Roles ="Customer")]
    public async Task<IActionResult> CancelOrderByCustomer(Guid orderId, [FromBody] CancelOrderRequestDto request)
    {
        if(!User.TryGetUserId(out Guid customerId))
        {
            return Unauthorized(Result.Failure("INVALID_TOKEN","Phiên dùng không hợp lệ."));
        }
        var result = await _orderService.CancelOrderByCustomerAsync(customerId,orderId,request);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
    [HttpPost("{orderId}/respond-proposal")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> RespondToProposal(Guid orderId , [FromBody] OrderProposalResponseDto request)
    {
        if(!User.TryGetUserId(out Guid customerId))
        {
            return Unauthorized(Result.Failure("INVALID_TOKEN","Phiên dùng không hợp lệ."));
        }
        var result = await _orderService.RespondToProposalAsync(customerId,orderId,request);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return result.ErrorCode switch
        {
            "ORDER_NOT_FOUND" => NotFound(result),
            "INVALID_STATUS" => BadRequest(result),
            "DATA_ERROR" => BadRequest(result),
            "EMPTY_ORDER" =>BadRequest(result),
            _ => BadRequest(result)
        };
    }
    
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAdminOrder([FromQuery] OrderFilterModel filter)
    {
        var result = await _orderService.GetOrderAdminAsync(filter);
        return Ok(result);
    }
    [HttpPost("api/admin/order/{orderId}/confirm")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ConfirmOrder(Guid orderId)
    {
        if(!User.TryGetUserId(out Guid adminId))
        {
            return Unauthorized(Result.Failure("INVALID_TOKEN","Phiên dùng không hợp lệ."));
        }
        var result = await _orderService.ConfirmOrderAsync(adminId,orderId);
        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            {
                "ORDER_NOT_FOUND" => NotFound(result),
                "INVALID_STATUS" => BadRequest(),
                _=> BadRequest(result),
            };
        }
        return Ok(result);
    }
    [HttpPost("api/admin/order/{orderId}out-of-stock")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> OutOfStock(Guid orderId, [FromBody] OutOfStockRequest request)
    {
        if(!User.TryGetUserId(out Guid adminId))
        {
            return Unauthorized(Result.Failure("INVALID_TOKEN","Phiên dùng không hợp lệ."));
        }
        var result = await _orderService.OutOfStockAsync(adminId,orderId,request);
        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            {
                "ORDER_NOT_FOUND" => NotFound(result),
                "NO_ITEMS_FOUND" => NotFound(result),
                "INVALID_STATUS" => BadRequest(result),
                _ => BadRequest(result)
            };
        }
        return Ok(result);
    }
    [HttpPost("api/admin/order/{orderId}/start-preparing")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> StartPreparing(Guid orderId)
    {
        if(!User.TryGetUserId(out Guid adminId))
        {
            return Unauthorized(Result.Failure("INVALID_TOKEN","Phiên dùng không hợp lệ."));
        }
        var result = await _orderService.StartPreparingAsync(adminId,orderId);
        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            {
                "ORDER_NOT_FOUND" => NotFound(result),
                "INVALID_STATUS" => BadRequest(result),
                _ => BadRequest(result)
            };
        }
        return Ok(result);
    }
    [HttpPost("api/admin/order/{orderId}/cancel")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> MarkAsReady(Guid orderId, [FromBody] CancelOrderRequestDto request)
    {
        if(!User.TryGetUserId(out Guid adminId))
        {
            return Unauthorized(Result.Failure("INVALID_TOKEN","Phiên dùng không hợp lệ."));
        }
        var result = await _orderService.CancelOrderByAdminAsync(adminId,orderId,request);
        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            {
                "ORDER_NOT_FOUND" => NotFound(result),
                "INVALID_STATUS" => BadRequest(result),
                _ => BadRequest(result)
            };
        }
        return Ok(result);
    }
}