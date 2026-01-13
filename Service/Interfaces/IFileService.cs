
using FoodDelivery.Common;

namespace FoodDelivery.Service.Interfaces;
public interface IFileService
{
    Task <Result<string>> SaveFileAsync(IFormFile file, string folderName);
    void DeleteFile(string fileUrl);
}