namespace AMZN.Services.Storage.Local;

public class LocalStorageService : ILocalsStorageService
{
    private const string LocalPath = "C:\\amzn\\"; // указываем путь к папке с сохранением изображений

    public string GetRealPath(string fileName)
    {
        return LocalPath + fileName;
    }

    public string SaveFile(IFormFile file)
    {
        var ext = Path.GetExtension(file.FileName);
        string savedName;
        string fullName;
        do
        {
            savedName = Guid.NewGuid() + ext;
            fullName = LocalPath + savedName;
        } while (File.Exists(fullName));
        file.CopyTo(new FileStream(fullName, FileMode.CreateNew));
        return savedName;
    }
}