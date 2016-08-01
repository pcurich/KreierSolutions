using Ks.Core.Domain.Messages;

namespace Ks.Data.Mapping.Messages
{
    public class EmailAccountMap : KsEntityTypeConfiguration<EmailAccount>
    {
        public EmailAccountMap()
        {
            ToTable("EmailAccount");
            HasKey(ea => ea.Id);

            Property(ea => ea.Email).IsRequired().HasMaxLength(255);
            Property(ea => ea.DisplayName).HasMaxLength(255);
            Property(ea => ea.Host).IsRequired().HasMaxLength(255);
            Property(ea => ea.Username).IsRequired().HasMaxLength(255);
            Property(ea => ea.Password).IsRequired().HasMaxLength(255);

            Ignore(ea => ea.FriendlyName);
        }
    }
}