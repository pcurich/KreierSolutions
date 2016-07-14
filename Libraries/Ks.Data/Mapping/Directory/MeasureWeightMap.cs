using Ks.Core.Domain.Directory;

namespace Ks.Data.Mapping.Directory
{
    public partial class MeasureWeightMap : KsEntityTypeConfiguration<MeasureWeight>
    {
        public MeasureWeightMap()
        {
            ToTable("MeasureWeight");
            HasKey(m => m.Id);
            Property(m => m.Name).IsRequired().HasMaxLength(100);
            Property(m => m.SystemKeyword).IsRequired().HasMaxLength(100);
            Property(m => m.Ratio).HasPrecision(18, 8);
        }
    }
}