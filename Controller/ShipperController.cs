using Microsoft.AspNetCore.Mvc;
using FoodDelivery.DTOs;
using FoodDelivery.Service.Interfaces;
using FoodDelivery.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace FoodDelivery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipperController : ControllerBase
    {
        private readonly IShipperService _service;
        public ShipperController(IShipperService service) { _service = service; }
        [HttpPost("confirm-pickup/{orderId}")]
        [Authorize(Roles ="Shipper")]
        public async Task<IActionResult> Confirm(Guid orderId)
        {
            Guid shipperId;
            try
            {
                shipperId = HttpContext.User.GetUserId();
            }
            catch
            {
                return Unauthorized(new { message = "Không tìm thấy shipper trong token." });
            }

            var ok = await _service.ConfirmPickUpAsync(orderId, shipperId);
            if (!ok) return NotFound(new { message = "Order detail not found." });
            return Ok(new { message = "Xác nhận lấy hàng thành công" });
        }

        [HttpPost("delivery-success/{orderId}")]
        [Authorize(Roles ="Shipper")]
        public async Task<IActionResult> Success(Guid orderId)
        {
            var ok = await _service.MarkSuccessAsync(orderId);
            if (!ok) return NotFound(new { message = "Order detail not found." });
            return Ok(new { message = "Đã cập nhật: Giao hàng thành công" });
        }

        [HttpPost("delivery-failed")]
        [Authorize(Roles ="Shipper")]
        public async Task<IActionResult> Failed([FromBody] ShipperActionDTO dto)
        {
            Guid? shipperId = null;
            try
            {
                shipperId = HttpContext.User.GetUserId();
            }
            catch
            {
                // ignore, cancelledBy remains null
            }

            var ok = await _service.MarkFailedAsync(dto.OrderId, dto.Reason ?? "Không có lý do", shipperId);
            if (!ok) return NotFound(new { message = "Order detail not found." });
            return Ok(new { message = "Đã cập nhật: Giao hàng thất bại" });
        }
        [HttpGet("all-shippers")]
        [Authorize(Roles = "Admin")] // Chỉ Admin mới được xem danh sách
        public async Task<IActionResult> GetAllShippers()
        {
            var result = await _service.GetAllShippersAsync();
            return Ok(result);
        }

        // 3. Xem chi tiết shipper
        [HttpGet("detail/{userId}")]
        public async Task<IActionResult> GetDetail(Guid userId)
        {
            var result = await _service.GetShipperByIdAsync(userId);
            if (result == null) return NotFound(new { message = "Không tìm thấy shipper" });
            return Ok(result);
        }

        // 4. Kích hoạt / khóa shipper
        [HttpPut("toggle-status/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleStatus(Guid userId, [FromQuery] bool isActive)
        {
            var ok = await _service.ToggleShipperStatusAsync(userId, isActive);
            if (!ok) return BadRequest(new { message = "Cập nhật trạng thái thất bại" });
            return Ok(new { message = isActive ? "Đã kích hoạt shipper" : "Đã khóa shipper" });
        }

        // 5. Xem lịch sử giao hàng của 1 shipper
        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetHistory(Guid userId)
        {
            var result = await _service.GetShipperHistoryAsync(userId);
            return Ok(result);
        }

        // 1. Tạo shipper từ user (Phân quyền)
        [HttpPost("assign-role/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole(Guid userId)
        {
            var ok = await _service.AssignShipperRoleAsync(userId);
            if (!ok) return BadRequest(new { message = "Không thể phân quyền cho user này" });
            return Ok(new { message = "Đã chuyển user thành shipper thành công" });
        }
        
    }
    
    
}