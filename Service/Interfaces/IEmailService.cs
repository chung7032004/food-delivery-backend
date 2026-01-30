namespace FoodDelivery.Service.Interfaces;
public interface IEmailService
{
    Task SendOtpEmailAsync(string toEmail, string otp);
}