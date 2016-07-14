using Ks.Core.Domain.Common;

namespace Ks.Data.Mapping.Common
{
    public partial class AddressMap : KsEntityTypeConfiguration<Address>
    {
        public AddressMap()
        {
            ToTable("Address");
            HasKey(a => a.Id);

            HasOptional(a => a.Country)
                .WithMany()
                .HasForeignKey(a => a.CountryId).WillCascadeOnDelete(false);

            HasOptional(a => a.StateProvince)
                .WithMany()
                .HasForeignKey(a => a.StateProvinceId).WillCascadeOnDelete(false);
        }
    }
}
