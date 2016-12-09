using Ks.Core.Domain.Contract;

namespace Ks.Data.Mapping.Contract
{
    public class TabDetailMap: KsEntityTypeConfiguration<TabDetail>
    {
        public TabDetailMap()
        {
            ToTable("TabDetail");
            HasKey(sp => sp.Id);
            Property(sp => sp.YearInActivity).IsRequired();
            Property(sp => sp.Value);
        }
    }
}