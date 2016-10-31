using System;
using System.IO;
using System.Threading;
using Quartz;
using Quartz.Impl;
using Topshelf.Logging;

namespace Ks.Batch.Contribution.Copere.Out
{
    public class JobScheduler
    {
        private static readonly LogWriter Log = HostLogger.Get<JobScheduler>();
        private FileSystemWatcher _watcher;

        #region Start, Stop, Pause,Continue

        public bool Start()
        {
            var schedFact = new StdSchedulerFactory();
            var sched = schedFact.GetScheduler();
            sched.Start();

            var job = JobBuilder.Create<InfoJob>()
                .WithIdentity("ExportData", "ACMR").Build();

            var trigger = TriggerBuilder.Create()
              .WithIdentity("CheckData", "ACMR").StartNow()
              .WithSimpleSchedule(x => x.WithIntervalInMinutes(1).RepeatForever())
              .Build();

            sched.ScheduleJob(job, trigger);

            return true;
        }

        public bool Stop()
        {
            _watcher.Dispose();
            return true;
        }

        public bool Pause()
        {
            _watcher.EnableRaisingEvents = false;
            return true;
        }

        public bool Continue()
        {
            _watcher.EnableRaisingEvents = true;
            return true;
        }

        #endregion

        private void FileCreated(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(1000 * 10); //10 Sec because is not atomic

            try
            {
                Log.InfoFormat("Starting Reading file '{0}'", e.FullPath);

            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error in Reading file:'{0}'", e.FullPath);
                Log.ErrorFormat("Exception: '{0}'", e.FullPath);
            }



            if (e.FullPath.Contains("bad_in"))
            {
                throw new NotSupportedException("Cannot convert");
            }

            var content = File.ReadAllText(e.FullPath);
            var upperContent = content.ToUpperInvariant();
            var dir = Path.GetDirectoryName(e.FullPath);
            var convertedFileName = Path.GetFileName(e.FullPath) + ".converted";
            if (dir != null)
            {
                var convertedPath = Path.Combine(dir, convertedFileName);
                File.WriteAllText(convertedPath, upperContent);
            }
        }

        public void CustomCommand(int commandNumber)
        {
            //128-255
            Log.InfoFormat("Starting Convertion of '{0}' ", commandNumber);
        }
    }
}
