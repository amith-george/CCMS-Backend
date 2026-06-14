using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CCMS.Application.Interfaces;

namespace CCMS.Infrastructure.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _storageDirectory = "Uploads";
        private readonly string[] _allowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png" };
        private const int MaxFileSizeMB = 5;
        private const long MaxFileSizeBytes = MaxFileSizeMB * 1024 * 1024;
        private readonly IAuditLogService _auditLogService;

        public LocalFileStorageService(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), _storageDirectory);
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
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

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_storageDirectory, uniqueFileName);
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath);

            using (var fileStreamOutput = new FileStream(fullPath, FileMode.Create))
            {
                await fileStream.CopyToAsync(fileStreamOutput);
            }

            await _auditLogService.LogAsync("Upload", "File", $"File securely uploaded with identifier: {uniqueFileName}");

            // Return relative path
            return filePath.Replace("\\", "/");
        }

        public Task<Stream?> GetFileAsync(string fileUrl)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), fileUrl);
            if (File.Exists(fullPath))
            {
                return Task.FromResult<Stream?>(new FileStream(fullPath, FileMode.Open, FileAccess.Read));
            }
            return Task.FromResult<Stream?>(null);
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), fileUrl);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                await _auditLogService.LogAsync("Delete", "File", $"File deleted from storage: {fileUrl}");
            }
        }
    }
}
