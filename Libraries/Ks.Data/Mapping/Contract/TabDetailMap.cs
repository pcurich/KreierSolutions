using Ks.Core.Domain.Contract;

namespace Ks.Data.Mapping.Contract
{
    public class TabDetailMap: KsEntityTypeConfiguration<TabDetail>
    {
        public TabDetailMap()
        {
            ToTable("TabDetail");
            HasKey(sp => sp.Id);

            HasRequired(pc => pc.Tab)
               .WithMany(pc => pc.TabDetails)
               .HasForeignKey(pc => pc.TabId);
        }
    }
}