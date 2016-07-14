using Ks.Core.Domain.Catalog;

namespace Ks.Data.Mapping.Catalog
{
    public partial class SpecificationAttributeMap : KsEntityTypeConfiguration<SpecificationAttribute>
    {
        public SpecificationAttributeMap()
        {
            ToTable("SpecificationAttribute");
            HasKey(sa => sa.Id);
            Property(sa => sa.Name).IsRequired();
        }
    }
}