namespace FoodDelivery.DTOs.Admin.UserManager;
public class UserAdminSummaryResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
    public int TotalOrders { get; set; } 
}