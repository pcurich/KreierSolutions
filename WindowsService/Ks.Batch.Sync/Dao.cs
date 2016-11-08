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
                            PathBase = sqlReader.GetString(3),
                            FolderRead = sqlReader.GetString(4),
                            FolderLog = sqlReader.GetString(5),
                            FolderMoveToDone = sqlReader.GetString(6),
                            FolderMoveToError = sqlReader.GetString(7),
                            FrecuencyId = sqlReader.GetInt32(8),
                            PeriodYear = sqlReader.GetInt32(9),
                            PeriodMonth = sqlReader.GetInt32(10),
                            Enabled = sqlReader.GetBoolean(14)
                        };
                        if (!sqlReader.IsDBNull(11))
                            schedule.StartExecutionOnUtc = sqlReader.GetDateTime(11);
                        if (!sqlReader.IsDBNull(12))
                            schedule.NextExecutionOnUtc = sqlReader.GetDateTime(12);
                        if (!sqlReader.IsDBNull(13))
                            schedule.LastExecutionOnUtc = sqlReader.GetDateTime(13);

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