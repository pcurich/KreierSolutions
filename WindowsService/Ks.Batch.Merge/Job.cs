using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Quartz;
using Ks.Batch.Util;
using Ks.Batch.Util.Model;

namespace Ks.Batch.Merge
{
    public class Job : IJob
    {
        List<Info> _copereOut;
        List<Info> _copereIn;
        List<Info> _cajaOut;
        List<Info> _cajaIn;
        Reports _reportCopere;
        Reports _reportCaja;
        string _connection;

        public void Execute(IJobExecutionContext context)
        {
            _connection = ConfigurationManager.ConnectionStrings["ACMR"].ConnectionString;
            var dao = new Dao(_connection);
            var listData = dao.GetData();

            SplitList(listData);

            if (_copereOut != null && _copereIn != null && _copereOut.Count > 0 && _copereIn.Count > 0)
                dao.ProcessCopere(_reportCopere,_copereIn, _copereOut);
            if (_cajaOut != null && _cajaIn != null && _cajaOut.Count > 0 && _cajaIn.Count > 0)
                dao.ProcessCaja(_cajaIn, _cajaOut);

        }

        #region Util

        protected void SplitList(Dictionary<Reports, List<Info>> listData)
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