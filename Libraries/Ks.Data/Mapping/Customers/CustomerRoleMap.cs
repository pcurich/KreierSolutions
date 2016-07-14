using Ks.Core.Domain.Customers;

namespace Ks.Data.Mapping.Customers
{
    public partial class CustomerRoleMap : KsEntityTypeConfiguration<CustomerRole>
    {
        public CustomerRoleMap()
        {
            ToTable("CustomerRole");
            HasKey(cr => cr.Id);
            Property(cr => cr.Name).IsRequired().HasMaxLength(255);
            Property(cr => cr.SystemName).HasMaxLength(255);
        }
    }
}