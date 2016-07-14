using Ks.Core.Domain.Security;

namespace Ks.Data.Mapping.Security
{
    public partial class PermissionRecordMap :KsEntityTypeConfiguration<PermissionRecord>
    {
        public PermissionRecordMap()
        {
            ToTable("PermissionRecord");
            HasKey(pr => pr.Id);
            Property(pr => pr.Name).IsRequired();
            Property(pr => pr.SystemName).IsRequired().HasMaxLength(255);
            Property(pr => pr.Category).IsRequired().HasMaxLength(255);

            HasMany(pr => pr.CustomerRoles)
                .WithMany(cr => cr.PermissionRecords)
                .Map(m => m.ToTable("PermissionRecord_Role_Mapping"));
        }
    }
}