using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Quartz;
using Ks.Batch.Util;

namespace Ks.Batch.Sync
{
    public class Job : IJob
    {
        string PathValue { get; set; }

        public void Execute(IJobExecutionContext context)
        {
            PathValue = ConfigurationManager.AppSettings["Path"];
            var records = DataBase();
            SyncFiles(records);
        }

        #region Util

        protected List<ScheduleBatch> DataBase()
        {
            var connection = ConfigurationManager.ConnectionStrings["ACMR"].ConnectionString;
            var dao = new Dao(connection);
            dao.Connect();
            var scheduleBatchs = dao.Process();
            dao.Close();
            return scheduleBatchs;
        }

        public void SyncFiles(List<ScheduleBatch> scheduleBatchs)
        {
            var dirs = Directory.GetFiles(PathValue, "*ScheduleBatch.xml", SearchOption.AllDirectories);

            foreach (var scheduleBatch in scheduleBatchs)
            {
                var batch = scheduleBatch;
                var file = dirs.FirstOrDefault(x => x.Contains(batch.SystemName));
                if (file == null)
                    continue;

                var old = XmlHelper.Deserialize<ScheduleBatch>(file);

                if (old.Equals(scheduleBatch))
                    continue;

                File.Delete(file);
                XmlHelper.Serialize(scheduleBatch, file);
            }
        }

        #endregion

    }
}