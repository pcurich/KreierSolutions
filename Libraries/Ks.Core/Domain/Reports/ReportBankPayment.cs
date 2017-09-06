using System;

namespace Ks.Core.Domain.Reports
{
    public class ReportBankPayment
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AdmCode { get; set; }
        public string Dni { get; set; }
        public string MilitarySituation { get; set; }
        public string Data { get; set; }
        public string TransactionNumber { get; set; }
        public string ProcessedDateOnUtc { get; set; }
        public string BankName { get; set; }
        public decimal AmountPayed { get; set; }
    }
}