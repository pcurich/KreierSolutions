using System.Configuration;
using Quartz;
using Topshelf;
using Topshelf.Quartz;

namespace Ks.Batch.Ftp.Upload
{
    class Program
    {
        static void Main(string[] args)
        {
            bool isJob = bool.Parse(ConfigurationManager.AppSettings["IsJob"]);
            int interval = int.Parse(ConfigurationManager.AppSettings["Interval"]);
            string sysName = ConfigurationManager.AppSettings["SysName"];
            string showName = ConfigurationManager.AppSettings["ShowName"];
            string customer = ConfigurationManager.AppSettings["Customer"];
            bool isDebug = bool.Parse(ConfigurationManager.AppSettings["IsDebug"]);

            if (isDebug)
            {
                var job = new Job();
                job.Execute(null);
            }
            else
            {
                HostFactory.Run(serviceConfig =>
                {
                    serviceConfig.UseNLog();

                    if (isJob)
                    {
                        serviceConfig.Service<BatchContainerScheduleQuartzJob>(serviceInstance =>
                        {
                            serviceInstance.ConstructUsing(name => new BatchContainerScheduleQuartzJob());
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
                                            .WithIntervalInSeconds(30)
                                            .RepeatForever())
                                        .Build())
                            );
                        });
                    }
                    else
                    {
                        serviceConfig.Service<BatchContainerFileSystemWatcher>(serviceInstance =>
                        {
                            serviceInstance.ConstructUsing(name => new BatchContainerFileSystemWatcher());
                            serviceInstance.WhenStarted(execute => execute.Start());
                            serviceInstance.WhenStopped(execute => execute.Stop());
                            serviceInstance.WhenPaused(execute => execute.Pause());
                            serviceInstance.WhenContinued(execute => execute.Continue());
                            serviceInstance.WhenCustomCommandReceived(
                                (execute, hostControl, commandNumber) => execute.CustomCommand(commandNumber));
                        });
                    }

                    serviceConfig.EnableServiceRecovery(recoveryOption =>
                    {
                        recoveryOption.RestartService(1); //Un minuto para recuperarse
                    });

                    serviceConfig.SetServiceName(sysName);
                    serviceConfig.SetDisplayName(showName);
                    serviceConfig.SetDescription(string.Format("{0}{1}", customer, sysName));

                    serviceConfig.EnablePauseAndContinue();
                    serviceConfig.StartAutomatically();
                });
            }
        }
    }
}
