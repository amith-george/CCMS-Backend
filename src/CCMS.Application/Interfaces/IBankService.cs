using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CCMS.Application.DTOs;

namespace CCMS.Application.Interfaces
{
    public interface IBankService
    {
        Task<IEnumerable<CaseDto>> GetBankCasesAsync(CancellationToken cancellationToken = default);
        Task<CaseDto?> GetBankCaseByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> SubmitBankResponseAsync(int caseId, BankResponseDto responseDto, CancellationToken cancellationToken = default);
    }
}
