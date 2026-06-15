using System;
using CCMS.Domain.Enums;

namespace CCMS.Application.DTOs
{
    public class CaseDocumentDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
    }
}
