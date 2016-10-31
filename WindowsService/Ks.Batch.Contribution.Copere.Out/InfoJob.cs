using System.Configuration;
using Quartz;

namespace Ks.Batch.Contribution.Copere.Out
{
    public class InfoJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var S=ConfigurationManager.AppSettings["countoffiles"];
        }
    }
}