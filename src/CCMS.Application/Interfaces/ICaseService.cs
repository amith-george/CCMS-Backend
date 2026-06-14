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
        Task<IEnumerable<CaseDto>> GetCasesAsync();
    }
}
