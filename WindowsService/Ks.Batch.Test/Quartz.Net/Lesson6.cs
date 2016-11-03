using System;
using Quartz;

namespace Ks.Batch.Test.Quartz.Net
{
    public class Lesson6
    {
        public void Test6()
        {
            //With CronTrigger, you can specify firing-schedules such as “every Friday at noon”, 
            //or “every weekday and 9:30 am”, or 
            //even “every 5 minutes between 9:00 am and 10:00 am on every Monday, Wednesday and Friday”.

            // “0 0 12 ? * WED” - which means “every Wednesday at 12:00 pm”.
            // could be replaces with “MON-FRI”, “MON, WED, FRI”, or even “MON-WED,SAT”.

            //Wild-cards (the ‘* character) can be used to say “every” possible value of this field.
            //Therefore the ‘*’ character in the “Month” field of the previous example simply means “every month”. 
            //A ‘*’ in the Day-Of-Week field would obviously mean “every day of the week”.

            /*
             * All of the fields have a set of valid values that can be specified. 
             * These values should be fairly obvious - such as the numbers 0 to 59 for seconds and minutes, 
             * and the values 0 to 23 for hours. Day-of-Month can be any value 0-31, 
             * but you need to be careful about how many days are in a given month! 
             * Months can be specified as values between 0 and 11, or by using the strings 
             * JAN, FEB, MAR, APR, MAY, JUN, JUL, AUG, SEP, OCT, NOV and DEC. 
             * Days-of-Week can be specified as vaules between 1 and 7 (1 = Sunday) or by using the strings 
             * SUN, MON, TUE, WED, THU, FRI and SAT.
             */

            /*
             * The ‘/’ character can be used to specify increments to values. 
             * For example, if you put ‘0/15’ in the Minutes field, it means ‘every 15 minutes, starting at minute zero’. 
             * If you used ‘3/20’ in the Minutes field, it would mean ‘every 20 minutes during the hour, 
             * starting at minute three’ - or in other words it is the same as specifying 
             * ‘3,23,43’ in the Minutes field.
             */

            /*
             * The ‘?’ character is allowed for the day-of-month and day-of-week fields. 
             * It is used to specify “no specific value”. 
             * This is useful when you need to specify something in one of the two fields, 
             * but not the other. See the examples below (and CronTrigger API documentation) for clarification.
             */
                
            /*
             * The ‘L’ character is allowed for the day-of-month and day-of-week fields. 
             * This character is short-hand for “last”, but it has different meaning in each of the two fields. 
             * For example, the value “L” in the day-of-month field means “the last day of the month” - 
             * day 31 for January, day 28 for February on non-leap years. 
             * If used in the day-of-week field by itself, it simply means “7” or “SAT”. 
             * But if used in the day-of-week field after another value, it means “the last xxx day of the month” 
             * - for example “6L” or “FRIL” both mean “the last friday of the month”.
             * When using the ‘L’ option, it is important not to specify lists, or ranges of values, 
             * as you’ll get confusing results.
             */

            /*
             * The ‘W’ is used to specify the weekday (Monday-Friday) nearest the given day. 
             * As an example, if you were to specify “15W” as the value for the day-of-month field, 
             * the meaning is: “the nearest weekday to the 15th of the month”.
             */

            /*
             * The ‘#’ is used to specify “the nth” XXX weekday of the month. For example, the value of “6#3” or “FRI#3” 
             * in the day-of-week field means “the third Friday of the month”.
             */

            //CronTrigger Example 1 - an expression to create a trigger that simply fires every 5 minutes
            //"0 0/5 * * * ?"

            //CronTrigger Example 2 - an expression to create a trigger that fires every 5 minutes, at 10 seconds after the minute 
            //(i.e. 10:00:10 am, 10:05:10 am, etc.).
            //"10 0/5 * * * ?"

            //CronTrigger Example 3 - an expression to create a trigger that fires at 10:30, 11:30, 12:30,and 13:30
            //on every Wednesday and Friday.
            //"0 30 10-13 ? * WED,FRI"

            //CronTrigger Example 4 - an expression to create a trigger that fires every half hour between 
            //the hours of 8 am and 10 am on the 5th and 20th of every month. 
            //Note that the trigger will NOT fire at 10:00 am, just at 8:00, 8:30, 9:00 and 9:30
            //"0 0/30 8-9 5,20 * ?"

            //Build a trigger that will fire every other minute, between 8am and 5pm, every day:
            var trigger = TriggerBuilder.Create()
                .WithIdentity("trigger3", "group1")
                .WithCronSchedule("0 0/2 8-17 * * ?")
                .ForJob("myJob", "group1")
                .Build();
            
            //Build a trigger that will fire daily at 10:42 am:
            trigger = TriggerBuilder.Create()
                .WithIdentity("trigger3", "group1")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(10, 42)) //.WithCronSchedule("0 42 10 * * ?")
                .ForJob(myJobKey)
                .Build();

            //Build a trigger that will fire on Wednesdays at 10:42 am, in a TimeZone other than the system’s default:
            trigger = TriggerBuilder.Create()
                .WithIdentity("trigger3", "group1")
                .WithSchedule(CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(DayOfWeek.Wednesday, 10, 42)//"0 42 10 ? * WED"
                .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time")))
                .ForJob(myJobKey)
                .Build();


        }
    }
}