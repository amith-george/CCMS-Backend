using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CCMS.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CCMS.Infrastructure.Services
{
    public class AzureBlobStorageService : IFileStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;
        private readonly string[] _allowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png" };
        private const int MaxFileSizeMB = 5;
        private const long MaxFileSizeBytes = MaxFileSizeMB * 1024 * 1024;
        private readonly IAuditLogService _auditLogService;

        public AzureBlobStorageService(IConfiguration configuration, IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
            var connectionString = configuration["AzureBlobStorage:ConnectionString"] ?? throw new ArgumentNullException("AzureBlobStorage:ConnectionString is missing");
            _containerName = configuration["AzureBlobStorage:ContainerName"] ?? throw new ArgumentNullException("AzureBlobStorage:ContainerName is missing");
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            if (fileStream.Length > MaxFileSizeBytes)
            {
                throw new ArgumentException($"File size exceeds the maximum limit of {MaxFileSizeMB} MB.");
            }

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
            {
                throw new ArgumentException("Invalid file type. Only PDF, JPG, and PNG are allowed.");
            }

            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var blobClient = containerClient.GetBlobClient(uniqueFileName);

            // Upload the file
            var blobHttpHeaders = new BlobHttpHeaders { ContentType = contentType };
            await blobClient.UploadAsync(fileStream, new BlobUploadOptions { HttpHeaders = blobHttpHeaders });

            await _auditLogService.LogAsync("Upload", "BlobStorage", $"File securely uploaded to Azure with identifier: {uniqueFileName}");

            // Return the unique file name to be saved in the DB (can be reconstructed as URI later)
            return uniqueFileName;
        }

        public async Task<Stream?> GetFileAsync(string fileUrl)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileUrl);

            if (await blobClient.ExistsAsync())
            {
                var downloadResult = await blobClient.DownloadStreamingAsync();
                return downloadResult.Value.Content;
            }

            return null;
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileUrl);

            var deleted = await blobClient.DeleteIfExistsAsync();
            if (deleted.Value)
            {
                await _auditLogService.LogAsync("Delete", "BlobStorage", $"File deleted from Azure storage: {fileUrl}");
            }
        }
    }
}
