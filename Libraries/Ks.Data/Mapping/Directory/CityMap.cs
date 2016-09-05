using Ks.Core.Domain.Directory;

namespace Ks.Data.Mapping.Directory
{
    public class CityMap : KsEntityTypeConfiguration<City>
    {
        public CityMap()
        {
            ToTable("City");
            HasKey(sp => sp.Id);
            Property(sp => sp.Name).IsRequired().HasMaxLength(100);
            Property(sp => sp.Ubigeo).HasMaxLength(100);


            HasRequired(sp => sp.StateProvince)
                .WithMany(c => c.Cities)
                .HasForeignKey(sp => sp.StateProvinceId);
        }
    }
}