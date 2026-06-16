using System;
using System.Collections.Generic;
using CCMS.Domain.Enums;

namespace CCMS.Domain.Entities
{
    public class Case
    {
        public int Id { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        
        // Court Input
        public string DefendantName { get; set; } = string.Empty;
        public string TargetBank { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string AadhaarNumber { get; set; } = string.Empty;
        public string PanNumber { get; set; } = string.Empty;
        
        public OrderType OrderType { get; set; }
        public decimal? RequestedFreezeAmount { get; set; } 
        
        // Status
        public CaseStatus Status { get; set; } = CaseStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ResolvedAt { get; set; }

        // Batch Job Results
        public string? MatchedAccountNumber { get; set; }
        public decimal? BatchFoundBalance { get; set; }
        public string? BatchAccountStatus { get; set; }
        
        // Bank Officer Response
        public decimal? FinalFreezeAmount { get; set; }
        public decimal? FinalReportedBalance { get; set; }
        public string? BankRemarks { get; set; }
        public string? SystemRemarks { get; set; } 

        public ICollection<CaseDocument> Documents { get; set; } = new List<CaseDocument>();
    }
}