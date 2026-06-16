namespace CCMS.Application.DTOs
{
    public class CaseStatisticsDto
    {
        public int TotalCases { get; set; }
        public int PendingBatch { get; set; }
        public int AwaitingAction { get; set; }
        public int AutoResolved { get; set; }
        public int Completed { get; set; }
    }
}
