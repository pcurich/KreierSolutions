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
        private static string Path { get; set; }
        private static string Connection { get; set; }

        public static void FileCreated(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(1000 * 10); //10 Sec because is not atomic
            Path = ConfigurationManager.AppSettings["Path"];
            Connection = ConfigurationManager.ConnectionStrings["ACMR"].ConnectionString;
            Batch = XmlHelper.Deserialize<ScheduleBatch>(System.IO.Path.Combine(Path, "ScheduleBatch.xml"));

            var file = e.Name.Substring(0, e.Name.Length - 4);
            var length = file.Length;
            var year = file.Substring(length - 4, 4);
            var month = file.Substring(length - 6, 2);

            Batch.PeriodYear = Convert.ToInt32(year);
            Batch.PeriodMonth = Convert.ToInt32(month);

            try
            {
                Log.InfoFormat("Starting Reading file '{0}'", e.FullPath);
                var infos = InfoService.ReadFile(e.FullPath);
                Log.InfoFormat("File readed with '{0}' records", infos.Count);
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
            var dao = new Dao(Connection);
            dao.Connect();
            dao.Process(Batch, infos);
            dao.Close();
        }

        protected static void UpdateScheduleBatch(bool executed = true)
        {
            var dao = new Dao(Connection);

            if (executed)
            {
                if (Batch.NextExecutionOnUtc.HasValue)
                    Batch.NextExecutionOnUtc = Batch.FrecuencyId == 30 ?
                        Batch.NextExecutionOnUtc.Value.AddMonths(Batch.FrecuencyId) :
                        Batch.NextExecutionOnUtc.Value.AddDays(Batch.FrecuencyId);
            }

            Batch.LastExecutionOnUtc = DateTime.UtcNow;
            dao.UpdateScheduleBatch(Batch);
        }

        private static void MoveFile(string fullPath, string fileName)
        {
            File.Move(fullPath, System.IO.Path.Combine(System.IO.Path.Combine(Path, Batch.FolderMoveToDone), fileName));
        }
        #endregion
    }
}
