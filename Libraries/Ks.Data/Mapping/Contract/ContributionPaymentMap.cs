using Ks.Core.Domain.Contract;

namespace Ks.Data.Mapping.Contract
{
    public partial class ContributionPaymentMap : KsEntityTypeConfiguration<ContributionPayment>
    {
        public ContributionPaymentMap()
        {
            ToTable("ContributionPayment");
            HasKey(sp => sp.Id);
            Property(sp => sp.Amount1).HasPrecision(6, 4);
            Property(sp => sp.Amount2).HasPrecision(6, 4);
            Property(sp => sp.Amount3).HasPrecision(6, 4);

            HasRequired(sp => sp.Contribution)
                .WithMany(c => c.ContributionPayments)
                .HasForeignKey(sp => sp.ContributionId);
        }
    }
}