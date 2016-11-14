using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Ks.Batch.Util;
using Quartz;

namespace Ks.Batch.Caja.Out
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

            if (Batch.NextExecutionOnUtc.HasValue && DateTime.Now >=
                DateTimeHelper.ConvertToUserTime(Batch.NextExecutionOnUtc.Value, TimeZoneInfo.Utc))
            {
                if (!ExistFile())
                {
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
            else
                UpdateScheduleBatch(false);
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
            var nameFile = string.Format("6008_{0}00.txt", Batch.PeriodYear.ToString("0000") + Batch.PeriodMonth.ToString("00"));
            File.WriteAllLines(System.IO.Path.Combine(System.IO.Path.Combine(Path, Batch.FolderMoveToDone), nameFile), result);
        }

        protected bool ExistFile()
        {
            var nameFile = string.Format("6008_{0}00.txt", Batch.PeriodYear.ToString("0000") + Batch.PeriodMonth.ToString("00"));
            return File.Exists(System.IO.Path.Combine(System.IO.Path.Combine(Path, Batch.FolderMoveToDone), nameFile));
        }

        protected void UpdateScheduleBatch(bool executed = true)
        {
            var dao = new Dao(Connection);
            dao.Connect();
            if (executed)
            {
                if (Batch.NextExecutionOnUtc.HasValue)
                    Batch.NextExecutionOnUtc = Batch.FrecuencyId == 30 ?
                        Batch.NextExecutionOnUtc.Value.AddMonths(Batch.FrecuencyId) :
                        Batch.NextExecutionOnUtc.Value.AddDays(Batch.FrecuencyId);

                if (Batch.PeriodMonth == 12)
                {
                    Batch.PeriodMonth = 1;
                    Batch.PeriodYear++;
                }
                else
                    Batch.PeriodMonth++;
            }

            Batch.LastExecutionOnUtc = DateTime.UtcNow;
            dao.UpdateScheduleBatch(Batch);
            dao.Close();
        }

        #endregion
    }
}