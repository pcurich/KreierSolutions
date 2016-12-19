using Ks.Core.Domain.Contract;

namespace Ks.Data.Mapping.Contract
{
    public class ContributionBenefitMap: KsEntityTypeConfiguration<ContributionBenefit>
    {
        public ContributionBenefitMap()
        {
            ToTable("ContributionBenefit");
            HasKey(sp => sp.Id);
            Property(sp => sp.AmountBaseOfBenefit).HasPrecision(12, 2);
            Property(sp => sp.TotalToPay).HasPrecision(12, 2);
            Property(sp => sp.TotalContributionCaja).HasPrecision(12, 2);
            Property(sp => sp.TotalContributionCopere).HasPrecision(12, 2);
            Property(sp => sp.TotalContributionPersonalPayment).HasPrecision(12, 2);
            Property(sp => sp.TotalLoanToPay).HasPrecision(12, 2);
            Property(sp => sp.SubTotalToPay).HasPrecision(12, 2);
            Property(sp => sp.ReserveFund).HasPrecision(12, 2);

            HasRequired(al => al.Benefit).WithMany().HasForeignKey(al => al.BenefitId);
            HasRequired(al => al.Contribution).WithMany().HasForeignKey(al => al.ContributionId);


        }
    }
}