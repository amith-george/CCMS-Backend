namespace CCMS.Application.DTOs
{
    public class BankResponseDto
    {
        public decimal? FinalFreezeAmount { get; set; }
        public decimal? FinalReportedBalance { get; set; }
        public string? BankRemarks { get; set; }
    }
}
