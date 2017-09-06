namespace Ks.Core.Domain.Reports
{
    public class ReportLoanDetail
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AdmCode { get; set; }
        public string MilitarySituation { get; set; }
        public string ApprovalOn { get; set; }
        public int LoanNumber { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal TotalToPay { get; set; }
        public int Period { get; set; }
        public decimal MonthlyPayed { get; set; }
        public int LoanId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Debit { get; set; }
        public string Active { get; set; }
    }
}