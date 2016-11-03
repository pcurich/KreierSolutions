using System;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Calendar;

namespace Ks.Batch.Test.Quartz.Net
{
    public class Lesson4
    {

        public void Test4()
        {
            //More About Triggers
            //Here is a listing of properties common to all trigger types:
            //The JobKey property indicates the identity of the job that should be executed when the trigger fires.
            //El StartTimeUtc indica cuando se disparara el trigger. El valor es un DateTimeOffset que define el 
            //momento en que se dispara el trigger
            //El EndTimeUtc  la ultima vez que se ejecutará

            //Los Calendars  son utiles para excluir bloques de tiempo  del programador 
            //Por ejemplo, se crea un trigger que dispara todos los miercoles alas 9:00 am pero se agrega un calendar
            //que excluye un bloque de feriados

            ISchedulerFactory schedFact = new StdSchedulerFactory();

            // get a scheduler
            IScheduler sched = schedFact.GetScheduler();
            sched.Start();

            HolidayCalendar cal = new HolidayCalendar();
            cal.AddExcludedDate(DateTime.Now);

            sched.AddCalendar("myHolidays", cal, false, false);


            ITrigger t1 = TriggerBuilder.Create()
                .WithIdentity("myTrigger")
                .ForJob("myJob")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(9, 30)) // execute job daily at 9:30
                .ModifiedByCalendar("myHolidays") // but not on holidays
                .Build();

            ITrigger t2 = TriggerBuilder.Create()
                    .WithIdentity("myTrigger2")
                    .ForJob("myJob2")
                    .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(11, 30)) // execute job daily at 11:30
                    .ModifiedByCalendar("myHolidays") // but not on holidays
                    .Build();
        }

        public interface ICalendar
        {
            string Description { get; set; }

            ICalendar CalendarBase { set; get; }

            bool IsTimeIncluded(DateTimeOffset timeUtc);

            DateTime GetNextIncludedTimeUtc(DateTimeOffset timeUtc);

        }
    }
}