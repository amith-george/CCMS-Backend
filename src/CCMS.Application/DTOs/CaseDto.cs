using System;
using CCMS.Domain.Enums;

namespace CCMS.Application.DTOs
{
    public class CaseDto
    {
        public int Id { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        
        public string DefendantName { get; set; } = string.Empty;
        public string TargetBank { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string AadhaarNumber { get; set; } = string.Empty;
        public string PanNumber { get; set; } = string.Empty;
        
        public OrderType OrderType { get; set; }
        public decimal? RequestedFreezeAmount { get; set; } 
        
        public CaseStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }

        public string? BankRemarks { get; set; }
        public string? SystemRemarks { get; set; } 
        
        public string? MatchedAccountNumber { get; set; }
        public decimal? BatchFoundBalance { get; set; }
        public decimal? FinalFreezeAmount { get; set; }
        public decimal? FinalReportedBalance { get; set; }
    }
}
