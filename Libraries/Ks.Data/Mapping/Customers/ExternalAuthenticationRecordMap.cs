using Ks.Core.Domain.Customers;

namespace Ks.Data.Mapping.Customers
{
    public partial class ExternalAuthenticationRecordMap : KsEntityTypeConfiguration<ExternalAuthenticationRecord>
    {
        public ExternalAuthenticationRecordMap()
        {
            ToTable("ExternalAuthenticationRecord");

            HasKey(ear => ear.Id);

            HasRequired(ear => ear.Customer)
                .WithMany(c => c.ExternalAuthenticationRecords)
                .HasForeignKey(ear => ear.CustomerId);

        }
    }
}