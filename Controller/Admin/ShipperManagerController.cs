using FoodDelivery.DTOs.Shipper1;
using FoodDelivery.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Controllers.Admin;
[ApiController]
[Route("api/admin/shippers")]
[Authorize(Roles = "Admin")]
public class ShippersManagerController : ControllerBase
{
    private readonly IShippers1Service _service;
    public ShippersManagerController( IShippers1Service service)
    {
        _service = service;
    }
    [HttpGet()]
    public async Task<IActionResult> GetAll([FromQuery] int page, [FromQuery] int pageSize)
    {
        var result = await _service.GetAllShippersAsync(page, pageSize);
        return Ok(result);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            {
                "_SHIPPER_NOT_FOUND" => NotFound(result),
                _ => BadRequest(result),
            };
        }
         return Ok(result);
    }
    [HttpPost("{userId}/register-shipper")]
    public async Task<IActionResult> Create(Guid userId)
    {
        var result = await _service.RegisterShipperAsync(userId);
        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            {
                "USER_NOT_FOUND" => NotFound(result),
                "ALREADY_SHIPPER" =>BadRequest(result),
                "ROLE_INVALID" =>BadRequest(result),
                _ =>StatusCode(500, result)
            };
        }
        return Ok(result);
    } 
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id,[FromBody] UpdateStatusRequest request)
    {
        var result = await _service.UpdateStatusAsync(id,request.IsActive);
        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            {
                "SHIPPER_NOT_FOUND" => NotFound(result),
                _ =>StatusCode(500, result)
            };
        }
        return Ok(result);
    }
    [HttpGet("{id}/history")]
    public async Task<IActionResult> GetDeliveryHistory(Guid id,[FromQuery]int page,[FromQuery] int pageSize)
    {
        var result = await _service.GetOrderByShipperIdAsync(id,page,pageSize);
        return Ok(result);
    }
}