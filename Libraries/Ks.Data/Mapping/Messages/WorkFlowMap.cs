using Ks.Core.Domain.Messages;

namespace Ks.Data.Mapping.Messages
{
    public class WorkFlowMap : KsEntityTypeConfiguration<WorkFlow>
    {
        public WorkFlowMap()
        {
            ToTable("WorkFlow");
            HasKey(wf => wf.Id);
        }
    }
}