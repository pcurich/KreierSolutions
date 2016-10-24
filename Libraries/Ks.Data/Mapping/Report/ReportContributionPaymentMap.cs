using Ks.Core.Domain.Report;

namespace Ks.Data.Mapping.Report
{
    public partial class ReportContributionPaymentMap : KsEntityTypeConfiguration<ReportContributionPayment>
    {
        public ReportContributionPaymentMap()
        {
            ToTable("ReportContributionPayment");
            HasKey(ea => ea.Id);
        }
    }
}