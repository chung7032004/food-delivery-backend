using FoodDelivery.DTOs.Staff;
using FoodDelivery.Entities;
using FoodDelivery.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Repositories.Implements;

public class StaffRepository : IStaffRepository
{
    private readonly FoodContext _context;

    public StaffRepository(FoodContext context)
    {
        _context = context;
    }

    public async Task<Staff?> GetStaffByUserId(Guid userId)
    {
        return await _context.Staff
            .Include(s => s.User)
            .Include(s => s.Restaurant)
            .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive);
    }

    public async Task<List<OrderDto>> GetOrdersByRestaurant(Guid restaurantId)
    {
        // Get all active orders (not completed or cancelled) for the restaurant
        // Note: Order entity doesn't have RestaurantId foreign key, so we can't filter by it
        // Staff accessing this endpoint should only see orders from their restaurant
        var orders = await _context.Orders
            .Include(o => o.OrderDetail)
            .Include(o => o.OrderItems)
            .Include(o => o.Customer)
            .Where(o => o.OrderDetail != null && 
                   (o.OrderDetail.Status == OrderStatus.Pending ||
                    o.OrderDetail.Status == OrderStatus.Confirmed ||
                    o.OrderDetail.Status == OrderStatus.Preparing ||
                    o.OrderDetail.Status == OrderStatus.ReadyForPickup))
            .AsNoTracking()
            .Select(o => new OrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderCode,
                CustomerName = o.Customer!.FullName ?? "Unknown",
                CustomerPhone = o.Customer.Phone ?? "",
                TotalAmount = o.TotalAmount,
                Status = (int)(o.OrderDetail!.Status),
                CreatedAt = o.CreatedAt,
                Items = o.OrderItems!.Select(oi => new OrderItemDto
                {
                    ProductName = oi.ProductName,
                    Quantity = oi.Quantity,
                    Price = oi.UnitPrice
                }).ToList()
            })
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders;
    }

    public async Task<List<OrderDto>> GetPendingOrdersByRestaurant(Guid restaurantId)
    {
        // Note: Order entity doesn't have RestaurantId foreign key
        // Staff should only see orders from their restaurant (validated in service layer)
        return await _context.Orders
            .Include(o => o.OrderDetail)
            .Include(o => o.OrderItems)
            .Include(o => o.Customer)
            .Where(o => o.OrderDetail != null && 
                   (o.OrderDetail.Status == OrderStatus.Pending ||
                    o.OrderDetail.Status == OrderStatus.Confirmed))
            .AsNoTracking()
            .Select(o => new OrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderCode,
                CustomerName = o.Customer!.FullName ?? "Unknown",
                CustomerPhone = o.Customer.Phone ?? "",
                TotalAmount = o.TotalAmount,
                Status = (int)(o.OrderDetail!.Status),
                CreatedAt = o.CreatedAt,
                Items = o.OrderItems!.Select(oi => new OrderItemDto
                {
                    ProductName = oi.ProductName,
                    Quantity = oi.Quantity,
                    Price = oi.UnitPrice
                }).ToList()
            })
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<OrderDto?> GetOrderDetails(Guid orderId)
    {
        return await _context.Orders
            .Where(o => o.Id == orderId)
            .Include(o => o.OrderDetail)
            .Include(o => o.OrderItems)
            .Include(o => o.Customer)
            .AsNoTracking()
            .Select(o => new OrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderCode,
                CustomerName = o.Customer!.FullName ?? "Unknown",
                CustomerPhone = o.Customer.Phone ?? "",
                TotalAmount = o.TotalAmount,
                Status = (int)(o.OrderDetail!.Status),
                CreatedAt = o.CreatedAt,
                Items = o.OrderItems!.Select(oi => new OrderItemDto
                {
                    ProductName = oi.ProductName,
                    Quantity = oi.Quantity,
                    Price = oi.UnitPrice
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateOrderStatusAsync(Guid orderId, int newStatus)
    {
        var order = await _context.Orders
            .Include(o => o.OrderDetail)
            .FirstOrDefaultAsync(o => o.Id == orderId);
        
        if (order?.OrderDetail == null) return false;

        order.OrderDetail.Status = (OrderStatus)newStatus;
        order.OrderDetail.EstimatedDeliveryTime = DateTime.UtcNow.AddMinutes(30);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Staff?> GetStaffByRestaurantIdAsync(Guid restaurantId)
    {
        return await _context.Staff
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.RestaurantId == restaurantId && s.IsActive);
    }

    public async Task<bool> ReportOrderIssueAsync(Guid orderId, string issueDescription, string issueType)
    {
        try
        {
            Console.WriteLine($"[ReportIssue] Starting - OrderId: {orderId}, Type: {issueType}");
            
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                Console.WriteLine($"[ReportIssue] Order not found: {orderId}");
                return false;
            }
            
            Console.WriteLine($"[ReportIssue] Order found: {order.OrderCode}");

            // Get all admins
            var admins = await _context.Users
                .Include(u => u.UserRoles)
                .Where(u => u.UserRoles.Any(ur => ur.Role!.Name == "Admin"))
                .ToListAsync();
            
            Console.WriteLine($"[ReportIssue] Found {admins.Count} admin users");

            // Get all active staff members (to notify them as well)
            var staffMembers = await _context.Staff
                .Include(s => s.User)
                .Where(s => s.IsActive)
                .ToListAsync();
            
            Console.WriteLine($"[ReportIssue] Found {staffMembers.Count} active staff members");

            if (admins.Count == 0 && staffMembers.Count == 0)
            {
                Console.WriteLine("[ReportIssue] WARNING: No admins or staff found!");
                return false;
            }

            // Convert issue type to Vietnamese
            var issueTypeVi = ConvertIssueTypeToVietnamese(issueType);

            // Send notifications to admins
            foreach (var admin in admins)
            {
                var notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = admin.Id,
                    Title = $"Vấn đề đơn hàng {order.OrderCode}",
                    Message = $"Loại vấn đề: {issueTypeVi}\nMô tả: {issueDescription}",
                    Type = NotificationType.SYSTEM,
                    RelatedOrderId = orderId,
                    Link = $"/admin/orders/{orderId}",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };
                _context.Notifications.Add(notification);
                Console.WriteLine($"[ReportIssue] Added notification for admin: {admin.Id}");
            }

            // Send notifications to staff members
            foreach (var staff in staffMembers)
            {
                var notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = staff.UserId,
                    Title = $"Vấn đề đơn hàng {order.OrderCode}",
                    Message = $"Loại vấn đề: {issueTypeVi}\nMô tả: {issueDescription}",
                    Type = NotificationType.SYSTEM,
                    RelatedOrderId = orderId,
                    Link = $"/admin/orders/{orderId}",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };
                _context.Notifications.Add(notification);
                Console.WriteLine($"[ReportIssue] Added notification for staff: {staff.UserId}");
            }

            var result = await _context.SaveChangesAsync();
            Console.WriteLine($"[ReportIssue] SaveChangesAsync returned: {result} rows affected");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ReportIssue] ERROR: {ex.Message}");
            Console.WriteLine($"[ReportIssue] StackTrace: {ex.StackTrace}");
            return false;
        }
    }

    private string ConvertIssueTypeToVietnamese(string issueType)
    {
        return issueType switch
        {
            "OutOfStock" => "Hết hàng",
            "Damaged" => "Hỏng hàng",
            "Wrong" => "Sai đơn",
            "Other" => "Khác",
            _ => issueType
        };
    }
}
