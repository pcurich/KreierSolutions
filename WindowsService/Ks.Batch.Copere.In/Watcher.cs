using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using Ks.Batch.Util;
using Ks.Batch.Util.Model;
using Topshelf.Logging;

namespace Ks.Batch.Copere.In
{
    public class Watcher
    {
        private static readonly LogWriter Log = HostLogger.Get<Watcher>();
        private static ScheduleBatch Batch { get; set; }
        private static ServiceSetting ServiceSetting { get; set; }

        public static void FileCreated(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(1000 * 3); //10 Sec because is not atomic

            ReadSetting(e.Name);

            if (!(ServiceSetting.ContributionCode == "" && ServiceSetting.LoanCode == "")) {
                try
                {
                    var infos = InfoService.ReadFile(e.FullPath, ServiceSetting.DefaultCulture, ServiceSetting.ContributionCode != "", ServiceSetting.LoanCode != "");
                    InsertData(infos);
                    UpdateScheduleBatch();
                    MoveFile(e.FullPath, e.Name);
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("Error in Reading file:'{0}'", e.FullPath);
                    Log.ErrorFormat("Exception: '{0}'", ex.Message);
                }
            }
        }

        public void CustomCommand(int commandNumber)
        {
            //128-255
            //sc control ProceesName commandNumber
            Log.InfoFormat("This is '{0}' ", commandNumber);
        }

        #region Util

        private static void InsertData(List<Info> infos)
        {
            var dao = new Dao(ServiceSetting.Connection);
            dao.Process(Batch, infos);
        }

        protected static void UpdateScheduleBatch()
        {
            var dao = new Dao(ServiceSetting.Connection);
            dao.Connect();
            Batch.LastExecutionOnUtc = DateTime.UtcNow;
            dao.UpdateScheduleBatch(Batch);
            dao.Close();
            Log.InfoFormat("7.- UpdateScheduleBatch");
        }

        private static void MoveFile(string fullPath, string fileName)
        {
            var destiny = Path.Combine(Path.Combine(ServiceSetting.Path, Batch.FolderMoveToDone), fileName);
            if (File.Exists(destiny))
                File.Delete(destiny);
            File.Move(fullPath, Path.Combine(Path.Combine(ServiceSetting.Path, Batch.FolderMoveToDone), fileName));
        }

        private static void ReadSetting(string fileName)
        {
            //EC.ACT.NOMINAL_8001_102019.text            
            fileName = fileName.Substring(0, fileName.Length - 4);
            var length = fileName.Length;

            Log.InfoFormat("1.- Read File: {0} ", fileName);

            ServiceSetting = new ServiceSetting
            {
                Path = ConfigurationManager.AppSettings["Path"],
                DefaultCulture = ConfigurationManager.AppSettings["DefaultCulture"],
                Connection = ConfigurationManager.ConnectionStrings["ACMR"].ConnectionString,
                SysName = ConfigurationManager.AppSettings["SysName"],
                IsUnique= bool.Parse(ConfigurationManager.AppSettings["IsUnique"]),
                ContributionCode = ConfigurationManager.AppSettings["ContributionCode"],
                LoanCode = ConfigurationManager.AppSettings["LoanCode"],
                FileYear = fileName.Substring(length - 4, 4),
                FileMonth = fileName.Substring(length - 6, 2),
            };

            var dao = new Dao(ServiceSetting.Connection);
            dao.Connect();
            Batch = dao.GetScheduleBatch(ServiceSetting.SysName);
            Batch.PeriodYear = Convert.ToInt32(ServiceSetting.FileYear);
            Batch.PeriodMonth = Convert.ToInt32(ServiceSetting.FileMonth);
            Batch.StartExecutionOnUtc = DateTime.UtcNow;
            dao.Close();

            if (fileName.Contains(ServiceSetting.ContributionCode) && !fileName.Contains(ServiceSetting.LoanCode))            
                ServiceSetting.LoanCode = "";
            
            if (!fileName.Contains(ServiceSetting.ContributionCode) && fileName.Contains(ServiceSetting.LoanCode))
                ServiceSetting.ContributionCode = "";

            if (!fileName.Contains(ServiceSetting.ContributionCode) && !fileName.Contains(ServiceSetting.LoanCode))
            {
                ServiceSetting.ContributionCode = "";
                ServiceSetting.LoanCode = "";
            }

            Log.InfoFormat("2.- SysName: {0} | ContributionCode: {1} | LoanCode: {2} ", ServiceSetting.SysName, ServiceSetting.ContributionCode, ServiceSetting.LoanCode);
        }
        #endregion
    }
}
