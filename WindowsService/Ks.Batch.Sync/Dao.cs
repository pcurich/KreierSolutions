using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Ks.Batch.Util;

namespace Ks.Batch.Sync
{
    public class Dao : DaoBase
    {
        public Dao(string connetionString)
            : base(connetionString)
        {

        }

        public List<ScheduleBatch> Process()
        {
            if (IsConnected)
            {
                var result = new List<ScheduleBatch>();
                try
                {
                    Log.InfoFormat("Time: {0}: Action: {1}", DateTime.Now, "Start to Select");
                    Sql = "Select * from ScheduleBatch";
                    Command = new SqlCommand(Sql, Connection);
                    var sqlReader = Command.ExecuteReader();
                    while (sqlReader.Read())
                    {
                        var schedule = new ScheduleBatch
                        {
                            Id = sqlReader.GetInt32(0),
                            Name = sqlReader.GetString(1),
                            SystemName = sqlReader.GetString(2),
                            PathRead = sqlReader.GetString(3),
                            PathLog = sqlReader.GetString(4),
                            PathMoveToDone = sqlReader.GetString(5),
                            PathMoveToError = sqlReader.GetString(6),
                            FrecuencyId = sqlReader.GetInt32(7),
                            PeriodYear = sqlReader.GetInt32(8),
                            PeriodMonth = sqlReader.GetInt32(9),
                            Enabled = sqlReader.GetBoolean(13)
                        };
                        if (!sqlReader.IsDBNull(8))
                            schedule.StartExecutionOnUtc = sqlReader.GetDateTime(10);
                        if (!sqlReader.IsDBNull(9))
                            schedule.NextExecutionOnUtc = sqlReader.GetDateTime(11);
                        if (!sqlReader.IsDBNull(10))
                            schedule.LastExecutionOnUtc = sqlReader.GetDateTime(12);

                        result.Add(schedule);
                    }

                    sqlReader.Close();

                    Log.InfoFormat("Time: {0}: Action: {1}", DateTime.Now, result.Count + " records readed");
                    return result;
                }
                catch (Exception ex)
                {
                    Log.FatalFormat("Time: {0} Error: {1}", DateTime.Now, ex.Message);
                    return null;
                }
            }
            else
            {
                //todo send message to brayan?
                return null;
            }

        }
    }
}