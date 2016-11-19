using Ks.Core.Domain.Contract;

namespace Ks.Data.Mapping.Contract
{
    public partial class ContributionMap : KsEntityTypeConfiguration<Contribution>
    {
        public ContributionMap()
        {
            ToTable("Contribution");
            HasKey(sp => sp.Id);
            Property(sp => sp.AuthorizeDiscount).IsRequired();
            Property(sp => sp.AmountMeta).HasPrecision(6, 2);
            Property(sp => sp.AmountPayed).HasPrecision(6, 2);
            Property(sp => sp.AmountPayed).HasPrecision(6, 2);
        }
    }
}