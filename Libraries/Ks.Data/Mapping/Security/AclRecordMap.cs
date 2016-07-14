using Ks.Core.Domain.Security;

namespace Ks.Data.Mapping.Security
{
    public partial class AclRecordMap : KsEntityTypeConfiguration<AclRecord>
    {
        public AclRecordMap()
        {
            ToTable("AclRecord");
            HasKey(ar => ar.Id);

            Property(ar => ar.EntityName).IsRequired().HasMaxLength(400);

            HasRequired(ar => ar.CustomerRole)
                .WithMany()
                .HasForeignKey(ar => ar.CustomerRoleId)
                .WillCascadeOnDelete(true);
        }
    }
}