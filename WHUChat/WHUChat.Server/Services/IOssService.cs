using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public interface IOssService
{
    Task<string> UploadFileAsync(IFormFile file, string directoryPath = "");
    Task DeleteFileAsync(string fileUrl);
}