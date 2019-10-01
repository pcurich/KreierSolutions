using System;

namespace Ks.Core.Domain.Reports
{
    public class ReportLoan
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AdmCode { get; set; }
        public string MilitarySituation { get; set; }
        public int LoanNumber { get; set; }
        public int Period { get; set; }
        public int TotalOfCycle { get; set; }
        public float Tea { get; set; }
        public float Safe { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal MonthlyQuota { get; set; }
        public decimal TotalFeed { get; set; }
        public decimal TotalSafe { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalToPay { get; set; }
        public decimal TotalPayed { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string CheckNumber { get; set; }
        public bool IsDelay { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime ApprovalOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}
