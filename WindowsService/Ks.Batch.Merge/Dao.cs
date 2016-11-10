using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Ks.Batch.Util;
using Ks.Batch.Util.Model;
using Topshelf.Logging;

namespace Ks.Batch.Merge
{
    public class Dao : DaoBase
    {
        private static readonly LogWriter Log = HostLogger.Get<Dao>();



        public Dao(string connetionString)
            : base(connetionString)
        {
        }

        public Dictionary<Reports, List<Info>> GetData()
        {
            Log.InfoFormat("Time: {0}: Action: {1}", DateTime.Now, "Ks.Batch.Merge.Dao.GetData()");

            var infoList = new Dictionary<Reports, List<Info>>();
            try
            {
                Connect();
                if (IsConnected)
                {
                    Sql = "SELECT * FROM Reports WHERE ParentKey in " +
                          " (SELECT ParentKey  FROM Reports WHERE StateId=" + (int)ReportState.InProcess +
                          "  and len(name)<>0  " +
                          "group by ParentKey having count(ParentKey)=2) ";

                    Command = new SqlCommand(Sql, Connection);
                    var sqlReader = Command.ExecuteReader();
                    while (sqlReader.Read())
                    {
                        infoList.Add(new Reports
                        {
                            Id = sqlReader.GetInt32(0),
                            Key = sqlReader.GetGuid(1),
                            Name = sqlReader.GetString(2),
                            Value = sqlReader.GetString(3),
                            StateId = sqlReader.GetInt32(5),
                            Period = sqlReader.GetString(6),
                            Source = sqlReader.GetString(7),
                            ParentKey = sqlReader.GetGuid(8),
                            DateUtc = sqlReader.GetDateTime(9)
                        },
                        XmlHelper.XmlToObject<List<Info>>(sqlReader.GetString(3)));
                    }
                    sqlReader.Close();
                }
                return infoList;
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Time: {0}: Action: {1}", DateTime.Now, ex.Message);
                Close();
                return null;

            }

        }


        public void ProcessCopere(List<Info> infoIn, List<Info> infoOut)
        {
            List<Info> infoEquals = new List<Info>(); //aountIn == amountOut  Pago completo
            List<Info> infoNotIn = new List<Info>(); //aountIn >0 aountOut ==0 No tiene liquidez
            List<Info> infoLoss = new List<Info>(); //aountIn >0 aountOut >0 pago por puchos 

            foreach (var info in infoIn)
            {
                var info1 = info;
                var _info = infoOut.FirstOrDefault(x => x.AdminCode == info1.AdminCode);
                if (_info == null)
                    infoNotIn.Add(info);
                else
                    if (info.Total == _info.Total)
                        infoEquals.Add(info);
                    else
                        infoLoss.Add(info);
            }
        }

        public void ProcessCaja(List<Info> infoIn, List<Info> infoOut)
        {
            throw new NotImplementedException();
        }
    }
}