namespace AMZN.Services.Storage.Cloud;

using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;


public class CloudStorageService : ICloudStorageService
{
    private const string ContainerName = "amzn-storage";
    private readonly BlobContainerClient _containerClient;


    public CloudStorageService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AzureStorage")
            ?? throw new InvalidOperationException("AzureStorage connection string is missing");

        _containerClient = new BlobContainerClient(connectionString, ContainerName);
        _containerClient.CreateIfNotExists();

        try
        {
            _containerClient.SetAccessPolicy(PublicAccessType.Blob);
        }
        catch (RequestFailedException)
        {
            // если PublicAccess запрещен
        }
    }

    public string SaveFile(IFormFile file)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));
        if (file.Length <= 0) throw new InvalidOperationException("File is empty");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var blobName  = $"{Guid.NewGuid():N}{ext}";

        var blob = _containerClient.GetBlobClient(blobName);

        var contentType = file.ContentType;

        // Если клиент не прислал ContentType то определяем по расширению (для корректного отображения в браузере)
        if (string.IsNullOrWhiteSpace(contentType))
        {
            contentType = ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                _ => "image/jpeg"
            };
        }

        var options = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        };

        using var stream = file.OpenReadStream();
        blob.Upload(stream, options);

        return blob.Uri.AbsoluteUri;
    }

    public Stream? GetFile(string fileName)
    {
        BlobClient blob = _containerClient.GetBlobClient(fileName);
        if (blob.Exists())
        {
            BlobDownloadInfo download = blob.Download();
            return download.Content;
        }
        return null;
    }

    public bool DeleteByUrl(string fileUrl)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
            return false;

        var blobName = ExtractBlobName(fileUrl);
        return DeleteByName(blobName);
    }

    public bool DeleteByName(string blobName)
    {
        if (string.IsNullOrWhiteSpace(blobName))
            return false;

        var blob = _containerClient.GetBlobClient(blobName);
        return blob.DeleteIfExists();
    }

    // Из blob url достает имя файла (последний сегмент пути)
    private static string ExtractBlobName(string fileUrl)
    {
        if (!Uri.TryCreate(fileUrl, UriKind.Absolute, out var uri))
            throw new InvalidOperationException("Invalid blob url");

        return Uri.UnescapeDataString(uri.Segments.Last().Trim('/'));
    }

}