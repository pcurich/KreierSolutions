using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace Ks.Batch.Contribution.Out
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(serviceConfig =>
            {
                serviceConfig.UseNLog();
                serviceConfig.Service<WatcherService>(serviceInstance =>
                {
                    serviceInstance.ConstructUsing(() => new WatcherService());
                    serviceInstance.WhenStarted(execute => execute.Start());
                    serviceInstance.WhenStopped(execute => execute.Stop());
                    serviceInstance.WhenPaused(execute => execute.Pause());
                    serviceInstance.WhenContinued(execute => execute.Continue());
                    serviceInstance.WhenCustomCommandReceived(
                        (execute, hostControl, commandNumber) => execute.CustomCommand(commandNumber));
                });

                serviceConfig.EnableServiceRecovery(recoveryOption =>
                {
                    recoveryOption.RestartService(1); //Un minuto para recuperarse
                });

                serviceConfig.SetServiceName("KsBatchContributionOut");
                serviceConfig.SetDisplayName("Ks Batch Contribution Out");
                serviceConfig.SetDescription(@"This process read a file from COPERE and Update data base. Those Files must be in c:\KS\ACMR\WinService\Out");

                serviceConfig.EnablePauseAndContinue();
                serviceConfig.StartAutomatically();
            });
        }
    }
}
