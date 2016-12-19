namespace Ks.Core.Domain.Reports
{
    public class ReportContributionPayment:BaseEntity
    {
        public int Year { get; set; }
        public int StateId { get; set; }
        public int IsAutomatic { get; set; }
        public decimal Ene { get; set; }
        public decimal Feb { get; set; }
        public decimal Mar { get; set; }
        public decimal Abr { get; set; }
        public decimal May { get; set; }
        public decimal Jun { get; set; }
        public decimal Jul { get; set; }
        public decimal Ago { get; set; }
        public decimal Sep { get; set; }
        public decimal Oct { get; set; }
        public decimal Nov { get; set; }
        public decimal Dic { get; set; }
    }
}
