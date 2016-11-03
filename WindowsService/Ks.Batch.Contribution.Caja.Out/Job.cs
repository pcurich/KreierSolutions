using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Ks.Batch.Util;
using Quartz;

namespace Ks.Batch.Contribution.Caja.Out
{
    public class Job : IJob
    {
        private ScheduleBatch Batch { get; set; }
        private string Path { get; set; }
        private string Connection { get; set; }

        public void Execute(IJobExecutionContext context)
        {
            Path = ConfigurationManager.AppSettings["Path"];
            Connection = ConfigurationManager.ConnectionStrings["ACMR"].ConnectionString;
            Batch = XmlHelper.Deserialize<ScheduleBatch>(System.IO.Path.Combine(Path, "ScheduleBatch.xml"));
            var records = DataBase();
            SyncFiles(records);
        }

        #region Util

        protected List<string> DataBase()
        {
            var dao = new Dao(Connection);
            dao.Connect();
            var scheduleBatchs = dao.Process(Batch);
            dao.Close();
            return scheduleBatchs;
        }

        public void SyncFiles(List<string> result)
        {
            var monthName = new DateTime(Batch.PeriodYear, Batch.PeriodMonth, 1).ToString("MMMM");
            var nameFile = string.Format("CPMP_{0}_{1}_6008_{2}.txt", Batch.PeriodYear, Batch.PeriodMonth, monthName);
            File.WriteAllLines(System.IO.Path.Combine(Path, nameFile), result);
        }

        #endregion
    }
}