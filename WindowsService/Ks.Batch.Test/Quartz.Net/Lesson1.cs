using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace Ks.Batch.Test.Quartz.Net
{
    class Lesson1
    {
        public void Test1()
        {
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            // get a scheduler
            IScheduler sched = schedFact.GetScheduler();
            sched.Start();

            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<HelloJob>()
                .WithIdentity("myJob", "group1")
                .Build();

            // Trigger the job to run now, and then every 40 seconds
            ITrigger trigger = TriggerBuilder.Create()
              .WithIdentity("myTrigger", "group1")
              .StartNow()
              .WithSimpleSchedule(x => x.WithIntervalInSeconds(40).RepeatForever())
              .WithCalendarIntervalSchedule()
              .WithCronSchedule("")
              .WithDailyTimeIntervalSchedule()
              .Build();

            // Tell quartz to schedule the job using our trigger
            sched.ScheduleJob(job, trigger);

             
        }
    }

    public class HelloJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
