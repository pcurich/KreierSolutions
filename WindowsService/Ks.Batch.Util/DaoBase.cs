using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Topshelf.Logging;

namespace Ks.Batch.Util
{
    public abstract class DaoBase : IDaoBase
    {
        public static readonly LogWriter Log = HostLogger.Get<DaoBase>();

        public string ConnetionString { get; private set; }
        public SqlConnection Connection;
        public SqlCommand Command;
        public string Sql;

        public bool IsConnected { get; set; }

        protected DaoBase(string connetionString)
        {
            ConnetionString = connetionString;
            IsConnected = false;
        }

        public void Connect()
        {
            if (!IsConnected)
            {
                try
                {
                    Log.InfoFormat("Action: {0}", "DaoBase.Connect()");
                    Connection = new SqlConnection(ConnetionString);
                    Connection.Open();
                    IsConnected = true;
                }
                catch (Exception ex)
                {
                    IsConnected = false;
                    Log.FatalFormat("Action: {0} Error: {1}", "DaoBase.Connect()", ex.Message);
                }
            }

        }

        public void Close()
        {
            if (IsConnected)
            {
                IsConnected = false;
                try
                {
                    Log.InfoFormat("Action: {0}", "DaoBase.Close()");
                    Command.Dispose();
                    Connection.Close();
                }
                catch (Exception ex)
                {
                    Log.FatalFormat("Action: {0} Error: {1}", "DaoBase.Close()", ex.Message);
                }
            }
        }

        public void Enabled(string systemName)
        {
            if (IsConnected)
                Exec(systemName, 1);
        }

        public void Disabled(string systemName)
        {
            if (IsConnected)
                Exec(systemName, 0);
        }

        public Dictionary<int, string> GetUserNames(List<int> customerIds)
        {
            var result = new Dictionary<int, string>();

            try
            {
                Log.InfoFormat("Action: {0}", "DaoBase.GetUserNames(" + string.Join(",", customerIds.ToArray()) + ")");

                Sql = "SELECT EntityId, Attribute =[Key], Value FROM GenericAttribute WHERE " +
                  " [Key] in ('FirstName','LastName') AND " +
                  " KeyGroup='Customer' AND EntityId IN (" + string.Join(",", customerIds.ToArray()) + ") " +
                  " ORDER BY EntityId ";

                Command = new SqlCommand(Sql, Connection);
                var sqlReader = Command.ExecuteReader();

                var count = 0;
                var firstName = "";
                var lastName = "";
                var entityId = 0;
                var repeatEntityId = 0;

                while (sqlReader.Read())
                {
                    if (sqlReader.GetString(1).Equals("FirstName"))
                    {
                        count++;
                        firstName = sqlReader.GetString(2);
                        entityId = sqlReader.GetInt32(0);
                    }
                    if (sqlReader.GetString(1).Equals("LastName"))
                    {
                        count++;
                        lastName = sqlReader.GetString(2);
                        repeatEntityId = sqlReader.GetInt32(0);
                    }
                    if (count == 2 && entityId == repeatEntityId)
                    {
                        result.Add(entityId, firstName + " " + lastName);
                        entityId = repeatEntityId = count = 0;
                    }
                }
                sqlReader.Close();
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "DaoBase.GetUserNames(" + string.Join(",", customerIds.ToArray()) + ")", ex.Message);
            }

            return result;
        }

        public void Install(ScheduleBatch batch)
        {
            try
            {
                Log.InfoFormat("Action: {0}", "DaoBase.Install(" + batch.SystemName + ")");
                if (!IsInstalled(batch.SystemName))
                {
                    Sql = "INSERT INTO ScheduleBatch " +
                          "(Name, SystemName, PathBase,FolderRead,FolderLog,FolderMoveToDone,FolderMoveToError, FrecuencyId, PeriodYear, PeriodMonth,Enabled) " +
                          "VALUES " +
                          "(@Name, @SystemName, @PathBase,@FolderRead,@FolderLog,@FolderMoveToDone,@FolderMoveToError, @FrecuencyId, @PeriodYear, @PeriodMonth, @Enabled) ";

                    var pName = new SqlParameter { ParameterName = "@Name", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = batch.Name };
                    var pSystemName = new SqlParameter { ParameterName = "@SystemName", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = batch.SystemName };
                    var pPathBase = new SqlParameter { ParameterName = "@PathBase", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = batch.PathBase };
                    var pFolderRead = new SqlParameter { ParameterName = "@FolderRead", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = batch.FolderRead };
                    var pFolderLog = new SqlParameter { ParameterName = "@FolderLog", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = batch.FolderLog };
                    var pFolderMoveToDone = new SqlParameter { ParameterName = "@FolderMoveToDone", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = batch.FolderMoveToDone };
                    var pFolderMoveToError = new SqlParameter { ParameterName = "@FolderMoveToError", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = batch.FolderMoveToError };
                    var pFrecuencyId = new SqlParameter { ParameterName = "@FrecuencyId", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input, Value = batch.Enabled };
                    var pPeriodMonth = new SqlParameter { ParameterName = "@PeriodMonth", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input, Value = 0 };
                    var pPeriodYear = new SqlParameter { ParameterName = "@PeriodYear", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input, Value = 0 };
                    var pEnabled = new SqlParameter { ParameterName = "@Enabled", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input, Value = 0 };

                    Command = new SqlCommand(Sql, Connection);

                    Command.Parameters.Add(pName);
                    Command.Parameters.Add(pSystemName);
                    Command.Parameters.Add(pPathBase);
                    Command.Parameters.Add(pFolderRead);
                    Command.Parameters.Add(pFolderLog);
                    Command.Parameters.Add(pFolderMoveToDone);
                    Command.Parameters.Add(pFolderMoveToError);
                    Command.Parameters.Add(pFrecuencyId);
                    Command.Parameters.Add(pPeriodMonth);
                    Command.Parameters.Add(pPeriodYear);
                    Command.Parameters.Add(pEnabled);

                    Command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "DaoBase.Install(" + batch.SystemName + ")", ex.Message);
            }
        }

        public void UnInstall(string systemName)
        {
            try
            {
                Log.InfoFormat("Action: {0}", "DaoBase.UnInstall(" + systemName + ")");

                if (!IsInstalled(systemName))
                {
                    Sql = "DELETE FROM ScheduleBatch WHERE SystemName=@SystemName";

                    var pSystemName = new SqlParameter
                    {
                        ParameterName = "@SystemName",
                        SqlDbType = SqlDbType.NVarChar,
                        Direction = ParameterDirection.Input,
                        Value = systemName
                    };

                    Command = new SqlCommand(Sql, Connection);
                    Command.Parameters.Add(pSystemName);
                    Command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "DaoBase.UnInstall(" + systemName + ")", ex.Message);
            }
        }

        public void UpdateScheduleBatch(ScheduleBatch batch)
        {
            try
            {
                Log.InfoFormat("Action: {0}", "DaoBase.UpdateScheduleBatch(" + batch.SystemName + ")");

                if (IsInstalled(batch.SystemName))
                {
                    Sql = "UPDATE ScheduleBatch SET " +
                          "NextExecutionOnUtc=@NextExecutionOnUtc , " +
                          "LastExecutionOnUtc=@LastExecutionOnUtc , " +
                          "PeriodYear=@PeriodYear , " +
                          "PeriodMonth=@PeriodMonth , " +
                          "Enabled=@Enabled , "+
                          "UpdateData=@UpdateData  " +
                          "where SystemName=@SystemName ";

                    var pNextExecutionOnUtc = new SqlParameter { ParameterName = "@NextExecutionOnUtc", SqlDbType = SqlDbType.DateTime2, Direction = ParameterDirection.Input, Value = batch.NextExecutionOnUtc };
                    var pLastExecutionOnUtc = new SqlParameter { ParameterName = "@LastExecutionOnUtc", SqlDbType = SqlDbType.DateTime2, Direction = ParameterDirection.Input, Value = batch.LastExecutionOnUtc };
                    var pPeriodYear = new SqlParameter { ParameterName = "@PeriodYear", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input, Value = batch.PeriodYear };
                    var pPeriodMonth = new SqlParameter { ParameterName = "@PeriodMonth", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input, Value = batch.PeriodMonth };
                    var pSystemName = new SqlParameter { ParameterName = "@SystemName", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = batch.SystemName };
                    var pEnabled = new SqlParameter { ParameterName = "@Enabled", SqlDbType = SqlDbType.Bit, Direction = ParameterDirection.Input, Value = batch.Enabled };
                    var pUpdateData = new SqlParameter { ParameterName = "@UpdateData", SqlDbType = SqlDbType.Bit, Direction = ParameterDirection.Input, Value = batch.UpdateData };

                    Command = new SqlCommand(Sql, Connection);

                    Command.Parameters.Add(pNextExecutionOnUtc);
                    Command.Parameters.Add(pLastExecutionOnUtc);
                    Command.Parameters.Add(pPeriodMonth);
                    Command.Parameters.Add(pPeriodYear);
                    Command.Parameters.Add(pSystemName);
                    Command.Parameters.Add(pEnabled);
                    Command.Parameters.Add(pUpdateData);

                    Command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "DaoBase.UpdateScheduleBatch(" + batch.SystemName + ")", ex.Message);
            }
        }

        public void DeleteReport(string period, string source)
        {
            try
            {
                Log.InfoFormat("Action: {0}", "DaoBase.DeleteReport()");

                Sql = " DELETE Report WHERE Period=@Period AND [Source]=@Source";

                Command = new SqlCommand(Sql, Connection);
                Command.Parameters.AddWithValue("@Period", period);
                Command.Parameters.AddWithValue("@Source", source);
                Command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "DaoBase.DeleteReport()", ex.Message);
            }
        }
        public Guid CreateReportIn(ScheduleBatch batch, string value)
        {
            var guid = Guid.NewGuid();
            try
            {
                Log.InfoFormat("Action: {0}", "DaoBase.CreateReportIn()");

                Sql = " INSERT INTO Report " +
                      " ([Key],Name,Value,PathBase,StateId,Period,Source, ParentKey,DateUtc)" +
                      " VALUES " +
                      " (@Key,@Name,@Value,@PathBase,@StateId,@Period,@Source,@ParentKey,@DateUtc)";


                Command = new SqlCommand(Sql, Connection);
                Command.Parameters.AddWithValue("@Key", guid);
                Command.Parameters.AddWithValue("@Name", string.Format("Archivos para la caja en el periodo - {0}", batch.PeriodYear.ToString("0000") + batch.PeriodMonth.ToString("00")));
                Command.Parameters.AddWithValue("@Value", value);
                Command.Parameters.AddWithValue("@PathBase", batch.PathBase);
                Command.Parameters.AddWithValue("@StateId", 2);
                Command.Parameters.AddWithValue("@Period", batch.PeriodYear.ToString("0000") + batch.PeriodMonth.ToString("00"));
                Command.Parameters.AddWithValue("@Source", batch.SystemName);
                Command.Parameters.AddWithValue("@ParentKey", guid);
                Command.Parameters.AddWithValue("@DateUtc", DateTime.UtcNow);

                Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "DaoBase.CreateReportIn()", ex.Message);
            }
            return guid;
        }
        public void CreateReportOut(Guid guid, string period, string source)
        {
            try
            {
                Log.InfoFormat("Action: {0}", "DaoBase.CreateReportOut(" + guid + ")");

                Sql = " INSERT INTO Report " +
                      " ([Key],Name,Value,PathBase,StateId,Period,Source, ParentKey,DateUtc)" +
                      " VALUES " +
                      " (@Key,@Name,@Value,@PathBase,@StateId,@Period,@Source,@ParentKey,@DateUtc)";

                Guid.NewGuid();
                Command = new SqlCommand(Sql, Connection);
                Command.Parameters.AddWithValue("@Key", Guid.NewGuid());
                Command.Parameters.AddWithValue("@Name", "");
                Command.Parameters.AddWithValue("@Value", "");
                Command.Parameters.AddWithValue("@PathBase", "");
                Command.Parameters.AddWithValue("@StateId", 1);
                Command.Parameters.AddWithValue("@Period", period);
                Command.Parameters.AddWithValue("@Source", source);
                Command.Parameters.AddWithValue("@ParentKey", guid);
                Command.Parameters.AddWithValue("@DateUtc", DateTime.UtcNow);

                Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "DaoBase.CreateReportOut(" + guid + ")", ex.Message);
            }
        }

        public ScheduleBatch GetScheduleBatch(string sysName)
        {
            if (IsConnected)
            {
                ScheduleBatch result = null;
                try
                {
                    Log.InfoFormat("Time: {0}: Action: {1}", DateTime.Now, "Start to Select");
                    Sql = "Select * from ScheduleBatch where SystemName='" + sysName + "'";
                    Command = new SqlCommand(Sql, Connection);
                    var sqlReader = Command.ExecuteReader();
                    while (sqlReader.Read())
                    {
                        result = new ScheduleBatch
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
                            result.StartExecutionOnUtc = sqlReader.GetDateTime(11);
                        if (!sqlReader.IsDBNull(12))
                            result.NextExecutionOnUtc = sqlReader.GetDateTime(12);
                        if (!sqlReader.IsDBNull(13))
                            result.LastExecutionOnUtc = sqlReader.GetDateTime(13);
                    }

                    sqlReader.Close();
                    return result;
                }
                catch (Exception ex)
                {
                    Log.FatalFormat("Time: {0} Error: {1}", DateTime.Now, ex.Message);
                    return null;
                }
            }
            return null;
        }

        #region Utilities

        private bool IsInstalled(string systemName)
        {
            var result = false;
            try
            {
                Log.InfoFormat("Action: {0}", "DaoBase.IsInstalled(" + systemName + ")");

                Sql = "SELECT SystemName FROM ScheduleBatch WHERE SystemName=@SystemName ";

                Command = new SqlCommand(Sql, Connection);
                var pSystemName = new SqlParameter
                {
                    ParameterName = "@SystemName",
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    Value = systemName
                };
                Command.Parameters.Add(pSystemName);
                var sqlReader = Command.ExecuteReader();

                while (sqlReader.Read())
                    result = true;

                sqlReader.Close();

            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "DaoBase.IsInstalled(" + systemName + ")", ex.Message);
            }
            return result;
        }

        private void Exec(string systemName, int option)
        {
            try
            {
                Sql = "UPDATE ScheduleBatch SET Enabled=" + option + " WHERE SystemName=@SystemName ";
                Log.InfoFormat("Action: {0}", "DaoBase.Exec(" + systemName + "," + option + ")");

                Command = new SqlCommand(Sql, Connection);
                var pSystemName = new SqlParameter
                {
                    ParameterName = "@SystemName",
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    Value = systemName
                };
                Command.Parameters.Add(pSystemName);
                Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "DaoBase.Exec(" + option + ")", ex.Message);
            }
        }



        #endregion
    }
}