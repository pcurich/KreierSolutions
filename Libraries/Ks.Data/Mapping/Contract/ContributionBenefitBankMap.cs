using Ks.Core.Domain.Contract;

namespace Ks.Data.Mapping.Contract
{
    public class ContributionBenefitBankMap : KsEntityTypeConfiguration<ContributionBenefitBank>
    {
        public ContributionBenefitBankMap()
        {
            ToTable("ContributionBenefitBank");
            HasKey(sp => sp.Id);
            
            HasRequired(pc => pc.ContributionBenefit)
                .WithMany(pc => pc.ContributionBenefitBanks)
                .HasForeignKey(pc => pc.ContributionBenefitId);
        }
    }
}