using Ks.Batch.Util;
using System;
using System.Configuration;
using System.IO;
using Topshelf.Logging;

namespace Ks.Batch.Ftp.Upload
{
    class BatchContainerFileSystemWatcher : IBatchContainer, IDisposable
    {
        private string _sysName { get; set; }
        private static readonly LogWriter Log = HostLogger.Get<BatchContainerFileSystemWatcher>();
        private FileSystemWatcher _watcher;
        private string _path;
        private string _ext;


        public BatchContainerFileSystemWatcher()
        {
            _sysName = ConfigurationManager.AppSettings["SysName"];
            _path = ConfigurationManager.AppSettings["Path"];
            _ext = ConfigurationManager.AppSettings["Ext"];
        }

        public bool Start()
        {
            try
            {
                Log.InfoFormat(LogMessages.BatchStart);
                lock (this)
                {
                    _watcher = new FileSystemWatcher
                    {
                        Path = _path,
                        Filter = _ext,
                        IncludeSubdirectories = false,
                        EnableRaisingEvents = true
                }; 
                    _watcher.Created += Watcher.FileCreated; 
                    Log.InfoFormat(LogMessages.BatchReadOk);
                }
            }
            catch (Exception e)
            {
                Log.InfoFormat(LogMessages.BatchReadError, e.Message);
            }
            return true;
        } 

        public bool Continue()
        {
            try
            {
                Log.InfoFormat(LogMessages.BatchContinue);
                _watcher.EnableRaisingEvents = true;
                Log.InfoFormat("Result: " + LogMessages.BatchContinueOk, _sysName);
            }
            catch (Exception e)
            {
                Log.InfoFormat("Result: " + LogMessages.BatchContinueError, _sysName, e.Message);
            }
            return true;
        }

        public bool Pause()
        {
            try
            {
                Log.InfoFormat(LogMessages.BatchPause);
                _watcher.EnableRaisingEvents = false;
                Log.InfoFormat("Result: " + LogMessages.BatchPauseOk, _sysName);
            }
            catch (Exception e)
            {
                Log.InfoFormat("Result: " + LogMessages.BatchPauseError, _sysName, e.Message);
            }
            return true;
        }

        public bool Stop()
        {
            try
            {
                Log.InfoFormat(LogMessages.BatchStop);
                _watcher.Dispose();
                Log.InfoFormat("Result: " + LogMessages.BatchStopOk, _sysName );
            }catch (Exception e)
            {
                Log.InfoFormat("Result: " + LogMessages.BatchStopError, _sysName, e.Message);
            }
            return true;
        }

        public void CustomCommand(int commandNumber)
        {
            //128-255
            Log.InfoFormat("Starting Convertion of '{0}' ", commandNumber);
        }

        public void Dispose()
        {
            _watcher.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
