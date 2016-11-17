using System;
using System.Configuration;
using System.IO;
using Ks.Batch.Util;
using Topshelf.Logging;

namespace Ks.Batch.Merge
{
    public class BatchContainer : IBatchContainer
    {
        private static readonly LogWriter Log = HostLogger.Get<BatchContainer>();
        private FileSystemWatcher _watcher;
        public ScheduleBatch Batch;
        public string PathValue;

        public bool Start()
        {
            Read();
            Log.InfoFormat("Time: {0}: Action: {1}", DateTime.Now, "Service Start");
            return true;
        }

        public bool Stop()
        {
            Log.InfoFormat("Time: {0}: Action: {1}", DateTime.Now, "Service Stop");
            return true;
        }

        public bool Pause()
        {
            Log.InfoFormat("Time: {0}: Action: {1}", DateTime.Now, "Service Pause");
            return true;
        }

        public bool Continue()
        {
            Log.InfoFormat("Time: {0}: Action: {1}", DateTime.Now, "Service Continue");
            return true;
        }

        public void CustomCommand(int commandNumber)
        {
            //128-255
            Log.InfoFormat("Starting Convertion of '{0}' ", commandNumber);
        }

        private void Read()
        {
            PathValue = ConfigurationManager.AppSettings["Path"];

            _watcher = new FileSystemWatcher(PathValue, "*.txt");
            _watcher.Created += Watcher.FileCreated;
            _watcher.IncludeSubdirectories = false;
            _watcher.EnableRaisingEvents = true;
        }
    }
}