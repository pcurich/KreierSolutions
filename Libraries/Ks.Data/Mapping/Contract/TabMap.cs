using Ks.Core.Domain.Contract;

namespace Ks.Data.Mapping.Contract
{
    public class TabMap : KsEntityTypeConfiguration<Tab>
    {
        public TabMap()
        {
            ToTable("Tab");
            HasKey(sp => sp.Id);
            Property(sp => sp.Name).IsRequired();
            Property(sp => sp.BaseAmount).HasPrecision(12, 2);
        }
    }
}