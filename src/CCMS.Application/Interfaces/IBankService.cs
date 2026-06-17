using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CCMS.Application.DTOs;

namespace CCMS.Application.Interfaces
{
    public interface IBankService
    {
        Task<PagedResult<CaseDto>> GetBankCasesAsync(int page = 1, int limit = 15, string filter = "all", CancellationToken cancellationToken = default);
        Task<CaseStatisticsDto> GetBankStatisticsAsync(CancellationToken cancellationToken = default);
        Task<CaseDto?> GetBankCaseByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> SubmitBankResponseAsync(int caseId, BankResponseDto responseDto, CancellationToken cancellationToken = default);
        Task<(System.IO.Stream? Stream, string ContentType, string FileName)> GetCourtOrderDocumentAsync(int caseId, CancellationToken cancellationToken = default);
    }
}
