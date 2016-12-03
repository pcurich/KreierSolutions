using Quartz;
using Topshelf;
using Topshelf.Quartz;

namespace Ks.Batch.Copere.Out
{
    class Program
    {
        static void Main(string[] args)
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
                                JobBuilder.Create<Job>()
                                .Build())
                            .AddTrigger(() =>
                                TriggerBuilder.Create()
                                    .WithSimpleSchedule(builder => builder
                                        .WithIntervalInSeconds(60)
                                        .RepeatForever())
                                    .Build())
                            );
                });

                serviceConfig.EnableServiceRecovery(recoveryOption =>
                {
                    recoveryOption.RestartService(1); //Un minuto para recuperarse
                });

                serviceConfig.SetServiceName("Ks.Batch.Copere.Out");
                serviceConfig.SetDisplayName("Ks Batch Copere Out");
                serviceConfig.SetDescription("Process to get the file with customer in activity state to Copere");

                serviceConfig.EnablePauseAndContinue();
                serviceConfig.StartAutomatically();
            });

            //var job = new Job();
            //job.Execute(null);
        }
    }
}
