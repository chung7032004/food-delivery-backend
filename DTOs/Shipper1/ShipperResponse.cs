public class ShipperResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public bool IsActive { get; set; }
    public bool IsAvailable { get; set; } 
    public int TotalDeliveredOrders { get; set; } 
    public string FullName {get; set;} = string.Empty;
    public string Email {get; set;} = string.Empty;
    public string Phone {get; set;} = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } 
    public DateTime? UpdatedAt { get; set; }
}