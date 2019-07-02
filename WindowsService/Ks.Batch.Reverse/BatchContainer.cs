using System;
using System.Configuration;
using System.IO;
using Ks.Batch.Util;
using Topshelf.Logging;

namespace Ks.Batch.Reverse
{
    public class BatchContainer : IBatchContainer
    {
        private static readonly LogWriter Log = HostLogger.Get<BatchContainer>();
        private FileSystemWatcher _watcher;
        public string Connection;
        public string PathValue;
        public string SysName;
        public ScheduleBatch Batch;

        public bool Start()
        {
            try
            {
                Read();
                Install();
                Log.InfoFormat("Result: " + LogMessages.BatchStartOk, Batch.SystemName);
            }catch (Exception e)
            {
                Log.InfoFormat("Result: " + LogMessages.BatchStartError, Batch.SystemName, e.Message);
            }

            return true;
        }

        public bool Stop()
        {
            try
            {
                _watcher.Dispose();
                Log.InfoFormat("Result: " + LogMessages.BatchStopOk, Batch.SystemName);
            }
            catch (Exception e)
            {
                Log.InfoFormat("Result: " + LogMessages.BatchStopError, Batch.SystemName, e.Message);
            }
            return true;
        }

        public bool Pause()
        {
            try
            {
                _watcher.EnableRaisingEvents = false;
                Log.InfoFormat("Result: " + LogMessages.BatchPauseOk, Batch.SystemName);
            }
            catch (Exception e)
            {
                Log.InfoFormat("Result: " + LogMessages.BatchPauseError, Batch.SystemName, e.Message);
            }
            return true;
        }

        public bool Continue()
        {
            try
            {
                _watcher.EnableRaisingEvents = true;
                Log.InfoFormat("Result: " + LogMessages.BatchContinueOk, Batch.SystemName);
            }
            catch (Exception e)
            {
                Log.InfoFormat("Result: " + LogMessages.BatchContinueError, Batch.SystemName, e.Message);
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
            try
            {
                lock (this)
                {
                    Connection = ConfigurationManager.ConnectionStrings["ACMR"].ConnectionString;
                    PathValue = ConfigurationManager.AppSettings["Path"];
                    SysName = ConfigurationManager.AppSettings["SysName"];

                    var dao = new Dao(Connection);
                    dao.Connect();
                    Batch = dao.GetScheduleBatch(SysName);

                    _watcher = new FileSystemWatcher(PathValue, "*.txt");

                    _watcher.Created += Watcher.FileCreated;
                    _watcher.IncludeSubdirectories = false;
                    _watcher.EnableRaisingEvents = true;

                    Log.InfoFormat(LogMessages.BatchReadOk);
                }
            }
            catch (Exception e)
            {
                Log.InfoFormat(LogMessages.BatchReadError, e.Message);
            }

        }

        private void Install()
        {
            var dao = new Dao(Connection);
            dao.Connect();
            dao.Install(Batch);
            dao.Close();
        }

        #endregion
    }
}
