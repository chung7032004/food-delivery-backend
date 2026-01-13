
using FoodDelivery.Common;
using FoodDelivery.Service.Interfaces;

namespace FoodDelivery.Service.Implementations;
public class FileService : IFileService
{
    private readonly IWebHostEnvironment _environment;
    public FileService (IWebHostEnvironment webHostEnvironment)
    {
        _environment = webHostEnvironment;
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
    
    public void DeleteFile(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl)) return;
        if (!fileUrl.StartsWith("/uploads/")) return;
        if (fileUrl.EndsWith("/default.png")) return;
        var filePath = Path.Combine(_environment.WebRootPath, fileUrl.TrimStart('/'));
        if (File.Exists(filePath)) File.Delete(filePath);
    }
}