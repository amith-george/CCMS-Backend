using System.Collections.Generic;

namespace CCMS.Application.DTOs
{
    public class CaseDetailsDto : CaseDto
    {
        public List<CaseDocumentDto> Documents { get; set; } = new List<CaseDocumentDto>();
    }
}
