using System;

namespace CCMS.Domain.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        public string Operation { get; set; } = string.Empty; 
        
        public string EntityName { get; set; } = string.Empty; 
        
        public string Action { get; set; } = string.Empty; 
        
        public int? UserId { get; set; } 
        public int? CaseId { get; set; } 
    }
}