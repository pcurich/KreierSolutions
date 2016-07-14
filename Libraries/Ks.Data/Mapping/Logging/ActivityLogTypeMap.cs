using Ks.Core.Domain.Logging;

namespace Ks.Data.Mapping.Logging
{
    public partial class ActivityLogTypeMap : KsEntityTypeConfiguration<ActivityLogType>
    {
        public ActivityLogTypeMap()
        {
            ToTable("ActivityLogType");
            HasKey(alt => alt.Id);

            Property(alt => alt.SystemKeyword).IsRequired().HasMaxLength(100);
            Property(alt => alt.Name).IsRequired().HasMaxLength(200);
        }
    }
}
