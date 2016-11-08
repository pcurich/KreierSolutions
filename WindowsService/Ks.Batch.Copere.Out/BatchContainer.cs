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
        public string PathValue;
        public ScheduleBatch Batch;
        
        public bool Start()
        {
            Read();
            Install();
            Log.InfoFormat("Time: {0}: Action: {1}", DateTime.Now, "Service Start");
            Enabled();
            return true;
        }

        public bool Stop()
        {
            UnInstall();
            Log.InfoFormat("Time: {0}: Action: {1}", DateTime.Now, "Service Stop");
            return true;
        }

        public bool Pause()
        {
            Log.InfoFormat("Time: {0}: Action: {1}", DateTime.Now, "Service Pause");
            Disabled();
            return true;
        }

        public bool Continue()
        {
            Log.InfoFormat("Time: {0}: Action: {1}", DateTime.Now, "Service Continue");
            Enabled();
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
            dao.Install(Batch);
        }

        private void UnInstall()
        {
            var dao = new Dao(Connection);
            dao.UnInstall(Batch.SystemName);
        }

        private void Enabled()
        {
            var dao = new Dao(Connection);
            dao.Enabled(Batch.SystemName);
        }

        private void Disabled()
        {
            var dao = new Dao(Connection);
            dao.Disabled(Batch.SystemName);
        }

        private void Read()
        {
            Connection = ConfigurationManager.ConnectionStrings["ACMR"].ConnectionString;
            PathValue = ConfigurationManager.AppSettings["Path"];
            Batch = XmlHelper.Deserialize<ScheduleBatch>(System.IO.Path.Combine(PathValue, "ScheduleBatch.xml"));
        }
    }
}