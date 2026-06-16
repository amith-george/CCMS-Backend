using System.Collections.Generic;

namespace CCMS.Application.DTOs
{
    public class CaseDetailsDto : CaseDto
    {
        public string? BankRemarks { get; set; }
        public string? SystemRemarks { get; set; }
        public string? BatchAccountStatus { get; set; }

        public List<CaseDocumentDto> Documents { get; set; } = new List<CaseDocumentDto>();
    }
}
