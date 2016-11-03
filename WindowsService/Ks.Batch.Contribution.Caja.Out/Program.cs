using Ks.Batch.Util;
using Quartz;
using Topshelf;
using Topshelf.Quartz;

namespace Ks.Batch.Contribution.Caja.Out
{
    public class Program
    {
        public static void Main(string[] args)
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
                                        .WithIntervalInMinutes(2)
                                        .RepeatForever())
                                    .Build())
                            );
                });

                serviceConfig.EnableServiceRecovery(recoveryOption =>
                {
                    recoveryOption.RestartService(1); //Un minuto para recuperarse
                });

                serviceConfig.SetServiceName("Ks.Batch.Contribution.Caja.Out");
                serviceConfig.SetDisplayName("Ks Batch Contribution Caja Out");
                serviceConfig.SetDescription("Process to get the file with customer in retired state");

                serviceConfig.EnablePauseAndContinue();
                serviceConfig.StartAutomatically();
            });
        }
    }
}
