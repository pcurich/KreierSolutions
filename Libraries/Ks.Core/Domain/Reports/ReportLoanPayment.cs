using System;
namespace Ks.Core.Domain.Reports
{
    public class ReportLoanPayment
    {
        public string MonthName { get; set; }
        public int Year { get; set; }
        public int Quota { get; set; }
        public decimal MonthlyCapital { get; set; }
        public decimal MonthlyFee { get; set; }
        public decimal MonthlyQuota { get; set; }
        public int StateId { get; set; }
        public string Description { get; set; }
    }
}