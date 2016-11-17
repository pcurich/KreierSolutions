using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using Ks.Batch.Util.Model;

namespace Ks.Batch.Merge
{
    public class Watcher
    {
        private static List<Info> _copereOut;
        private static List<Info> _copereIn;
        private static List<Info> _cajaOut;
        private static List<Info> _cajaIn;
        private static Reports _reportCopere;
        private static Reports _reportCaja;

        public static void FileCreated(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(1000*3); //3 Sec because is not atomic
            var connection = ConfigurationManager.ConnectionStrings["ACMR"].ConnectionString;

            var dao = new Dao(connection);
            dao.Connect();
            var listData = dao.GetData();

            if (listData.Count == 2)
                SplitList(listData);

            if (_copereOut != null && _copereIn != null && _copereOut.Count > 0 && _copereIn.Count > 0)
                dao.Process(_reportCopere, _copereIn, _copereOut);
            if (_cajaOut != null && _cajaIn != null && _cajaOut.Count > 0 && _cajaIn.Count > 0)
                dao.Process(_reportCaja, _cajaIn, _cajaOut);
            dao.Close();

            File.Delete(e.FullPath);
        }

        #region Util

        protected static void SplitList(Dictionary<Reports, List<Info>> listData)
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