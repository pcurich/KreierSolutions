using System;

namespace Ks.Core.Domain.Reports
{
    public class ReportInterfaceLoan
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AdmCode { get; set; }
        public string Dni { get; set; }
        public string MilitarySituation { get; set; }
        public string Active { get; set; }
        public int LoanNumber { get; set; }
        public decimal LoanAmount { get; set; }
        public int Quota { get; set; }
        public DateTime ScheduledDateOnUtc { get; set; }
        public decimal MonthlyCapital { get; set; }
        public decimal MonthlyFee { get; set; }
        public decimal MonthlyQuota { get; set; }
        public decimal MonthlyPayed { get; set; }
        public string State { get; set; }
    }
}