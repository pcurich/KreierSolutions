using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Ks.Batch.Util;
using Quartz;
using Topshelf.Logging;

namespace Ks.Batch.Copere.Out
{
    public class Job : IJob
    {
        public static readonly LogWriter Log = HostLogger.Get<Job>();
        private ScheduleBatch Batch { get; set; }
        private string Path { get; set; }
        public string SysName;
        private string Connection { get; set; }

        public void Execute(IJobExecutionContext context)
        {
            Path = ConfigurationManager.AppSettings["Path"];
            Connection = ConfigurationManager.ConnectionStrings["ACMR"].ConnectionString;

            SysName = ConfigurationManager.AppSettings["SysName"];

            var dao = new Dao(Connection);
            dao.Connect();
            Batch = dao.GetScheduleBatch(SysName);
            dao.Close();


            if (Batch.Enabled)
            {
                ExistFile();

                var records = DataBase();
                if (records.Count != 0)
                {
                    SyncFiles(records);
                    UpdateScheduleBatch();
                }
                else
                {
                    UpdateScheduleBatch(false);
                }
            }

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

        protected void SyncFiles(List<string> result)
        {
            try
            {
                var nameFile = string.Format("8001_{0}00.txt", Batch.PeriodYear.ToString("0000") + Batch.PeriodMonth.ToString("00"));
                File.WriteAllLines(System.IO.Path.Combine(System.IO.Path.Combine(Path, Batch.FolderMoveToDone), nameFile), result);
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "Job.SyncFiles()", ex.Message);
            }
        }

        protected void ExistFile()
        {
            var nameFile = string.Format("8001_{0}00.txt", Batch.PeriodYear.ToString("0000") + Batch.PeriodMonth.ToString("00"));
            try
            {
                Log.InfoFormat("Action: {0}", "Job.ExistFile()");
                if (File.Exists(System.IO.Path.Combine(System.IO.Path.Combine(Path, Batch.FolderMoveToDone), nameFile)))
                    File.Delete(System.IO.Path.Combine(System.IO.Path.Combine(Path, Batch.FolderMoveToDone), nameFile));

            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "Job.ExistFile(" + nameFile + ")", ex.Message);
            }
        }

        protected void UpdateScheduleBatch(bool executed = true)
        {
            var dao = new Dao(Connection);
            dao.Connect();
            Log.InfoFormat("Action: Start{0}", "Job.UpdateScheduleBatch(executed=" + executed + " ,Batch=" + Batch.ToString() + ")");
            if (executed && Batch.UpdateData)
            {
                if (Batch.NextExecutionOnUtc != null)
                    Batch.NextExecutionOnUtc = Batch.NextExecutionOnUtc.Value.AddSeconds(30);

                if (Batch.PeriodMonth == 12)
                {
                    Batch.PeriodMonth = 1;
                    Batch.PeriodYear++;
                }
                else
                    Batch.PeriodMonth++;
            }

            Batch.LastExecutionOnUtc = DateTime.UtcNow;
            Batch.Enabled = false;
            Batch.UpdateData = false;
            dao.UpdateScheduleBatch(Batch);
            Log.InfoFormat("Action: End{0}", "Job.UpdateScheduleBatch(executed=" + executed + " ,Batch=" + Batch.ToString() + ")");
            dao.Close();
        }

        #endregion
    }
}