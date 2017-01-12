namespace Ks.Core.Domain.Reports
{
    public class ReportLoanDetail
    {
        public string ApprovalDate { get; set; }
        public int LoanNumber { get; set; }
        public string LoanState { get; set; }
        public string CheckNumber { get; set; }
        public int CustomerId { get; set; }
        public string Source { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AdmCode { get; set; }
        public string CustomerState { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal TotalSafe { get; set; }
        public decimal TotalToPay { get; set; }
        public decimal TotalFeed { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPayed { get; set; }
        public decimal PayedCapital { get; set; }
        public decimal PayedTax { get; set; }
        public decimal Debit { get; set; }
        public int Period { get; set; }
        public decimal MonthlyQuota { get; set; }
        public decimal QoutaCapital { get; set; }
        public decimal QoutaTax { get; set; }
        public string LastDate { get; set; }
    }
}