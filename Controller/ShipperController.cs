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

        public ShipperController(IShipperService service)
        {
            _service = service;
        }

        #region --- SHIPPER CORE FUNCTIONS ---

        // 1. Lấy thống kê Dashboard cho Shipper
        [HttpGet("dashboard-stats")]
        [Authorize(Roles = "Shipper")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var userId = HttpContext.User.GetUserId();
                var stats = await _service.GetShipperStatsAsync(userId);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi lấy thống kê: " + ex.Message });
            }
        }

        // 2. Lấy danh sách đơn hàng được phân công
        [HttpGet("assigned-orders")]
        [Authorize(Roles = "Shipper")]
        public async Task<IActionResult> GetAssignedOrders()
        {
            try
            {
                var userId = HttpContext.User.GetUserId();
                var orders = await _service.GetAssignedOrdersAsync(userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi lấy danh sách đơn hàng: " + ex.Message });
            }
        }

        // 3. Xem chi tiết một đơn hàng cụ thể
        [HttpGet("order/{orderId}")]
        [Authorize(Roles = "Shipper")]
        public async Task<IActionResult> GetOrderById(Guid orderId)
        {
            var userId = HttpContext.User.GetUserId();
            var order = await _service.GetOrderByIdAsync(orderId);
            if (order == null) return NotFound(new { message = "Không tìm thấy chi tiết đơn hàng" });
            
            // Get shipper to check ownership
            var shipper = await _service.GetShipperByIdAsync(userId);
            if (shipper == null) return Forbid();
            
            // Allow viewing if:
            // 1. Order is available (no shipper assigned yet) AND status is ReadyForPickup
            // 2. OR shipper owns the order
            bool canView = (order.ShipperId == null || order.ShipperId == Guid.Empty) 
                        || order.ShipperId == shipper.Id;
                        
            if (!canView) return Forbid("Bạn không có quyền xem đơn hàng này");
            
            return Ok(order);
        }

        // 4. Xác nhận đã lấy hàng từ quán
        [HttpPost("confirm-pickup/{orderId}")]
        [Authorize(Roles = "Shipper")]
        public async Task<IActionResult> Confirm(Guid orderId)
        {
            var userId = HttpContext.User.GetUserId();
            var ok = await _service.ConfirmPickUpAsync(orderId, userId);
            if (!ok) return BadRequest(new { message = "Xác nhận lấy hàng thất bại hoặc đơn không tồn tại." });
            return Ok(new { message = "Xác nhận lấy hàng thành công" });
        }

        // 5. Cập nhật giao hàng thành công
        [HttpPost("delivery-success/{orderId}")]
        [Authorize(Roles = "Shipper")]
        public async Task<IActionResult> Success(Guid orderId)
        {
            var userId = HttpContext.User.GetUserId();
            var ok = await _service.MarkSuccessAsync(orderId, userId);
            if (!ok) return BadRequest(new { message = "Không thể cập nhật trạng thái thành công. Kiểm tra quyền truy cập đơn hàng." });
            return Ok(new { message = "Đã cập nhật: Giao hàng thành công" });
        }

        // 6. Cập nhật giao hàng thất bại (kèm lý do)
        [HttpPost("delivery-failed")]
        [Authorize(Roles = "Shipper")]
        public async Task<IActionResult> Failed([FromBody] ShipperActionDTO dto)
        {
            var userId = HttpContext.User.GetUserId();
            var ok = await _service.MarkFailedAsync(dto.OrderId, dto.Reason ?? "Không có lý do", userId);
            if (!ok) return BadRequest(new { message = "Không thể cập nhật trạng thái thất bại." });
            return Ok(new { message = "Đã cập nhật: Giao hàng thất bại" });
        }

        #endregion

        #region --- ADMIN & MANAGEMENT FUNCTIONS ---

        [HttpGet("all-shippers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllShippers()
        {
            var result = await _service.GetAllShippersAsync();
            return Ok(result);
        }

        [HttpGet("detail/{userId}")]
        [Authorize(Roles = "Admin,Shipper")]
        public async Task<IActionResult> GetDetail(Guid userId)
        {
            var result = await _service.GetShipperByIdAsync(userId);
            if (result == null) return NotFound(new { message = "Không tìm thấy shipper" });
            return Ok(result);
        }

        [HttpPut("toggle-status/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleStatus(Guid userId, [FromQuery] bool isActive)
        {
            var ok = await _service.ToggleShipperStatusAsync(userId, isActive);
            if (!ok) return BadRequest(new { message = "Cập nhật trạng thái thất bại" });
            return Ok(new { message = isActive ? "Đã kích hoạt shipper" : "Đã khóa shipper" });
        }

        [HttpGet("history/{userId}")]
        [Authorize(Roles = "Admin,Shipper")]
        public async Task<IActionResult> GetHistory(Guid userId)
        {
            var requesterId = HttpContext.User.GetUserId();
            var requesterRole = HttpContext.User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
            // Shipper chỉ xem được history của chính mình, Admin xem được tất cả
            if (requesterRole != "Admin" && requesterId != userId)
                return Forbid("Bạn không có quyền xem lịch sử này");
            var result = await _service.GetShipperCompletedOrdersAsync(userId);
            return Ok(result);
        }

        [HttpPost("assign-role/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole(Guid userId)
        {
            var ok = await _service.AssignShipperRoleAsync(userId);
            if (!ok) return BadRequest(new { message = "Không thể phân quyền cho user này" });
            return Ok(new { message = "Đã chuyển user thành shipper thành công" });
        }

        [HttpPut("profile")]
        [Authorize(Roles = "Shipper")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateShipperProfileDto request)
        {
            var userId = HttpContext.User.GetUserId();
            var ok = await _service.UpdateShipperProfileAsync(userId, request);
            if (!ok) return BadRequest(new { message = "Cập nhật profile thất bại" });
            return Ok(new { message = "Cập nhật profile thành công" });
        }

        // Lấy danh sách các đơn hàng sẵn sàng để nhận
        [HttpGet("available-orders")]
        [Authorize(Roles = "Shipper")]
        public async Task<IActionResult> GetAvailableOrders()
        {
            try
            {
                var orders = await _service.GetAvailableOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi lấy danh sách đơn hàng sẵn sàng: " + ex.Message });
            }
        }

        // Shipper chấp nhận/nhận một đơn hàng
        [HttpPost("accept-order/{orderId}")]
        [Authorize(Roles = "Shipper")]
        public async Task<IActionResult> AcceptOrder(Guid orderId)
        {
            try
            {
                var userId = HttpContext.User.GetUserId();
                // AcceptOrderAsync now throws InvalidOperationException with reason when it fails
                await _service.AcceptOrderAsync(orderId, userId);
                return Ok(new { message = "Đã nhận đơn hàng thành công" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi nhận đơn hàng: " + ex.Message });
            }
        }

        #endregion
    }
}