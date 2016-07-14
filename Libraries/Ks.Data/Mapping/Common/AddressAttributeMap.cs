using Ks.Core.Domain.Common;

namespace Ks.Data.Mapping.Common
{
    public partial class AddressAttributeMap : KsEntityTypeConfiguration<AddressAttribute>
    {
        public AddressAttributeMap()
        {
            ToTable("AddressAttribute");
            HasKey(aa => aa.Id);
            Property(aa => aa.Name).IsRequired().HasMaxLength(400);

            Ignore(aa => aa.AttributeControlType);
        }
    }
}