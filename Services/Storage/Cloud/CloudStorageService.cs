namespace AMZN.Services.Storage.Cloud;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

public class CloudStorageService : ICloudStorageService
{
    private readonly BlobContainerClient _containerClient;

    public CloudStorageService(IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("AzureStorage");
        string containerName = "amzn-storage";
        _containerClient = new BlobContainerClient(connectionString, containerName);
        _containerClient.CreateIfNotExists();
    }
    

    public string SaveFile(IFormFile file)
    {
        string path = "C:/Users/vladi/Downloads/"; // указываем путь к папке, из которой загружаем картинки
        string savedName = file.FileName;
        BlobClient blob = _containerClient.GetBlobClient(savedName);
        using (FileStream fs = File.OpenRead(path + savedName))
        {
            blob.Upload(fs, true);
        }

        return blob.Uri.AbsoluteUri;
    }

    public Stream GetFile(string fileName)
    {
        BlobClient blob = _containerClient.GetBlobClient(fileName);
        if (blob.Exists())
        {
            BlobDownloadInfo download = blob.Download();
            return download.Content;
        }
        return null;
    }
}