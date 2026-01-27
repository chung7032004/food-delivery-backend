
using FoodDelivery.Common;
using FoodDelivery.Service.Interfaces;

namespace FoodDelivery.Service.Implementations;
public class FileService : IFileService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<FileService> _logger;
    public FileService (IWebHostEnvironment webHostEnvironment , ILogger<FileService> logger)
    {
        _environment = webHostEnvironment;
        _logger = logger;
    }
    public async Task <Result<string>> SaveFileAsync(IFormFile  file, string folderName)
    {
        if(file == null || file.Length == 0)
        {
            return Result<string>.Failure("FILE_INVALID","File không hợp lệ hoặc trống");
        }

        if (file.Length > 2 * 1024 * 1024)
        {
            return Result<string>.Failure("FILE_TOO_LARGE","Dung lượng ảnh không vượt quá 2MB");
        }

        var allowExtension = new[] {".jpg", ".jpeg", ".png", ".webp"};
        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        if (!allowExtension.Contains(fileExtension))
        {
            return Result<string>.Failure("FILE_EXTENSION_NOT_ALLOWED","Chỉ cho phép upload các định dạng: .jpg, .jpeg, .png, .webp");
        }
        
        if (!file.ContentType.StartsWith("image/"))
        {
            return Result<string>.Failure(
                "FILE_TYPE_INVALID",
                "File upload không phải là ảnh"
            );
        }
        try
        {
            var contentPath = _environment.WebRootPath;
            var path = Path.Combine(contentPath,"uploads",folderName);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var fileNameWithPath = Path.Combine(path,fileName);

            using var stream = new FileStream(fileNameWithPath, FileMode.Create);
            await file.CopyToAsync(stream);

            var fileUrl = $"/uploads/{folderName}/{fileName}";
            return Result<string>.Success(fileUrl);
        }
        catch(IOException ex)
        {
            return Result<string>.Failure("DISK_ERROR",$"Lỗi hệ thống khi lưu file: {ex.Message}");
        }
        catch(UnauthorizedAccessException)
        {
            return Result<string>.Failure("PERMISSION_DENIED", "Server không có quyền ghi vào thư mục lưu trữ.");
        }
        catch(Exception ex)
        {
            return Result<string>.Failure("SERVER_ERROR", $"Đã xảy ra lỗi: {ex.Message}");
        }
        
    }
    
    public void DeleteFile(string fileUrl)
    {
        try
        {
            if (string.IsNullOrEmpty(fileUrl)) return;
            if (!fileUrl.StartsWith("/uploads/")) return;
            if (fileUrl.EndsWith("/default.png")) return;
            var filePath = Path.Combine(_environment.WebRootPath, fileUrl.TrimStart('/'));
            if (File.Exists(filePath)) File.Delete(filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Lỗi khi xóa file tại: {FileUrl}. Lý do: {Message}", fileUrl, ex.Message);
        }
        
    }

}