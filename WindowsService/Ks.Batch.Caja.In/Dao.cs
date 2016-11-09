﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Ks.Batch.Util;
using Ks.Batch.Util.Model;

namespace Ks.Batch.Caja.In
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

            if (guid != null)
            {
                try
                {
                    Sql = " UPDATE Reports set Name=@Name, Value=@Value, PathBase=@PathBase,Source=@Source, DateUtc=@DateUtc" +
                          " WHERE [Key]=@Key";

                    Command = new SqlCommand(Sql, Connection);
                    Command.Parameters.AddWithValue("@Key", guid);
                    Command.Parameters.AddWithValue("@Name", string.Format("Archivo leido de la caja en el periodo - {0}", Batch.PeriodYear.ToString("0000") + Batch.PeriodMonth.ToString("00")));
                    Command.Parameters.AddWithValue("@Value", XmlHelper.Serialize2String(new List<Info>(infos)));
                    Command.Parameters.AddWithValue("@PathBase", Batch.PathBase);
                    Command.Parameters.AddWithValue("@Source", Batch.SystemName);
                    Command.Parameters.AddWithValue("@DateUtc", DateTime.UtcNow);

                    Command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Log.FatalFormat("Time: {0} Error: {1}", DateTime.Now, ex.Message);
                }
            }
        }

        #region Util

        private Guid? GetParentOut()
        {
            Guid? guid = null;
            try
            {
                Sql = " SELECT [Key]  FROM Reports " +
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
                return null;
            }
            return guid;
        }

        #endregion
    }
}
