using Azure.Storage.Blobs;
using Loan_Eligibility_WebAPI.Services;

public class BlobService : IBlobService
{
    private readonly string _connectionString;
    private readonly string _containerName;

    public BlobService(IConfiguration config)
    {
        _connectionString = config["AzureBlobStorage:ConnectionString"];
        _containerName = config["AzureBlobStorage:ContainerName"];
    }
    public async Task<string> UploadFileAsync(IFormFile file, string folder, string fileName)
    {
        var container = new BlobContainerClient(_connectionString, _containerName);
        await container.CreateIfNotExistsAsync();

        var blobPath = $"{folder}/{fileName}";

        var blob = container.GetBlobClient(blobPath);

        using var stream = file.OpenReadStream();

        await blob.UploadAsync(stream, overwrite: true);

        return blob.Uri.ToString();
    }
}