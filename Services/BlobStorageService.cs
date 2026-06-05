using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace EventEase2026.Services;

public interface IBlobStorageService
{
    Task EnsureContainerAsync();
    Task<string> UploadAsync(Stream content, string fileName, string contentType);
    Task DeleteAsync(string blobUrl);
}

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobContainerClient _container;

    public BlobStorageService(IConfiguration config)
    {
        var connectionString = config.GetConnectionString("AzureStorage")
            ?? throw new InvalidOperationException("AzureStorage connection string missing.");
        var containerName = config["BlobStorage:ContainerName"] ?? "venue-images";

        var serviceClient = new BlobServiceClient(connectionString);
        _container = serviceClient.GetBlobContainerClient(containerName);
    }

    public async Task EnsureContainerAsync()
    {
        // Allow direct image access from the browser.
        await _container.CreateIfNotExistsAsync(PublicAccessType.Blob);
    }

    public async Task<string> UploadAsync(Stream content, string fileName, string contentType)
    {
        var safeName = $"{Guid.NewGuid():N}_{Path.GetFileName(fileName)}";
        var blob = _container.GetBlobClient(safeName);

        await blob.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType });
        return blob.Uri.ToString();
    }

    public async Task DeleteAsync(string blobUrl)
    {
        if (string.IsNullOrWhiteSpace(blobUrl)) return;
        try
        {
            var name = new Uri(blobUrl).Segments.Last();
            await _container.DeleteBlobIfExistsAsync(name);
        }
        catch
        {
            // Swallow — failing to delete an old image must not block the user action.
        }
    }
}
