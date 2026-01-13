namespace FoodDelivery.DTOs.Account
{
    public class AccountRequest
    {
        public string? Email {get;set;}
        public string? FullName {get; set;}
        public string? Phone {get; set;}
        public IFormFile? Avatar {get; set;}
    }
}