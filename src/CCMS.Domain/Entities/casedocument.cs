using CCMS.Domain.Enums;

namespace CCMS.Domain.Entities
{
    public class CaseDocument
    {
        public int Id { get; set; }
        public int CaseId { get; set; }
        public DocumentType Type { get; set; }
        public string FilePath { get; set; } = string.Empty; 
        public string FileName { get; set; } = string.Empty; 
        
        public Case Case { get; set; } = null!;
    }
}