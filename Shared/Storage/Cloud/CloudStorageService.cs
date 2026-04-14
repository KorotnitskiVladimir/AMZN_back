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

    public async Task<string> SaveImageAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));
        if (file.Length <= 0) throw new InvalidOperationException("File is empty");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var blobName = $"{Guid.NewGuid():N}{ext}";

        BlobClient blob = _containerClient.GetBlobClient(blobName);

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

        await using Stream stream = file.OpenReadStream();
        await blob.UploadAsync(stream, options, cancellationToken);

        return blob.Uri.AbsoluteUri;
    }

    public async Task<Stream?> GetFileAsync(string fileName, CancellationToken cancellationToken = default)
    {
        BlobClient blob = _containerClient.GetBlobClient(fileName);

        if (!await blob.ExistsAsync(cancellationToken))
            return null;

        BlobDownloadResult download = await blob.DownloadContentAsync(cancellationToken);
        return download.Content.ToStream();
    }


    public async Task<bool> DeleteFileByUrlAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
            return false;

        string blobName = ExtractBlobName(fileUrl);
        return await DeleteFileByNameAsync(blobName, cancellationToken);
    }

    public async Task<bool> DeleteFileByNameAsync(string blobName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(blobName))
            return false;

        BlobClient blob = _containerClient.GetBlobClient(blobName);
        Response<bool> result = await blob.DeleteIfExistsAsync(cancellationToken: cancellationToken);

        return result.Value;
    }

    // Из blob url достает имя файла (последний сегмент пути)
    private static string ExtractBlobName(string fileUrl)
    {
        if (!Uri.TryCreate(fileUrl, UriKind.Absolute, out var uri))
            throw new InvalidOperationException("Invalid blob url");

        return Uri.UnescapeDataString(uri.Segments.Last().Trim('/'));
    }

}