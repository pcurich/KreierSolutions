using System;
using Quartz;

namespace Ks.Batch.Test.Quartz.Net
{
    public class Lesson5
    {
        public void Test5()
        {
            //SimpleTrigger
            //Se utiliza si se quiere que se ejecute en un determinado momento o en un momento especifco siguiendo un 
            //intervalo o si se quiere que se ejecute en un determinado momento y despues cada 10 segundos
            //La spropiedades que incluye son :
            //Tiempo Inicio, Tiempo Fin, contador de repeticiones, intervalo de repeticiones...

            //1 Build a trigger for a specific moment in time, with no repeats:
            //trigger builder creates simple trigger by default, actually an ITrigger is returned
            ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartAt(DateTime.Now) // some Date 
                .ForJob("job1", "group1") // identify job with name, group strings
                .Build();

            //2 Build a trigger for a specific moment in time, then repeating every ten seconds ten times:
            trigger = TriggerBuilder.Create()
                .WithIdentity("trigger3", "group1")
                .StartAt(myTimeToStartFiring) // if a start time is not given (if this line were omitted), "now" is implied
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(10).WithRepeatCount(10)) // note that 10 repeats will give a total of 11 firings
                .ForJob(myJob) // identify job with handle to its JobDetail itself                   
                .Build();

            //3 Build a trigger that will fire once, five minutes in the future:

            trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity("trigger5", "group1")
                .StartAt(DateBuilder.FutureDate(5, IntervalUnit.Minute)) // use DateBuilder to create a date in the future
                .ForJob(myJobKey) // identify job with its JobKey
                .Build();

            //4 Build a trigger that will fire now, then repeat every five minutes, until the hour 22:00:
            trigger = (ISimpleTrigger) TriggerBuilder.Create()
                .WithIdentity("trigger7", "group1")
                .WithSimpleSchedule(x => x.WithIntervalInMinutes(5).RepeatForever())
                .EndAt(DateBuilder.DateOf(22, 0, 0))
                .Build();

            //5 Build a trigger that will fire at the top of the next hour, then repeat every 2 hours, forever:
            trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity("trigger8") // because group is not specified, "trigger8" will be in the default group
                .StartAt(DateBuilder.EvenHourDate(null)) // get the next even-hour (minutes and seconds zero ("00:00"))
                .WithSimpleSchedule(x => x.WithIntervalInHours(2).RepeatForever())
                // note that in this example, 'forJob(..)' is not called 
                //  - which is valid if the trigger is passed to the scheduler along with the job  
                .Build();
            scheduler.scheduleJob(trigger, job);

            /* 
             * MisfireInstruction.IgnoreMisfirePolicy
             * MisfirePolicy.SimpleTrigger.FireNow
             * MisfirePolicy.SimpleTrigger.RescheduleNowWithExistingRepeatCount
             * MisfirePolicy.SimpleTrigger.RescheduleNowWithRemainingRepeatCount
             * MisfirePolicy.SimpleTrigger.RescheduleNextWithRemainingCount
             * MisfirePolicy.SimpleTrigger.RescheduleNextWithExistingCount
             */
            trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity("trigger7", "group1")
                .WithSimpleSchedule(x => x.WithIntervalInMinutes(5)
                    .RepeatForever().WithMisfireHandlingInstructionNextWithExistingCount())
                    .Build();
        }
    }
}