using Ks.Core.Domain.Contract;

namespace Ks.Data.Mapping.Contract
{
    public partial class ContributionPaymentMap : KsEntityTypeConfiguration<ContributionPayment>
    {
        public ContributionPaymentMap()
        {
            ToTable("ContributionPayment");
            HasKey(sp => sp.Id);
            Property(sp => sp.Amount).HasPrecision(6,2);


            HasRequired(sp => sp.Contribution)
                .WithMany(c => c.ContributionPayments)
                .HasForeignKey(sp => sp.ContributionId);
        }
    }
}