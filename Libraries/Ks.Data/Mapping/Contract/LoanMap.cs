using Ks.Core.Domain.Contract;

namespace Ks.Data.Mapping.Contract
{
    public class LoanMap : KsEntityTypeConfiguration<Loan>
    {
        public LoanMap()
        {
            ToTable("Loan");
            HasKey(sp => sp.Id);
            Property(sp => sp.LoanNumber).IsRequired();
            Property(sp => sp.MonthlyQuota).HasPrecision(12, 2);

            Property(sp => sp.LoanAmount).HasPrecision(12, 2);
            Property(sp => sp.TotalFeed).HasPrecision(12, 2);
            Property(sp => sp.TotalSafe).HasPrecision(12, 2);
            Property(sp => sp.TotalAmount).HasPrecision(12, 2);
            Property(sp => sp.TotalToPay).HasPrecision(12, 2);
            Property(sp => sp.TotalPayed).HasPrecision(12, 2);

        }
    }
}