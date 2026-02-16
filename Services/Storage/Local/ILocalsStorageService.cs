namespace AMZN.Services.Storage.Local;

public interface ILocalsStorageService
{
    string SaveFile(IFormFile file);
    string GetRealPath(string fileName);
}