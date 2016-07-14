using Ks.Core.Domain.Common;

namespace Ks.Data.Mapping.Common
{
    public partial class GenericAttributeMap : KsEntityTypeConfiguration<GenericAttribute>
    {
        public GenericAttributeMap()
        {
            ToTable("GenericAttribute");
            HasKey(ga => ga.Id);

            Property(ga => ga.KeyGroup).IsRequired().HasMaxLength(400);
            Property(ga => ga.Key).IsRequired().HasMaxLength(400);
            Property(ga => ga.Value).IsRequired();
        }
    }
}