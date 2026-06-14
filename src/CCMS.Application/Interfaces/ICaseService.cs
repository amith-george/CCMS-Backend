using System.Collections.Generic;
using System.Threading.Tasks;
using CCMS.Application.DTOs;

namespace CCMS.Application.Interfaces
{
    public interface ICaseService
    {
        Task<CaseDto> CreateCaseAsync(CreateCaseDto dto);
        Task<IEnumerable<CaseDto>> GetCasesAsync();
    }
}
