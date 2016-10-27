using System;
using System.IO;
using System.Threading;
using Topshelf.Logging;

namespace Ks.Batch.Contribution.Out
{
    public class WatcherService
    {
        private static readonly LogWriter Log = HostLogger.Get<WatcherService>();
        private FileSystemWatcher _watcher;

        #region Start, Stop, Pause,Continue

        public bool Start()
        {
            _watcher = new FileSystemWatcher(@"c:\KS\ACMR\WinService\Out", "*.txt");
            _watcher.Created += FileCreated;
            _watcher.IncludeSubdirectories = false;
            _watcher.EnableRaisingEvents = true;

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

                var infos=InfoService.ReadFile(e.FullPath);
                Log.InfoFormat("File readed with '{0}' records", infos.Count);
                
                var dao = new DaoService();
                dao.Connect();
                Log.InfoFormat("Data Base Connect Successfull");

                var number=dao.Process(infos);
                Log.InfoFormat("Records Process {0}",number);

                dao.Close();
                Log.InfoFormat("Process Done");
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error in Reading file:'{0}'", e.FullPath);
                Log.ErrorFormat("Exception: '{0}'", ex.Message);
            }
 
        }

        public void CustomCommand(int commandNumber)
        {
            //128-255
            //sc control ProceesName commandNumber
            Log.InfoFormat("This is '{0}' ", commandNumber);
        }
    }
}
