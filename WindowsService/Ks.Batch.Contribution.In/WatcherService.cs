using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Topshelf.Logging;

namespace Ks.Batch.Contribution.In
{
    public class WatcherService
    {
        private static readonly LogWriter Log = HostLogger.Get<WatcherService>();
        private FileSystemWatcher _watcher;

        #region Start, Stop, Pause,Continue

        public bool Start()
        {
            _watcher = new FileSystemWatcher(@"c:\KS\ACMR\WinService\In", "*.txt");
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
            Thread.Sleep(1000*10); //10 Sec because is not atomic

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
