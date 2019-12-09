using System;
using System.Configuration;
using Ks.Batch.Util;
using Topshelf.Logging;

namespace Ks.Batch.Copere.Out
{
    public class BatchContainer : IBatchContainer
    {
        private static readonly LogWriter Log = HostLogger.Get<BatchContainer>();
        public string Connection;
        public string SysName;
        public ScheduleBatch Batch;
        
        public bool Start()
        {
            Read();
           // Install();
            Log.InfoFormat("Time: {0}; Action: {1}; ", DateTime.Now, "BatchContainer.Start()");
            //Enabled();
            return true;
        }

        public bool Stop()
        {
            //UnInstall();
            Log.InfoFormat("Time: {0}; Action: {1}; ", DateTime.Now, "BatchContainer.Stop()");
            return true;
        }

        public bool Pause()
        {
            Log.InfoFormat("Time: {0}; Action: {1}; ", DateTime.Now, "BatchContainer.Pause()");
            //Disabled();
            return true;
        }

        public bool Continue()
        {
            Log.InfoFormat("Time: {0}; Action: {1}; ", DateTime.Now, "BatchContainer.Continue()");
            //Enabled();
            return true;
        }

        public void CustomCommand(int commandNumber)
        {
            //128-255
            Log.InfoFormat("Starting Convertion of '{0}' ", commandNumber);
        }

        private void Install()
        {
            var dao = new Dao(Connection);
            dao.Connect();
            dao.Install(Batch);
            dao.Close();
        }

        private void UnInstall()
        {
            var dao = new Dao(Connection);
            dao.Connect();
            dao.UnInstall(Batch.SystemName);
            dao.Close();
        }

        private void Enabled()
        {
            var dao = new Dao(Connection);
            dao.Connect();
            dao.Enabled(Batch.SystemName);
            dao.Close();
        }

        private void Disabled()
        {
            var dao = new Dao(Connection);
            dao.Connect();
            dao.Disabled(Batch.SystemName);
            dao.Close();
        }

        private void Read()
        {
            Connection = ConfigurationManager.ConnectionStrings["ACMR"].ConnectionString;
            SysName = ConfigurationManager.AppSettings["SysName"];

            var dao = new Dao(Connection);
            dao.Connect();
            Batch = dao.GetScheduleBatch(SysName);
            dao.Close();
        }
    }
}