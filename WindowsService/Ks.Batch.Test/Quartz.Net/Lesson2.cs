using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace Ks.Batch.Test.Quartz.Net
{
    /// The key interfaces and classes of the Quartz API are:
    /// IScheduler - La API principal que interactua con un scheduler.
    /// IJob - interfaz que implementa el componente que se dedea ejecutar por el scheduler.
    /// IJobDetail - usado para definir intancias de Jobs.
    /// ITrigger -un componente que define el horario en que se ejecuta un Job.
    /// JobBuilder - usado para definir/crear instancias de JobDetail, que define instancias de Jobs.
    /// TriggerBuilder - Utilizado para definir / crear instancias de Trigger.
    class Lesson2
    {
        //Jobs And Triggers
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

        private ISimpleTrigger _simpleTrigger;
        private ICronTrigger _cronTrigger;

        //Cuando el trigger del job se dispara, se invoca el metodo Execute(..) por uno de los hilos worker del schedule

        //JobDetail Es el objeto creado por Quartz.Net en el momento en que el Job es agregado al scheduler, 
        //posee JobDataMap que puede ser utilizado para almacenar informacion en una instancia 

        //Los Trigger son usados para siparar la ejecucion, tambien tienen asociados un JobDataMap con ellos
        //el cual pasa parametros al Job los cuales son especificos de la ejecucion del disparador.
        //Los diferentes tipos de disparadores son: ISimpleTrigger y ICronTrigger

        //ISimpleTrigger: se dispara una vez en un determinado momento o N veces con un retraso de T entre cada ejecucion
        //ICronTrigger: cuando se desea calendarizar los disparadores, ejemplo, cada viernes en la medianoche, 
        //o a las 15:15 del 10 de cada mes 

    }
}
