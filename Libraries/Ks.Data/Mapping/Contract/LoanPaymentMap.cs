using Ks.Core.Domain.Contract;

namespace Ks.Data.Mapping.Contract
{
    public partial class LoanPaymentMap : KsEntityTypeConfiguration<LoanPayment>
    {
        public LoanPaymentMap()
        {
            ToTable("LoanPayment");
            HasKey(sp => sp.Id);
            Property(sp => sp.MonthlyQuota).HasPrecision(12, 2);
            Property(sp => sp.MonthlyFee).HasPrecision(12, 2);
            Property(sp => sp.MonthlyCapital).HasPrecision(12, 2);
            Property(sp => sp.MonthlyPayed).HasPrecision(12, 2);

            Ignore(sp => sp.LoanState);

            HasRequired(sp => sp.Loan)
                .WithMany(c => c.LoanPayments)
                .HasForeignKey(sp => sp.LoanId);
        }
    }
}