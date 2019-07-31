using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using Ks.Batch.Util;
using Ks.Batch.Util.Model;

namespace Ks.Batch.Merge
{
    public class Watcher
    {
        private static List<Info> _copereOut;
        private static List<Info> _copereIn;
        private static List<Info> _cajaOut;
        private static List<Info> _cajaIn;
        private static Report _reportCopere;
        private static Report _reportCaja;
        private static bool isPre;

        public static void FileCreated(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(1000*3); //3 Sec because is not atomic
            var connection = ConfigurationManager.ConnectionStrings["ACMR"].ConnectionString;
            var SysName = ConfigurationManager.AppSettings["SysName"];

            var dao = new Dao(connection);
            dao.Connect(); 

            if (e.Name.ToUpper().Contains("PRE"))
                isPre = true;
            else
                isPre = false;

            var batch = dao.GetScheduleBatch(SysName);
            if (!isPre)
            {
                batch.Enabled = true;
                batch.LastExecutionOnUtc = DateTime.UtcNow;
                dao.UpdateScheduleBatch(batch);
            }

            //Data to calculate
            Dictionary<Report, List<Info>> listData = dao.GetData();

            //load _copereOut or _copereIn or _cajaOut or _cajaIn
            if (listData.Count == 2)
                SplitList(listData);

            var account = batch.SystemName + "." + batch.PeriodYear + "." + batch.PeriodMonth.ToString("D2");
            if (_copereOut != null && _copereIn != null && _copereOut.Count > 0 && _copereIn.Count > 0 && e.Name.ToUpper().Contains("COPERE"))
                dao.Process(_reportCopere, _copereIn, _copereOut, "Copere", account, isPre);
            if (_cajaOut != null && _cajaIn != null && _cajaOut.Count > 0 && _cajaIn.Count > 0 && e.Name.ToUpper().Contains("CAJA"))
                dao.Process(_reportCaja, _cajaIn, _cajaOut, "Caja", account, isPre);

            if (!isPre)
            {
                batch.Enabled = false;
                batch.LastExecutionOnUtc = DateTime.UtcNow;
                dao.UpdateScheduleBatch(batch);
            }

            dao.Close();

            File.Delete(e.FullPath);
        }

        #region Util

        protected static void SplitList(Dictionary<Report, List<Info>> listData)
        {
            foreach (var data in listData)
            {
                switch (data.Key.Source)
                {
                    case "Ks.Batch.Copere.Out":
                        {
                            _copereOut = data.Value;
                            _reportCopere = data.Key;
                            break;
                        }
                    case "Ks.Batch.Copere.In":
                        {
                            _copereIn = data.Value;
                            _reportCopere = data.Key;
                            break;
                        }
                    case "Ks.Batch.Caja.Out":
                        {
                            _cajaOut = data.Value;
                            _reportCaja = data.Key;
                            break;
                        }
                    case "Ks.Batch.Caja.In":
                        {
                            _cajaIn = data.Value;
                            _reportCaja = data.Key;
                            break;
                        }
                }
            }
        }

        #endregion
    }
}