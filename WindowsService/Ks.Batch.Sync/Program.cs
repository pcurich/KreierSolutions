using System;
using Ks.Batch.Util;
using Quartz;
using Topshelf;
using Topshelf.Logging;
using Topshelf.Quartz;

namespace Ks.Batch.Sync
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                HostFactory.Run(serviceConfig =>
                {
                    serviceConfig.UseNLog();
                    serviceConfig.Service<BatchContainer>(serviceInstance =>
                    {
                        serviceInstance.ConstructUsing(name => new BatchContainer());
                        serviceInstance.WhenStarted(execute => execute.Start());
                        serviceInstance.WhenStopped(execute => execute.Stop());
                        serviceInstance.WhenPaused(execute => execute.Pause());
                        serviceInstance.WhenContinued(execute => execute.Continue());
                        serviceInstance.WhenCustomCommandReceived(
                            (execute, hostControl, commandNumber) => execute.CustomCommand(commandNumber));

                        serviceInstance.ScheduleQuartzJob(q =>
                            q.WithJob(() =>
                                JobBuilder.Create<Job>().Build())
                                .AddTrigger(() =>
                                    TriggerBuilder.Create()
                                        .WithSimpleSchedule(builder => builder
                                            .WithIntervalInSeconds(40)
                                            .RepeatForever())
                                        .Build())
                            );
                    });

                    serviceConfig.EnableServiceRecovery(recoveryOption =>
                    {
                        recoveryOption.RestartService(1); //Un minuto para recuperarse
                    });

                    serviceConfig.SetServiceName("Ks.Batch.Sync");
                    serviceConfig.SetDisplayName("Ks Batch Sync");
                    serviceConfig.SetDescription("Process to sync the config file of each Batch Service");

                    serviceConfig.EnablePauseAndContinue();
                    serviceConfig.StartAutomatically();
                });
            }
            catch (Exception e)
            {
                var ex = e.Message;
            }
        }

         
    }
}
