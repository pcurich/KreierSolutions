using Ks.Core.Domain.Logging;

namespace Ks.Data.Mapping.Logging
{
    public partial class LogMap : KsEntityTypeConfiguration<Log>
    {
        public LogMap()
        {
            ToTable("Log");
            HasKey(l => l.Id);
            Property(l => l.ShortMessage).IsRequired();
            Property(l => l.IpAddress).HasMaxLength(200);

            Ignore(l => l.LogLevel);

            HasOptional(l => l.Customer)
                .WithMany()
                .HasForeignKey(l => l.CustomerId)
            .WillCascadeOnDelete(true);

        }
    }
}