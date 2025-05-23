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

    /// <summary>
    /// Uploads a file to Alibaba Cloud OSS.
    /// </summary>
    /// <param name="file">The file to upload.</param>
    /// <param name="directoryPath">Optional directory path within the bucket (e.g., "images/avatars").</param>
    /// <returns>The public URL of the uploaded file.</returns>
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
                // Basic upload
                _client.PutObject(_ossConfig.BucketName, objectKey, stream);

                // To upload asynchronously (more suitable for web apps)
                // var putObjectRequest = new PutObjectRequest(_ossConfig.BucketName, objectKey, stream);
                // var result = await Task.Factory.FromAsync(
                //    (callback, state) => _client.BeginPutObject(putObjectRequest, callback, state),
                //    ar => _client.EndPutObject(ar),
                //    null);

            }

            // Construct the public URL (ensure your bucket has public read access or use signed URLs for private objects)
            // The exact URL format might depend on your OSS endpoint and bucket settings.
            // For public buckets, it's usually: https://<BucketName>.<Endpoint>/<ObjectKey>
            // Ensure your Endpoint in appsettings.json does NOT include "https://" for the OssClient initialization,
            // but you might need it here for the public URL.
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
            // Log OSS specific errors
            Console.WriteLine($"OSS Error Code: {ex.ErrorCode}, Message: {ex.Message}");
            throw; // Re-throw or handle as appropriate
        }
        catch (Exception ex)
        {
            // Log general errors
            Console.WriteLine($"Error uploading file: {ex.Message}");
            throw; // Re-throw or handle as appropriate
        }
    }
}