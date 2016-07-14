using Ks.Core.Domain.Directory;

namespace Ks.Data.Mapping.Directory
{
    public partial class CountryMap : KsEntityTypeConfiguration<Country>
    {
        public CountryMap()
        {
            ToTable("Country");
            HasKey(c =>c.Id);
            Property(c => c.Name).IsRequired().HasMaxLength(100);
            Property(c =>c.TwoLetterIsoCode).HasMaxLength(2);
            Property(c =>c.ThreeLetterIsoCode).HasMaxLength(3);
        }
    }
}