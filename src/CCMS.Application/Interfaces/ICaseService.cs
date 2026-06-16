using System.Collections.Generic;
using System.Threading.Tasks;
using CCMS.Application.DTOs;

namespace CCMS.Application.Interfaces
{
    public interface ICaseService
    {
        Task<CaseDto> CreateCaseAsync(CreateCaseDto dto, 
            (System.IO.Stream Stream, string FileName, string ContentType) courtOrder,
            (System.IO.Stream Stream, string FileName, string ContentType) aadhaarDoc,
            (System.IO.Stream Stream, string FileName, string ContentType) panDoc);
        Task<PagedResult<CaseDto>> GetCasesAsync(int page = 1, int limit = 15);
        Task<CaseStatisticsDto> GetStatisticsAsync();
        Task<CaseDetailsDto?> GetCaseByIdAsync(int id);
        Task<(System.IO.Stream? Stream, string ContentType, string FileName)> GetDocumentAsync(int caseId, int documentId);
    }
}
