using System.IO;
using System.Threading.Tasks;

namespace CCMS.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
        Task<Stream?> GetFileAsync(string fileUrl);
        Task DeleteFileAsync(string fileUrl);
    }
}
