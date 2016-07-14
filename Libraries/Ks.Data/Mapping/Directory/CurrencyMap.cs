using Ks.Core.Domain.Directory;

namespace Ks.Data.Mapping.Directory
{
    public partial class CurrencyMap : KsEntityTypeConfiguration<Currency>
    {
        public CurrencyMap()
        {
            ToTable("Currency");
            HasKey(c =>c.Id);
            Property(c => c.Name).IsRequired().HasMaxLength(50);
            Property(c => c.CurrencyCode).IsRequired().HasMaxLength(5);
            Property(c => c.DisplayLocale).HasMaxLength(50);
            Property(c => c.CustomFormatting).HasMaxLength(50);
            Property(c => c.Rate).HasPrecision(18, 4);
        }
    }
}