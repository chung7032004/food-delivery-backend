namespace FoodDelivery.DTOs.Staff;

public class OrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = "";
    public string CustomerName { get; set; } = "";
    public string CustomerPhone { get; set; } = "";
    public decimal TotalAmount { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public string ProductName { get; set; } = "";
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class StaffStatsDto
{
    public int TotalOrdersToday { get; set; }
    public int PreparingCount { get; set; }
    public int ReadyCount { get; set; }
    public int IssueCount { get; set; }
}

public class UpdateOrderStatusRequest
{
    public int Status { get; set; }
}

public class ReportIssueRequest
{
    public string IssueDescription { get; set; } = "";
    public string IssueType { get; set; } = ""; // OutOfStock, Damaged, etc.
}

public class StaffProfileDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RestaurantId { get; set; }
    public string Position { get; set; } = "";
    public bool IsActive { get; set; }
    public string RestaurantName { get; set; } = "";
    public string UserName { get; set; } = "";
}
