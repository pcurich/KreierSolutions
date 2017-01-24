namespace Ks.Core.Domain.Reports
{
    public class ReportMilitarSituation
    {
        public string AdmCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CustomerId { get; set; }
        public int MilitarSituationId { get; set; }
        public string MilitarSituation { get; set; }
        public int ContributionAuthorizeDiscont { get; set; }
        public decimal ContributionAmountMeta { get; set; }
        public decimal ContributionAmountPayed { get; set; }
        public int LoanNumber { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal LoanTotalAmount { get; set; }
        public decimal LoanTotalPayed { get; set; }
        public decimal LoanPeriod { get; set; }
    }
}