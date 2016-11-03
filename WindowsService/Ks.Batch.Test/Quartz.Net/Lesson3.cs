using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace Ks.Batch.Test.Quartz.Net
{
    class Lesson3
    {
        public void Test()
        {
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            // get a scheduler
            IScheduler sched = schedFact.GetScheduler();
            sched.Start();
            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<HelloJob2>()
                .WithIdentity("myJob", "group1")
                .UsingJobData("jobSays", "Hello World!")
                .UsingJobData("myFloatValue", 3.141f)
                .Build();

            // Trigger the job to run now, and then every 40 seconds
            ITrigger trigger = TriggerBuilder.Create()
              .WithIdentity("myTrigger", "group1")
              .StartNow()
              .WithSimpleSchedule(x => x
                  .WithIntervalInSeconds(40)
                  .RepeatForever())
              .Build();

            sched.ScheduleJob(job, trigger);
        }

        //Cada vez que el planificador ejecuta el job, crea una nueva instancia de la clase antes de llamar al Execute
        //asi que no vale la pena poner campos porque su valor desaparece entre cada ejecucion
        //Para mantener el rastro entre cada ejecucion de jobs es a travez de JobDataMap

        //JobDataMap  es una implementacion de IDictionary

    }
    class HelloJob2 : IJob
    {
        public string JobSays { private get; set; }
        public float FloatValue { private get; set; }

        public void Execute(IJobExecutionContext context)
        {
            JobKey key = context.JobDetail.Key;

            JobDataMap dataMap = context.JobDetail.JobDataMap;

            JobSays = dataMap.GetString("jobSays");
            FloatValue = dataMap.GetFloat("myFloatValue");

            IList<DateTimeOffset> state = (IList<DateTimeOffset>)dataMap["myStateData"];
            state.Add(DateTimeOffset.UtcNow);

            Console.WriteLine("HelloJob is executing.");
            Console.Error.WriteLine("Instance " + key + " of DumbJob says: " + jobSays + ", and val is: " + myFloatValue);
        }
    }
}
