using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FoodDelivery.Common;
using FoodDelivery.DTOs;
using FoodDelivery.Entities;
using FoodDelivery.Repositories;
using FoodDelivery.Repositories.Interfaces;
using FoodDelivery.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
namespace FoodDelivery.Service.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _tokenSecretKey;
        private readonly IEmailService _emailService;
        private readonly IPasswordResetOtpRepository _passwordResetOtpRepository;
        private readonly ILogger<AuthService> _logger;
        public AuthService(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository,
                            IRoleRepository roleRepository,IUnitOfWork unitOfWork, IConfiguration configuration,
                            IEmailService emailService, IPasswordResetOtpRepository passwordResetOtpRepository,
                            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _tokenSecretKey = configuration["TokenSecretKey"] ?? "default_secret_key_2025_food_delivery";
            _unitOfWork = unitOfWork;
            _roleRepository = roleRepository;
            _emailService = emailService;
            _passwordResetOtpRepository = passwordResetOtpRepository;
            _logger = logger;
        }
        public async Task <Result> RegisterUserAsync(string email, string password, string fullName, string phone)
        {
            if(string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(phone))
            {
                return Result.Failure("INVALID_INPUT","Thông tin không hợp lệ");
            }
            // Validate phone number: phải bắt đầu 0 và đủ 10 chữ số
            if(!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^0\d{9}$"))
            {
                return Result.Failure("INVALID_PHONE", "Số điện thoại phải bắt đầu bằng 0 và có đúng 10 chữ số");
            }
            if(await _userRepository.IsEmailExistsAsync(email.Trim()))
            {
                return Result.Failure("EMAIL_EXITS", "Email đã tồn tại");
            }
            var hmac = new HMACSHA512();
            var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            var user = new User
            {
                Email = email.Trim(),
                FullName = fullName,
                Phone = phone,
                PasswordHash = passwordHash,
                PasswordSalt = hmac.Key,
                CreatedAt = DateTime.UtcNow,
            };
            await _userRepository.AddAsync(user);
            var defaultRole = await _roleRepository.GetByNameAsync("Customer");
            if(defaultRole == null)
            {
                return Result.Failure("ROLE_INVALID", "Role Customer không tồn tại");
            }
            else
            {
                var userRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = defaultRole.Id
                };
                await _userRepository.AddUserRoleAsync(userRole);
                await _unitOfWork.SaveChangesAsync();
            }
            return Result.Success();
        }
        public async Task <Result<LoginResponse>> AuthenticateUserAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return Result<LoginResponse>.Failure("INVALID_INPUT","Email hoặc mật khẩu trống");
            }
            
            var user = await _userRepository.GetByEmailAsync(email);
            
            var fakeHash = new byte[64];
            var fakeSalt = new byte[128];

            if (user == null)
            {
                VerifyPasswordHash(password, fakeHash, fakeSalt); // GIẢ
                return Result<LoginResponse>.Failure("USER_NOT_FOUND", "User không tồn tại");
            }

            if (!user.IsActive)
            {
                return Result<LoginResponse>.Failure("USER_INACTIVE","Tài khoảng bị khóa");
            }
            if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return Result<LoginResponse>.Failure("INVALID_PASSWORD","Sai email hoặc mật khẩu");
            }
            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            var refresh = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false, 
            };
            
            // Cập nhật LastLogin khi đăng nhập thành công
            await _userRepository.UpdateLastLoginAsync(user.Id);
            
            await _refreshTokenRepository.AddAsync(refresh);
            await _unitOfWork.SaveChangesAsync();
            return Result<LoginResponse>.Success(new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }
        // CHỐNG TIMING ATTACK – BẮT BUỘC PHẢI CÓ
        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedKey)
        {
            if (password == null) return false;
            if (storedHash == null || storedKey == null) return false;

            using var hmac = new HMACSHA512(storedKey);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            return CryptographicOperations.FixedTimeEquals(computedHash, storedHash); //Luôn mất đúng một khoảng thời gian cố định dù có bao nhiêu ký tự đúng
        }
        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim("name", user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("jti", Guid.NewGuid().ToString())// chống replay
            };
            if(user.UserRoles != null)
            {
                foreach(var userRole in user.UserRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
                }
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer : "foodApi",
                audience: "foodApi",
                claims : claims,
                expires : DateTime.UtcNow.AddMinutes(15),
                signingCredentials : creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        } 
        public async Task<Result<RefreshTokenResponse>> RefreshAccessTokenAsync(string refreshToken)
        {
            var refresh = await _refreshTokenRepository.GetRefreshTokenAsync(refreshToken);
            if(refresh == null || refresh.IsRevoked || refresh.ExpiresAt < DateTime.UtcNow)
            {
                return Result<RefreshTokenResponse>.Failure("REFRESH_INVALID","Refresh token không hợp lệ");
            }

            var user = await _userRepository.GetUserByIdWithRoleAsync(refresh.UserId);
            if(user == null)
            {
                return Result<RefreshTokenResponse>.Failure("USER_NOT_FOUND", "User không tồn tại");
            }
            refresh.IsRevoked = true;
            refresh.RevokedAt = DateTime.UtcNow;

            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false,
                UserId = user.Id,
            };
            await _refreshTokenRepository.AddAsync(newRefreshToken);
            await _unitOfWork.SaveChangesAsync();
            return  Result<RefreshTokenResponse>.Success(new RefreshTokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            });
        }
        public async Task<Result> LogoutAsync (Guid userId,string refreshToken)
        {
            var refresh = await _refreshTokenRepository.GetRefreshTokenAsync(refreshToken);
            if(refresh == null || refresh.UserId != userId)
            {
                return Result.Failure("REFRESH_NOT_FOUND", "Refresh token không hợp lệ");
            }
            if(refresh.ExpiresAt < DateTime.UtcNow ||refresh.IsRevoked)
            {
                return Result.Success();
            }

            refresh.IsRevoked = true;
            refresh.RevokedAt = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(refresh);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        public async Task<Result> ChangePasswordAsync (Guid userId,ChangePasswordRequest changePasswordRequest)
        {
            if(changePasswordRequest.NewPassword != changePasswordRequest.ConfirmNewPassword)
            {
                return Result.Failure("PASSWORD_MISMATCH", "Mật khẩu xác nhận không khớp");
            }
            if(changePasswordRequest.NewPassword == changePasswordRequest.OldPassword)
            {
                return Result.Failure(
                    "PASSWORD_SAME_AS_OLD",
                    "Mật khẩu mới không được trùng mật khẩu cũ"
                );
            }
            var user = await _userRepository.GetByIdAsync(userId);
            if(user == null)
            {
                return Result.Failure("USER_NOT_FOUND", "Không tìm thấy người dùng");
            }
            if(!VerifyPasswordHash(changePasswordRequest.OldPassword,user.PasswordHash, user.PasswordSalt))
            {
                return Result.Failure("INVALID_PASSWORD", "Mật khẩu cũ không đúng");
            }
            var hmac = new HMACSHA512();
            await _refreshTokenRepository.RevokeAllAsync(userId);
            var newPasswordSalt = hmac.Key;
            var newPasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(changePasswordRequest.NewPassword));
            await _userRepository.ChangePasswordAsync(userId,newPasswordHash,newPasswordSalt);
            return Result.Success();
        }
        public async Task<Result> SendOtpAsync (SendOtpRequest request)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(request.Email);
                if(user == null)
                {
                    return Result.Failure("EMAIL_INVALID","Không tồn tại tài khoảng.");
                }
                var oldOpts = await _passwordResetOtpRepository.GetAllByUserId(user.Id);
                await _passwordResetOtpRepository.DeleteRangeAsync(oldOpts);
                var otp = new Random().Next(100000,999999).ToString();
                var hmac = new HMACSHA512();
                var otpHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(otp));
                var reset = new PasswordResetOtp{
                    UserId = user.Id,
                    Email = user.Email,
                    OtpHash = otpHash,
                    OtpSalt = hmac.Key,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                    RetryCount = 0
                };
                await _passwordResetOtpRepository.AddAsync(reset);
                await _unitOfWork.SaveChangesAsync();
                try{
                await _emailService.SendOtpEmailAsync(request.Email,otp);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Lỗi gửi email OTP cho {Email}", request.Email);
                    return Result.Failure("EMAIL_SERVICE_ERROR", "Không thể gửi email lúc này. Vui lòng thử lại sau.");
                }
                return Result.Success();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Lỗi Database khi lưu OTP cho {Email}", request.Email);
                return Result.Failure("DATABASE_ERROR", "Lỗi hệ thống khi xử lý yêu cầu.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định trong SendOtpAsync cho {Email}", request.Email);
                return Result.Failure("SERVER_ERROR", "Đã xảy ra lỗi ngoài ý muốn.");
            }
        }
        public async Task<Result> ResetPasswordAsync(string email, string otp, string newPassword)
        {
            try{
                var record = await _passwordResetOtpRepository.GetByEmailAsync(email);
                if(record == null)
                {
                    return Result.Failure("OTP_INVALID","Yêu cầu khôi phục không tồn tại");
                }
                if (record.RetryCount >= 5)
                {
                    //Nếu sai quá nhiều, xóa luôn OTP để bắt người dùng tạo yêu cầu mới
                    await _passwordResetOtpRepository.DeleteAsync(record);
                    await _unitOfWork.SaveChangesAsync();
                    return Result.Failure("OTP_LOCKED", "Bạn đã nhập sai quá nhiều lần. Vui lòng yêu cầu mã mới.");
                }
                if(record.ExpiresAt  < DateTime.UtcNow)
                {
                    return Result.Failure("OTP_EXPIRED", "Mã OTP đã hết hạn.");
                }
                if(!VerifyPasswordHash(otp, record.OtpHash, record.OtpSalt))
                {
                    record.RetryCount++;
                    await _unitOfWork.SaveChangesAsync();
                    return Result.Failure("OTP_INVALID","OTP không đúng.");
                }
                var hmac = new HMACSHA512();
                await _refreshTokenRepository.RevokeAllAsync(record.UserId);
                var newPasswordSalt = hmac.Key;
                var newPasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(newPassword));
                await _userRepository.ChangePasswordAsync(record.UserId,newPasswordHash,newPasswordSalt);
                await _passwordResetOtpRepository.DeleteAsync(record);
                await _unitOfWork.SaveChangesAsync();
                return Result.Success();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Lỗi Database khi cập nhật mật khẩu");
                return Result.Failure("DATABASE_ERROR", "Lỗi hệ thống khi xử lý yêu cầu.");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định {ex}",ex);
                return Result.Failure("SERVER_ERROR", "Đã xảy ra lỗi ngoài ý muốn.");
            }
        }
    }
}