using Ks.Core.Domain.Customers;

namespace Ks.Data.Mapping.Customers
{
    public partial class CustomerAttributeValueMap : KsEntityTypeConfiguration<CustomerAttributeValue>
    {
        public CustomerAttributeValueMap()
        {
            ToTable("CustomerAttributeValue");
            HasKey(cav => cav.Id);
            Property(cav => cav.Name).IsRequired().HasMaxLength(400);

            HasRequired(cav => cav.CustomerAttribute)
                .WithMany(ca => ca.CustomerAttributeValues)
                .HasForeignKey(cav => cav.CustomerAttributeId);
        }
    }
}