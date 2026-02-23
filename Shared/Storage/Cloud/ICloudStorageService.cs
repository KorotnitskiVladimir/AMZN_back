namespace AMZN.Services.Storage.Cloud;

public interface ICloudStorageService
{
    string SaveFile(IFormFile file);
    Stream GetFile(string fileName);

    bool DeleteByUrl(string fileUrl);
    bool DeleteByName(string blobName);
}