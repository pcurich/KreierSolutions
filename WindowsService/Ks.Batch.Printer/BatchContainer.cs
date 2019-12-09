using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf.Logging;

namespace Ks.Batch.Printer
{
    public class BatchContainer : IBatchContainer
    {
        private static readonly LogWriter Log = HostLogger.Get<BatchContainer>();
        private FileSystemWatcher _watcher;

        public bool Start()
        {
            Read();
            Log.InfoFormat("Time: {0}; Action: {1}; ", DateTime.Now, "BatchContainer.Start()");
            return true;
        }
        public bool Stop()
        {
            Log.InfoFormat("Time: {0}; Action: {1}; ", DateTime.Now, "BatchContainer.Stop()");
            return true;
        }
        public bool Continue()
        {
            Log.InfoFormat("Time: {0}; Action: {1}; ", DateTime.Now, "BatchContainer.Continue()");
            return true;
        }

        public void CustomCommand(int commandNumber)
        {
            //128-255
            Log.InfoFormat("Starting Convertion of '{0}' ", commandNumber);
        }

        public bool Pause()
        {
            Log.InfoFormat("Time: {0}; Action: {1}; ", DateTime.Now, "BatchContainer.Pause()");
            return true;
        }

        private void Read()
        {
            _watcher = new FileSystemWatcher(Path.Combine(@"c:/print/", "queve"), "*.json");
            _watcher.Created += Watcher.FileCreated;
            _watcher.IncludeSubdirectories = false;
            _watcher.EnableRaisingEvents = true;
        }
    }
}
