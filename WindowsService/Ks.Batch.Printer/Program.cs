using System.Globalization;
using Topshelf;

namespace Ks.Batch.Printer
{
    class Program
    {
        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-PE");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("es-PE");

            HostFactory.Run(serviceConfig =>
            {
                serviceConfig.UseNLog();
                serviceConfig.Service<BatchContainer>(serviceInstance =>
                {
                    serviceInstance.ConstructUsing(() => new BatchContainer());
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

                serviceConfig.SetServiceName("Ks.Batch.Printer");
                serviceConfig.SetDisplayName("Ks Batch Printer");
                serviceConfig.SetDescription(@"Printer from c printer queve");

                serviceConfig.EnablePauseAndContinue();
                serviceConfig.StartAutomatically();
            }); 
        }
    }
}
