namespace Ks.Core.Domain.Reports
{
    public class ReportGlobal
    {
        public string Source { get; set; }
        public string AdmCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CustomerId { get; set; }
        public int Number{get; set;}
        public decimal Payed { get; set; }
        public int IsAutomatic { get; set; }
        public string BankName { get; set; }
        public int StateId { get; set; }
        public string ScheduledDate { get; set; }
        public string ProcessedDate { get; set; }

    }
}