using Topshelf;

namespace Ks.Batch.Merge
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
                });

                serviceConfig.EnableServiceRecovery(recoveryOption =>
                {
                    recoveryOption.RestartService(1); //Un minuto para recuperarse
                });

                serviceConfig.SetServiceName("Ks.Batch.Merge");
                serviceConfig.SetDisplayName("Ks Batch Merge");
                serviceConfig.SetDescription("Process to Merge files");

                serviceConfig.EnablePauseAndContinue();
                serviceConfig.StartAutomatically();
            });
        }
    }
}
