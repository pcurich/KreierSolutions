using Ks.Core.Domain.Common;

namespace Ks.Data.Mapping.Common
{
    public partial class AddressAttributeValueMap : KsEntityTypeConfiguration<AddressAttributeValue>
    {
        public AddressAttributeValueMap()
        {
            ToTable("AddressAttributeValue");
            HasKey(aav => aav.Id);
            Property(aav => aav.Name).IsRequired().HasMaxLength(400);

            HasRequired(aav => aav.AddressAttribute)
                .WithMany(aa => aa.AddressAttributeValues)
                .HasForeignKey(aav => aav.AddressAttributeId);
        }
    }
}