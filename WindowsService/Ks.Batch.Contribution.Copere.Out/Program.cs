using Topshelf;

namespace Ks.Batch.Contribution.Copere.Out
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

                serviceConfig.SetServiceName("AwesomeFileConverted");
                serviceConfig.SetDisplayName("Awesome File Converted");
                serviceConfig.SetDescription("A pluralsight demo service");

                serviceConfig.EnablePauseAndContinue();
                serviceConfig.StartAutomatically();
            });
        }
    }
}
