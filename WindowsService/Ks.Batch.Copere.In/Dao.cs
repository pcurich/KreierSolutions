using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Ks.Batch.Util;
using Ks.Batch.Util.Model;

namespace Ks.Batch.Copere.In
{
    public class Dao : DaoBase
    {
        private ScheduleBatch Batch { get; set; }

        public Dao(string connetionString)
            : base(connetionString)
        {
        } 

        public void Process(ScheduleBatch batch, List<Info> infos)
        {
            Batch = batch;
            var guid = GetParentOut();

            if (guid == null)
                return;

            var oldData = GetReportChild(guid.Value, Batch.SystemName, Batch.PeriodYear.ToString("0000") + Batch.PeriodMonth.ToString("00"));

            if (oldData == null || oldData.Count == 0)
                Log.InfoFormat("5.- First Time to load total: {0} | Amount Total: {1} ", oldData.Count, oldData.Sum(x => x.TotalPayed));
            else
            {
                infos.AddRange(oldData);
                Log.InfoFormat("5.- Second time to load Record: {0} | Amount Total: {1} ", infos.Count, infos.Sum(x => x.TotalPayed));
            }
             
            infos = JoinData(infos);
            Log.InfoFormat("6.- join data Record: {0} | Amount Total: {1} | Contribution: {2} | Loan: {3}", infos.Count, infos.Sum(x => x.TotalPayed), infos.Sum(x => x.TotalContribution), infos.Sum(x => x.TotalLoan));



            if (guid != null)
            {
                try
                {
                    Connect();
                    Sql = " UPDATE Report set Name=@Name, StateId=@StateId, " +
                          " Value=@Value, PathBase=@PathBase,Source=@Source, DateUtc=@DateUtc" +
                          " WHERE [Key]=@Key";

                    Command = new SqlCommand(Sql, Connection);
                    Command.Parameters.AddWithValue("@Key", guid);
                    Command.Parameters.AddWithValue("@Name", string.Format("Archivo leido por el Coopere en el periodo - {0}", Batch.PeriodYear.ToString("0000") + Batch.PeriodMonth.ToString("00")));
                    Command.Parameters.AddWithValue("@Value", XmlHelper.Serialize2String(new List<Info>(infos)));
                    Command.Parameters.AddWithValue("@PathBase", Batch.PathBase);
                    Command.Parameters.AddWithValue("@StateId", (int)ReportState.InProcess);
                    Command.Parameters.AddWithValue("@Source", Batch.SystemName);
                    Command.Parameters.AddWithValue("@DateUtc", DateTime.UtcNow);

                    Command.ExecuteNonQuery();
                    Log.InfoFormat("7.- Insert into Report with key {0}", guid);
                    Close();
                }
                catch (Exception ex)
                {
                    Log.FatalFormat("Action: {0} Error: {1}","Dao.Process(" + batch.SystemName + ")", ex.Message);
                    Close();
                }
            }
        }

        #region Util

        private Guid? GetParentOut()
        {
            Guid? guid = null;
            try
            {
                Connect();
                Log.InfoFormat("Action: {0}","Dao.GetParentOut()");

                Sql = " SELECT [Key]  FROM Report " +
                      " WHERE Source=@Source AND " +
                      " Period=@Period";

                Command = new SqlCommand(Sql, Connection);
                Command.Parameters.AddWithValue("@Source", Batch.SystemName);
                Command.Parameters.AddWithValue("@Period", Batch.PeriodYear.ToString("0000") + Batch.PeriodMonth.ToString("00"));

                var sqlReader = Command.ExecuteReader();
                while (sqlReader.Read())
                {
                    guid = sqlReader.GetGuid(0);
                }
                sqlReader.Close(); 
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}","Dao.GetParentOut()", ex.Message);
                return null;
            }
            Close();
            return guid;
        } 
        #endregion
    }
}