using Ks.Core.Domain.Customers;

namespace Ks.Data.Mapping.Customers
{
    public partial class CustomerAttributeMap : KsEntityTypeConfiguration<CustomerAttribute>
    {
        public CustomerAttributeMap()
        {
            ToTable("CustomerAttribute");
            HasKey(ca => ca.Id);
            Property(ca => ca.Name).IsRequired().HasMaxLength(400);

            Ignore(ca => ca.AttributeControlType);
        }
    }
}