using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using Ks.Batch.Util;
using Ks.Batch.Util.Model;
using Topshelf.Logging;

namespace Ks.Batch.Caja.In
{
    public class Watcher
    {
        private static readonly LogWriter Log = HostLogger.Get<Watcher>();
        private static ScheduleBatch Batch { get; set; }
        private static ServiceSetting ServiceSetting { get; set; }

        public static void FileCreated(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(1000 * 3); //10 Sec because is not atomic

            ReadServiceSetting(e.Name);
            ReadBatchService();

            try
            {
                var infos = InfoService.ReadFile(e.FullPath,ServiceSetting.DefaultCulture);
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

        private static void ReadServiceSetting(string fileName)
        {
            //CPMP 2019 05_6008 MAYO            
            Log.InfoFormat("1.- Read File: {0} ", fileName);

            ServiceSetting = new ServiceSetting
            {
                Path = ConfigurationManager.AppSettings["Path"],
                DefaultCulture = ConfigurationManager.AppSettings["DefaultCulture"],
                Connection = ConfigurationManager.ConnectionStrings["ACMR"].ConnectionString,
                SysName = ConfigurationManager.AppSettings["SysName"],
                IsUnique = bool.Parse(ConfigurationManager.AppSettings["IsUnique"]),
                ContributionCode = ConfigurationManager.AppSettings["ContributionCode"],
                LoanCode = ConfigurationManager.AppSettings["LoanCode"],
                FileYear = fileName.Split(' ')[1],
                FileMonth = fileName.Split(' ')[2].Substring(0, 2),
            };

            Log.InfoFormat("2.- [ReadServiceSetting]: SysName: {0} | ContributionCode: {1} | LoanCode: {2} ", ServiceSetting.SysName, ServiceSetting.ContributionCode, ServiceSetting.LoanCode);
        }

         private static void ReadBatchService()
         {
            var dao = new Dao(ServiceSetting.Connection);
            dao.Connect();
            Batch = dao.GetScheduleBatch(ServiceSetting.SysName);
            Batch.PeriodYear = Convert.ToInt32(ServiceSetting.FileYear);
            Batch.PeriodMonth = Convert.ToInt32(ServiceSetting.FileMonth);
            Batch.StartExecutionOnUtc = DateTime.UtcNow;
            dao.Close();
         }
        #endregion
    }
}
