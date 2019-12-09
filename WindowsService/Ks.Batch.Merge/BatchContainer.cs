using System;
using System.Configuration;
using System.IO;
using Ks.Batch.Util;
using Topshelf.Logging;

namespace Ks.Batch.Merge
{
    public class BatchContainer : IBatchContainer, IDisposable
    {
        private static readonly LogWriter Log = HostLogger.Get<BatchContainer>();
        private FileSystemWatcher _watcher;

        public bool Start()
        {
            try
            {
                Read();
                Install();
            }catch(Exception e)
            {
                Log.InfoFormat("Result: " + e.Message);
            }
            
            return true;
        }

        public bool Stop()
        {
            try
            {
                _watcher.Dispose();
                Log.InfoFormat("Result: " + LogMessages.BatchStopOk, "Merge");
            }catch(Exception e)
            {
                Log.InfoFormat("Result: " + LogMessages.BatchStopError, "Merge", e.Message);
            }
            return true;
        }

        public bool Pause()
        {
            try
            {
                _watcher.EnableRaisingEvents = false;
                Log.InfoFormat("Result: " + LogMessages.BatchPauseOk, "Merge");
            }
            catch(Exception e)
            {
                Log.InfoFormat("Result: " + LogMessages.BatchPauseError, "Merge", e.Message);
            }
            return true;
        }

        public bool Continue()
        {
            try
            {
                _watcher.EnableRaisingEvents = true;
                Log.InfoFormat("Result: " + LogMessages.BatchContinueOk, "Merge");
            }
            catch (Exception e)
            {
                Log.InfoFormat("Result: " + LogMessages.BatchContinueError, "Merge", e.Message);
            }
            return true;
        }

        public void CustomCommand(int commandNumber)
        {
            //128-255
            Log.InfoFormat("Starting Convertion of '{0}' ", commandNumber);
        }

        #region Utilities

        private void Read() 
        {
            try {
                lock (this)
                {
                    _watcher = new FileSystemWatcher(ConfigurationManager.AppSettings["Path"], "*.txt");
                    _watcher.Created += Watcher.FileCreated;
                    _watcher.IncludeSubdirectories = false;
                    _watcher.EnableRaisingEvents = true;

                    Log.InfoFormat(LogMessages.BatchReadOk);
                }
            } catch(Exception e)
            {
                Log.InfoFormat(LogMessages.BatchReadError,e.Message);
            }
            
        }

        private void Install()
        {
            var dao = new Dao(ConfigurationManager.ConnectionStrings["ACMR"].ConnectionString);
            dao.Connect();
            var batch = dao.GetScheduleBatch(ConfigurationManager.AppSettings["SysName"]);
            //TODO
            dao.Install(batch);
            dao.Close();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _watcher.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}