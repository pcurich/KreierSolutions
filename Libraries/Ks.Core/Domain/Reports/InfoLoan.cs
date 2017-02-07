namespace Ks.Core.Domain.Reports
{
    public class InfoLoan
    {
        public int LoanPaymentId { get; set; }
        public int LoanId { get; set; }
        public int Quota { get; set; }
        public decimal MonthlyQuota { get; set; }
        public decimal MonthlyFee { get; set; }
        public decimal MonthlyCapital { get; set; }
        public decimal MonthlyPayed { get; set; }
        public int StateId { get; set; }
        public bool IsAutomatic { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string TransactionNumber { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; } 
    }
}