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
        private static string Path { get; set; }
        private static string SysName { get; set; }
        private static string Connection { get; set; }

        public static void FileCreated(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(1000 * 3); //10 Sec because is not atomic
            Path = ConfigurationManager.AppSettings["Path"];
            Connection = ConfigurationManager.ConnectionStrings["ACMR"].ConnectionString;

            SysName = ConfigurationManager.AppSettings["SysName"];

            var dao = new Dao(Connection);
            dao.Connect();
            Batch = dao.GetScheduleBatch(SysName);
            dao.Close();

            Batch.PeriodYear = Convert.ToInt32(e.Name.Split(' ')[1]);
            Batch.PeriodMonth = Convert.ToInt32(e.Name.Split(' ')[2].Substring(0, 2));

            try
            {
                Log.InfoFormat("Starting Reading file '{0}'", e.FullPath);
                var infos = InfoService.ReadFile(e.FullPath);
                Log.InfoFormat("File readed with '{0}' records", infos.Count);
                InsertData(infos);
                UpdateScheduleBatch();
                MoveFile(e.FullPath, e.Name);
                //WakeUpMerge();

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

        private static void WakeUpMerge()
        {
            var path = ConfigurationManager.AppSettings["WakeUp"];
            path = System.IO.Path.Combine(path, "CajaWakeUp.txt");
            if (!File.Exists(path))
            {
                using (var myFile = File.Create(path))
                {
                    TextWriter tw = new StreamWriter(myFile);
                    tw.WriteLine("CajaWakeUp");
                    tw.Close();
                }
            }
        }

        private static void InsertData(List<Info> infos)
        {
            var dao = new Dao(Connection);
            dao.Connect();
            dao.Process(Batch, infos);
            dao.Close();
        }

        protected static void UpdateScheduleBatch(bool executed = true)
        {
            var dao = new Dao(Connection);
            dao.Connect();

            if (executed)
            {
                if (Batch.NextExecutionOnUtc.HasValue)
                    Batch.NextExecutionOnUtc = Batch.FrecuencyId == 30 ?
                        Batch.NextExecutionOnUtc.Value.AddMonths(Batch.FrecuencyId) :
                        Batch.NextExecutionOnUtc.Value.AddDays(Batch.FrecuencyId);
            }

            Batch.LastExecutionOnUtc = DateTime.UtcNow;
            dao.UpdateScheduleBatch(Batch);
            dao.Close();

        }

        private static void MoveFile(string fullPath, string fileName)
        {
            File.Move(fullPath, System.IO.Path.Combine(System.IO.Path.Combine(Path, Batch.FolderMoveToDone), fileName));
        }

        #endregion
    }
}
