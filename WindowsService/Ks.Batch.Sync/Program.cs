using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace Ks.Batch.Sync
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(serviceConfig =>
            {
                serviceConfig.UseNLog();
                serviceConfig.Service<JobScheduler>(serviceInstance =>
                {
                    serviceInstance.ConstructUsing(() => new JobScheduler());
                    serviceInstance.WhenStarted(execute => execute.Start());
                    serviceInstance.WhenStopped(execute => execute.Stop());
                    serviceInstance.WhenPaused(execute => execute.Pause());
                    serviceInstance.WhenContinued(execute => execute.Continue());
                    serviceInstance.WhenCustomCommandReceived(
                        (execute, hostControl, commandNumber) => execute.CustomCommand(commandNumber));
                });

                serviceConfig.EnableServiceRecovery(recoveryOption =>
                {
                    //recoveryOption.RunProgram(1, @"SendEmail.exe");
                    recoveryOption.RestartService(1); //Un minuto para recuperarse
                    //recoveryOption.RestartComputer(2, "Adios");
                });

                serviceConfig.SetServiceName("Ks.Batch.Sync");
                serviceConfig.SetDisplayName("Ks Batch Sync");
                serviceConfig.SetDescription("Process to sync the config file of each Batch Service");

                serviceConfig.EnablePauseAndContinue();
                serviceConfig.StartAutomatically();
            });
        }
    }
}
