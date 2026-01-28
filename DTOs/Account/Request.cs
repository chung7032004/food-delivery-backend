namespace FoodDelivery.DTOs.Account
{
    public class AccountRequest
    {
        public string? Email {get;set;}
        public string? FullName {get; set;}
        public string? Phone {get; set;}
        public IFormFile? Avatar {get; set;}
    }

    public class UpdateProfileRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}