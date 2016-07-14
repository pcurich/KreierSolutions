using Ks.Core.Domain.Directory;

namespace Ks.Data.Mapping.Directory
{
    public partial class StateProvinceMap : KsEntityTypeConfiguration<StateProvince>
    {
        public StateProvinceMap()
        {
            ToTable("StateProvince");
            HasKey(sp => sp.Id);
            Property(sp => sp.Name).IsRequired().HasMaxLength(100);
            Property(sp => sp.Abbreviation).HasMaxLength(100);


            HasRequired(sp => sp.Country)
                .WithMany(c => c.StateProvinces)
                .HasForeignKey(sp => sp.CountryId);
        }
    }
}