using Ks.Core.Domain.Contract;

namespace Ks.Data.Mapping.Contract
{
    public partial class ContributionMap : KsEntityTypeConfiguration<Contribution>
    {
        public ContributionMap()
        {
            ToTable("Contribution");
            HasKey(sp => sp.Id);
            Property(sp => sp.LetterNumber).IsRequired();
            Property(sp => sp.AmountTotal).HasPrecision(6, 2);
        }
    }
}