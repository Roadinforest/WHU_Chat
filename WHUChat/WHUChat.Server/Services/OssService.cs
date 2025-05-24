using Aliyun.OSS;
using Aliyun.OSS.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;
using WHUChat.Server.Models;

public class OssService : IOssService
{
    private readonly OssConfig _ossConfig;
    private readonly IOss _client;

    public OssService(IOptions<OssConfig> ossConfigOptions)
    {
        _ossConfig = ossConfigOptions.Value;
        _client = new OssClient(_ossConfig.Endpoint, _ossConfig.AccessKeyId, _ossConfig.AccessKeySecret);
    }

    public async Task<string> UploadFileAsync(IFormFile file, string directoryPath = "")
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File cannot be null or empty.", nameof(file));
        }

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var objectKey = string.IsNullOrEmpty(directoryPath) ? fileName : $"{directoryPath.Trim('/')}/{fileName}";

        try
        {
            using (var stream = file.OpenReadStream())
            {
                _client.PutObject(_ossConfig.BucketName, objectKey, stream);
            }

            string publicUrl;
            if (_ossConfig.Endpoint.StartsWith("https://") || _ossConfig.Endpoint.StartsWith("http://"))
            {
                publicUrl = $"{_ossConfig.Endpoint.Replace("https://", $"https://{_ossConfig.BucketName}.").Replace("http://", $"http://{_ossConfig.BucketName}.")}/{objectKey}";
            }
            else
            {
                publicUrl = $"https://{_ossConfig.BucketName}.{_ossConfig.Endpoint}/{objectKey}";
            }


            return publicUrl;
        }
        catch (OssException ex)
        {
            Console.WriteLine($"OSS Error Code: {ex.ErrorCode}, Message: {ex.Message}");
            throw; 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error uploading file: {ex.Message}");
            throw; 
        }
    }

    public async Task DeleteFileAsync(string fileUrl)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
        {
            throw new ArgumentException("File URL cannot be null or empty.", nameof(fileUrl));
        }

        // 获取 objectKey，即 OSS 中存储的路径部分
        var prefix = $"https://{_ossConfig.BucketName}.{_ossConfig.Endpoint}/";
        if (!fileUrl.StartsWith(prefix))
        {
            throw new InvalidOperationException("Invalid file URL or not matching the configured OSS bucket.");
        }

        var objectKey = fileUrl.Substring(prefix.Length);

        try
        {
            _client.DeleteObject(_ossConfig.BucketName, objectKey);
        }
        catch (OssException ex)
        {
            Console.WriteLine($"OSS Error Code: {ex.ErrorCode}, Message: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting file: {ex.Message}");
            throw;
        }
    }


}