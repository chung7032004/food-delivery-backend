using Microsoft.AspNetCore.Mvc;
using FoodDelivery.DTOs;
using FoodDelivery.Service.Interfaces;
using FoodDelivery.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace FoodDelivery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShipperController : ControllerBase
    {
        private readonly IShipperService _service;
        public ShipperController(IShipperService service) { _service = service; }
        [HttpPost("confirm-pickup/{orderId}")]
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
        public async Task<IActionResult> Success(Guid orderId)
        {
            var ok = await _service.MarkSuccessAsync(orderId);
            if (!ok) return NotFound(new { message = "Order detail not found." });
            return Ok(new { message = "Đã cập nhật: Giao hàng thành công" });
        }

        [HttpPost("delivery-failed")]
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
    }
}