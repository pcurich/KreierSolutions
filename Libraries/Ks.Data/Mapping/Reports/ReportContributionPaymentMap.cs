using Ks.Core.Domain.Reports;

namespace Ks.Data.Mapping.Reports
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