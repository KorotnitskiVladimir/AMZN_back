namespace AMZN.Services.Storage.Cloud;

public interface ICloudStorageService
{
    string SaveFile(IFormFile file);
    Stream GetFile(string fileName);
}