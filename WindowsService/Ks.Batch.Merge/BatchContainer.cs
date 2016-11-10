using System;
using System.Configuration;
using Ks.Batch.Util;
using Topshelf.Logging;

namespace Ks.Batch.Merge
{
    public class BatchContainer : IBatchContainer
    {
        private static readonly LogWriter Log = HostLogger.Get<BatchContainer>();
        public string Connection;
        //public string PathValue;
        //public ScheduleBatch Batch;

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
            Connection = ConfigurationManager.ConnectionStrings["ACMR"].ConnectionString;
            //PathValue = ConfigurationManager.AppSettings["Path"];
            //Batch = XmlHelper.Deserialize<ScheduleBatch>(Path.Combine(PathValue, "ScheduleBatch.xml"));
        }
    }
}