using Ks.Core.Domain.System;

namespace Ks.Data.Mapping.System
{
    public partial class KsSystemMap : KsEntityTypeConfiguration<KsSystem>
    {
        public KsSystemMap()
        {
            ToTable("KsSystem");
            HasKey(s => s.Id);
            Property(s => s.Name).IsRequired().HasMaxLength(400);
            Property(s => s.Url).IsRequired().HasMaxLength(400);
            Property(s => s.SecureUrl).HasMaxLength(400);
            Property(s => s.Hosts).HasMaxLength(1000);

            Property(s => s.CompanyName).HasMaxLength(1000);
            Property(s => s.CompanyAddress).HasMaxLength(1000);
            Property(s => s.CompanyPhoneNumber).HasMaxLength(1000);
        }
    }
}