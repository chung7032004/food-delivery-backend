namespace FoodDelivery.DTOs.Account
{
    public class AccountResponse
    {
        public Guid Id {get;set;}
        public string? FullName {get; set;}
        public string? Email {get; set;}
        public string? Phone {get; set;} 
        public string? AvatarUrl {get; set;}
        public List<string> Roles { get; set; } = new List<string>();
    }
}