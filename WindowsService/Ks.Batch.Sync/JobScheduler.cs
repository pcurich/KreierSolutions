using Quartz;
using Quartz.Impl;

namespace Ks.Batch.Sync
{
    public class JobScheduler
    {
        public bool Start()
        {
            var schedFact = new StdSchedulerFactory();
            var sched = schedFact.GetScheduler();
            sched.Start();

            var job = JobBuilder.Create<InfoJob>()
                .WithIdentity("SyncBatchService", "KS").Build();

            var trigger = TriggerBuilder.Create()
              .WithIdentity("SyncConfigFiles", "KS").StartNow()
              .WithSimpleSchedule(x => x.WithIntervalInSeconds(40).RepeatForever())
              .Build();

            sched.ScheduleJob(job, trigger);

            return true;
        }
    }
}