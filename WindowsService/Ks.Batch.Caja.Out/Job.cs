using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Ks.Batch.Util;
using Quartz;
using Topshelf.Logging;

namespace Ks.Batch.Caja.Out
{
    public class Job : IJob
    {
        public static readonly LogWriter Log = HostLogger.Get<Job>();

        private ScheduleBatch Batch { get; set; }
        private string Path { get; set; }
        public string SysName { get; set; }
        private string Connection { get; set; }

        public void Execute(IJobExecutionContext context)
        {
            Path = ConfigurationManager.AppSettings["Path"];
            Connection = ConfigurationManager.ConnectionStrings["ACMR"].ConnectionString;
            SysName = ConfigurationManager.AppSettings["SysName"];

            Log.InfoFormat("************************************** -- " + SysName + "-- **************************************");

            var dao = new Dao(Connection);
            dao.Connect();
            Batch = dao.GetScheduleBatch(SysName);
            dao.Close();

            if (!FileHelper.IsBusy(Batch.PathBase)){
                FileHelper.CreateBusyFile(Batch.PathBase);

                if (Batch.Enabled)
                {
                    Log.InfoFormat("Action: {0} {1}", Batch.SystemName, "Activo");
                    ExistFile();

                    var records = DataBase();
                    if (records.Count != 0)
                    {
                        SyncFiles(records, Batch.UpdateData);
                        UpdateScheduleBatch();
                    }
                    else
                    {
                        UpdateScheduleBatch(false);
                    }
                }
                else
                {
                    Log.InfoFormat("Action: {0} {1}", Batch.SystemName, "No Activo");
                    FileHelper.DeleteBusyFile(Batch.PathBase);
                }
            }
            FileHelper.PurgeFile(Batch.PathBase);
        }

        #region Util

        protected List<string> DataBase()
        {
            var dao = new Dao(Connection);
            dao.Connect();
            Log.InfoFormat("Action: {0}", "Inicia Proceso de extraccion");
            var scheduleBatchs = dao.Process(Path,Batch);
            dao.Close();
            return scheduleBatchs;
        }

        protected void SyncFiles(List<string> result, bool updateData)
        {
            try
            {
                var nameFile = string.Format("6008_{0}00.txt", Batch.PeriodYear.ToString("0000") + Batch.PeriodMonth.ToString("00"));
                Log.InfoFormat("Action: Escribiendo en el archivo {0} la cantidad de {1} lineas", nameFile, result.Count);
                File.WriteAllLines(System.IO.Path.Combine(System.IO.Path.Combine(Path, Batch.FolderMoveToDone), nameFile), result);
                ZipHelper.CreateZipFile(System.IO.Path.Combine(Path, Batch.FolderMoveToDone), nameFile.Replace(".txt","")+ (!updateData? "-previo" : ""));
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "Job.SyncFiles()", ex.Message);
            }
}

        protected void ExistFile()
        {
            var nameFile = string.Format(
                "6008_{0}00.txt", 
                Batch.PeriodYear.ToString("0000") + 
                Batch.PeriodMonth.ToString("00"));

            try
            {
                Log.InfoFormat("Action: {0} {1}", "Buscando Archivo", nameFile);
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
            Log.InfoFormat("Action: ScheduleBatch valor inicial = {0}", Batch.ToString());
            Log.InfoFormat("Action: executed = {0}", executed);

            var dao = new Dao(Connection);
            dao.Connect();
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

            Log.InfoFormat("Action: ScheduleBatch valor final = {0}", Batch.ToString());
            dao.UpdateScheduleBatch(Batch);
            dao.Close();
        }

        #endregion
    }
}