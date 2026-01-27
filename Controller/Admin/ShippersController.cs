using FoodDelivery.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Controllers.Admin;
[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin")]
public class ShippersController : ControllerBase
{
    private readonly IShippers1Service _service;
    public ShippersController( IShippers1Service service)
    {
        _service = service;
    }
    [HttpGet()]
    public async Task<IActionResult> GetAll([FromHeader] int page, [FromHeader] int pageSize)
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
    [HttpPost]
    public async Task<IActionResult> Create()
    {
        return Ok();
    } 
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id,[FromBody] bool IsActive)
    {
        return Ok();
    }
    [HttpGet("{id}/history")]
    public async Task<IActionResult> GetDeliveryHistory(Guid id)
    {
        return Ok();
    }
}

// GET api/admin/shippers: Danh sách tất cả shipper.

// GET api/admin/shippers/{id}: Xem chi tiết 1 shipper.

// POST api/admin/shippers: Tạo shipper từ User có sẵn.

// PUT api/admin/shippers/{id}/status: Kích hoạt/Khóa shipper.

// GET api/admin/shippers/{id}/history: Xem lịch sử giao hàng.