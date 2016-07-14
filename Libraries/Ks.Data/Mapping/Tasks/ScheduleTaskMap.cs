using Ks.Core.Domain.Tasks;

namespace Ks.Data.Mapping.Tasks
{
    public partial class ScheduleTaskMap : KsEntityTypeConfiguration<ScheduleTask>
    {
        public ScheduleTaskMap()
        {
            ToTable("ScheduleTask");
            HasKey(t => t.Id);
            Property(t => t.Name).IsRequired();
            Property(t => t.Type).IsRequired();
        }
    }
}