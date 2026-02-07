using FoodDelivery.Service.Interfaces;
using System.Net;
using System.Net.Mail;
namespace FoodDelivery.Service.Implementations;
public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;
    public EmailService(IConfiguration config,ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }
     public async Task SendOtpEmailAsync(string toEmail, string otp)
    {
        using var smtp = new SmtpClient("smtp.gmail.com", 587)
        {
            Credentials = new NetworkCredential
            (
                _config["Smtp:Email"],
                _config["Smtp:Password"]
            ),
            EnableSsl = true
        };
        using var mail = CreateMailMessage(toEmail,otp);
        try
        {
            await smtp.SendMailAsync(mail);
        }catch(SmtpException ex)
        {
            _logger.LogError(ex, "Lỗi SMTP khi gửi OTP đến {Email}",toEmail);
        }
    }
    private MailMessage CreateMailMessage(string toEmail, string otp)
        {
            var senderEmail = _config["Smtp:Email"] 
            ?? throw new InvalidOperationException("Smtp:Email is not configured in appsettings.json");
            var mail = new MailMessage
            {
                From = new MailAddress(senderEmail, "FoodDelivery Support"),
                Subject = "Mã OTP khôi phục mật khẩu",
                Body = $@"
                    <div style='font-family: sans-serif; border: 1px solid #eee; padding: 20px;'>
                        <h2 style='color: #333;'>Xác nhận thay đổi mật khẩu</h2>
                        <p>Chào bạn, đây là mã OTP của bạn:</p>
                        <div style='font-size: 24px; font-weight: bold; color: #007bff; padding: 10px; background: #f8f9fa;'>
                            {otp}
                        </div>
                        <p style='font-size: 0.8em; color: #666;'>Mã này sẽ hết hạn sau 10 phút.</p>
                    </div>",
                IsBodyHtml = true
            };
            mail.To.Add(toEmail);
            return mail;
        }
}