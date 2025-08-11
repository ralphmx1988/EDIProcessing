using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using EDIProcessingApp.Core.Interfaces;

namespace EDIProcessingApp.Infrastructure.Services;

public class AzureBlobStorageService : IFileStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<AzureBlobStorageService> _logger;
    private readonly string _defaultContainerName;

    public AzureBlobStorageService(
        BlobServiceClient blobServiceClient, 
        IConfiguration configuration,
        ILogger<AzureBlobStorageService> logger)
    {
        _blobServiceClient = blobServiceClient;
        _logger = logger;
        _defaultContainerName = configuration["AzureStorage:DefaultContainer"] ?? "edi-files";
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string containerName = "edi-files")
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName ?? _defaultContainerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

            var blobName = $"{DateTime.UtcNow:yyyy/MM/dd}/{Guid.NewGuid()}/{fileName}";
            var blobClient = containerClient.GetBlobClient(blobName);

            fileStream.Position = 0;
            await blobClient.UploadAsync(fileStream, overwrite: true);

            _logger.LogInformation("File {FileName} uploaded successfully to {BlobName}", fileName, blobName);
            
            return blobClient.Uri.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName}", fileName);
            throw;
        }
    }

    public async Task<Stream> DownloadFileAsync(string filePath)
    {
        try
        {
            var uri = new Uri(filePath);
            var containerName = uri.Segments[1].TrimEnd('/');
            var blobName = string.Join("", uri.Segments.Skip(2));

            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var response = await blobClient.DownloadStreamingAsync();
            return response.Value.Content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file {FilePath}", filePath);
            throw;
        }
    }

    public async Task<bool> DeleteFileAsync(string filePath)
    {
        try
        {
            var uri = new Uri(filePath);
            var containerName = uri.Segments[1].TrimEnd('/');
            var blobName = string.Join("", uri.Segments.Skip(2));

            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
            
            _logger.LogInformation("File {FilePath} deleted successfully", filePath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {FilePath}", filePath);
            return false;
        }
    }

    public string GetFileUrl(string filePath, TimeSpan? expiry = null)
    {
        try
        {
            var uri = new Uri(filePath);
            var containerName = uri.Segments[1].TrimEnd('/');
            var blobName = string.Join("", uri.Segments.Skip(2));
    
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
    
            if (blobClient.CanGenerateSasUri)
            {
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = containerName,
                    BlobName = blobName,
                    Resource = "b",
                    ExpiresOn = DateTimeOffset.UtcNow.Add(expiry ?? TimeSpan.FromHours(1))
                };
                sasBuilder.SetPermissions(BlobSasPermissions.Read);
    
                return blobClient.GenerateSasUri(sasBuilder).ToString();
            }
    
            return filePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating URL for file {FilePath}", filePath);
            throw;
        }
    }
    
    public Task<string> GetFileUrlAsync(string filePath, TimeSpan? expiry = null)
    {
        return Task.FromResult(GetFileUrl(filePath, expiry));
    }
}
