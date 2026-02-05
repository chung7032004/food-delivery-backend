namespace FoodDelivery.DTOs.User;

public class UserFilterModel
{
    public string? SearchQuery { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
