using Ks.Core.Domain.Batchs;

namespace Ks.Data.Mapping.Batchs
{
    public partial class ScheduleBatchMap : KsEntityTypeConfiguration<ScheduleBatch>
    {
        public ScheduleBatchMap()
        {
            ToTable("ScheduleBatch");
            HasKey(sb => sb.Id);
            Ignore(sb => sb.ScheduleBatchFrecuency);
        }
    }
}