using Ks.Batch.Util;
using System;
using System.Configuration;
using Topshelf.Logging;

namespace Ks.Batch.Ftp.Upload
{
    class BatchContainerScheduleQuartzJob : IBatchContainer, IDisposable
    {
        private string _sysName { get; set; }
        private static readonly LogWriter Log = HostLogger.Get<BatchContainerScheduleQuartzJob>();

        public BatchContainerScheduleQuartzJob()
        {
            _sysName = ConfigurationManager.AppSettings["SysName"];
        }

        public bool Start()
        {
            try
            {
                Log.InfoFormat(string.Format(LogMessages.BatchStart), _sysName); 
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
                Log.InfoFormat(string.Format(LogMessages.BatchContinue), _sysName);
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
                Log.InfoFormat(string.Format(LogMessages.BatchPause), _sysName);
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
                Log.InfoFormat(string.Format(LogMessages.BatchStop), _sysName);
            }
            catch (Exception e)
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
            GC.SuppressFinalize(this);
        }
    }
}
