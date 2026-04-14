namespace AMZN.Services.Storage.Cloud;

public interface ICloudStorageService
{
    Task<string> SaveImageAsync(IFormFile file, CancellationToken cancellationToken = default);
    Task<Stream?> GetFileAsync(string fileName, CancellationToken cancellationToken = default);

    Task<bool> DeleteFileByUrlAsync(string fileUrl, CancellationToken cancellationToken = default);
    Task<bool> DeleteFileByNameAsync(string blobName, CancellationToken cancellationToken = default);
}