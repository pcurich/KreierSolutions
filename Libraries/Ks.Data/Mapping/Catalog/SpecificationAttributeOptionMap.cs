using Ks.Core.Domain.Catalog;

namespace Ks.Data.Mapping.Catalog
{
    public partial class SpecificationAttributeOptionMap : KsEntityTypeConfiguration<SpecificationAttributeOption>
    {
        public SpecificationAttributeOptionMap()
        {
            ToTable("SpecificationAttributeOption");
            HasKey(sao => sao.Id);
            Property(sao => sao.Name).IsRequired();
            
            HasRequired(sao => sao.SpecificationAttribute)
                .WithMany(sa => sa.SpecificationAttributeOptions)
                .HasForeignKey(sao => sao.SpecificationAttributeId);
        }
    }
}