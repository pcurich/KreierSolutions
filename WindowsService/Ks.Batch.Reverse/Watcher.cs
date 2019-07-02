using Ks.Batch.Util;
using System;
using System.Configuration;
using System.IO;
using System.Threading;
using Topshelf.Logging;

namespace Ks.Batch.Reverse
{
    public  class Watcher
    {
        public static readonly LogWriter Log = HostLogger.Get<Watcher>();

        public string SysName { get; set; }
        private string Connection { get; set; }

        public static void FileCreated(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(1000 * 3); //3 Sec because is not atomic
            var connection = ConfigurationManager.ConnectionStrings["ACMR"].ConnectionString;
            var SysName = ConfigurationManager.AppSettings["SysName"];

            var dao = new Dao(connection);
            dao.Connect();

            var batch = dao.GetScheduleBatch(SysName);
            var month = batch.PeriodMonth;
            var year = batch.PeriodYear;

            if(month == 1)
            {
                month = 12;
                year--;
            }
            else
            {
                month--;
            }
            dao.RevertDataContribution(year, month);
            dao.RevertDataLoan(year, month);

            UpdateScheduleBatch(dao, batch, month, year);
        }

        protected static void UpdateScheduleBatch(Dao dao,ScheduleBatch batch,int month, int year)
        {
            Log.InfoFormat("Action: ScheduleBatch valor inicial = {0}", batch.ToString());

            
            if (batch.NextExecutionOnUtc != null)
                batch.NextExecutionOnUtc = batch.NextExecutionOnUtc.Value.AddSeconds(30);

            batch.PeriodMonth = month;
            batch.PeriodYear = year;
            

            batch.LastExecutionOnUtc = DateTime.UtcNow;

            Log.InfoFormat("Action: ScheduleBatch valor final = {0}", batch.ToString());
            dao.UpdateScheduleBatch(batch);
            dao.Close();
        }
    }
}
