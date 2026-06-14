namespace CCMS.Domain.Entities
{
    public class BankCustomer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string AadhaarNumber { get; set; } = string.Empty;
        public string PanNumber { get; set; } = string.Empty;
        public decimal CurrentBalance { get; set; }
        public string AccountStatus { get; set; } = "Active";
    }
}