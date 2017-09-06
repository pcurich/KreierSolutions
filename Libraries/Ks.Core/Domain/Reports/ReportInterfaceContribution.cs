using System;

namespace Ks.Core.Domain.Reports
{
    public class ReportInterfaceContribution
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AdmCode { get; set; }
        public string Dni { get; set; }
        public string MilitarySituation { get; set; }
        public string Active { get; set; }
        public int Number { get; set; }
        public DateTime ScheduledDateOnUtc { get; set; }
        public decimal Amount1 { get;set; }
        public decimal Amount2 { get;set; }
        public decimal AmountOld { get; set; }
        public decimal AmountTotal { get; set; }
        public decimal AmountPayed { get; set; }
        public string State { get; set; }
    }
}