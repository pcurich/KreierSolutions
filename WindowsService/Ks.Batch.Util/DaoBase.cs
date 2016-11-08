using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
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
            try
            {
                Log.InfoFormat("Time: {0}: Action: {1} ", DateTime.Now, "Connected");
                Connection = new SqlConnection(ConnetionString);
                Connection.Open();
                Log.InfoFormat("Time: {0}: Action: {1} ", DateTime.Now, "Open");
                IsConnected = true;

            }
            catch (Exception ex)
            {
                IsConnected = false;
                Log.FatalFormat("Time: {0} Error: {1}", DateTime.Now, ex.Message);
            }

        }

        public void Close()
        {
            IsConnected = false;
            try
            {
                Log.InfoFormat("Time: {0}: Action: {1} ", DateTime.Now, "Data Base to be close");
                Command.Dispose();
                Connection.Close();
                Log.InfoFormat("Time: {0}: Action: {1} ", DateTime.Now, "Data Base closed");
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Time: {0} Error: {1}", DateTime.Now, ex.Message);
            }
        }

        public void Enabled(string systemName)
        {
            Exec(systemName, 1);
        }

        public void Disabled(string systemName)
        {
            Exec(systemName, 0);
        }

        public Dictionary<int, string> GetUserNames(List<int> customerIds )
        {
            var result = new Dictionary<int, string>();

            Sql = "SELECT EntityId, Attribute =[Key], Value FROM GenericAttribute WHERE " +
                  " [Key] in ('FirstName','LastName') AND " +
                  " KeyGroup='Customer' AND EntityId IN (@CustomerId) " +
                  " ORDER BY EntityId ";

            Command = new SqlCommand(Sql, Connection);
            var pCustomerIds = new SqlParameter
            {
                ParameterName = "@CustomerId",
                SqlDbType = SqlDbType.NVarChar,
                Direction = ParameterDirection.Input,
                Value = string.Join(",", customerIds.ToArray())
            };

            Command.Parameters.Add(pCustomerIds);

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
            return result;
        } 

        public void Install(ScheduleBatch batch)
        {
            try
            {
                if (!IsInstalled(batch.SystemName))
                {
                    Connect();
                    Sql = "INSERT INTO ScheduleBatch " +
                          "(Name, SystemName, PathBase,FolderRead,FolderLog,FolderMoveToDone,FolderMoveToError, FrecuencyId, PeriodYear, PeriodMonth,Enabled) " +
                          "VALUES " +
                          "(@Name, @SystemName, @PathBase,@FolderRead,@FolderLog,@FolderMoveToDone,@FolderMoveToError, @FrecuencyId, @PeriodYear, @PeriodMonth, @Enabled) ";

                    Log.InfoFormat("Time: {0}: SQL: {1} ", DateTime.Now, Sql);

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
                    Log.InfoFormat("Time: {0}: RESULT: {1} ", DateTime.Now, "Service Installed");
                    Close();
                }
                else
                {
                    Log.InfoFormat("Time: {0}: RESULT: {1} ", DateTime.Now, "Service Was Installed");
                }
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Time: {0}: ERROR: {1} ", DateTime.Now, ex.Message);
                Close();
            }

        }

        public void UnInstall(string systemName)
        {
            try
            {
                if (!IsInstalled(systemName))
                {
                    Connect();
                    Sql = "DELETE FROM ScheduleBatch " +
                          "WHERE SystemName=@SystemName";

                    Log.InfoFormat("Time: {0}: SQL: {1} ", DateTime.Now, Sql);

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

                    Log.InfoFormat("Time: {0}: RESULT: {1} ", DateTime.Now, "Service Installed");
                }
                else
                {
                    Log.InfoFormat("Time: {0}: RESULT: {1} ", DateTime.Now, "Service Was Installed");
                }
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Time: {0}: ERROR: {1} ", DateTime.Now, ex.Message);
            }
            Close();
        }

        public void UpdateScheduleBatch(ScheduleBatch batch)
        {
            try
            {
                if (IsInstalled(batch.SystemName))
                {
                    Connect();
                    Sql = "UPDATE ScheduleBatch SET " +
                          "NextExecutionOnUtc=@NextExecutionOnUtc , " +
                          "LastExecutionOnUtc=@LastExecutionOnUtc , " +
                          "PeriodYear=@PeriodYear , " +
                          "PeriodMonth=@PeriodMonth " +
                          "where SystemName=@SystemName ";

                    Log.InfoFormat("Time: {0}: SQL: {1} ", DateTime.Now, Sql);

                    var pNextExecutionOnUtc = new SqlParameter { ParameterName = "@NextExecutionOnUtc", SqlDbType = SqlDbType.DateTime2, Direction = ParameterDirection.Input, Value = batch.NextExecutionOnUtc };
                    var pLastExecutionOnUtc = new SqlParameter { ParameterName = "@LastExecutionOnUtc", SqlDbType = SqlDbType.DateTime2, Direction = ParameterDirection.Input, Value = batch.LastExecutionOnUtc };
                    var pPeriodYear = new SqlParameter { ParameterName = "@PeriodYear", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input, Value = batch.PeriodYear };
                    var pPeriodMonth = new SqlParameter { ParameterName = "@PeriodMonth", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Input, Value = batch.PeriodMonth };
                    var pSystemName = new SqlParameter { ParameterName = "@SystemName", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Input, Value = batch.SystemName };


                    Command = new SqlCommand(Sql, Connection);

                    Command.Parameters.Add(pNextExecutionOnUtc);
                    Command.Parameters.Add(pLastExecutionOnUtc);
                    Command.Parameters.Add(pPeriodMonth);
                    Command.Parameters.Add(pPeriodYear);
                    Command.Parameters.Add(pSystemName);

                    Command.ExecuteNonQuery();

                    Log.InfoFormat("Time: {0}: RESULT: {1} ", DateTime.Now, "Service Updated");
                    Close();
                }
                else
                {
                    Log.InfoFormat("Time: {0}: RESULT: {1} ", DateTime.Now, "Service Was not Installed");
                }
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Time: {0}: ERROR: {1} ", DateTime.Now, ex.Message);
                Close();
            }

        }

        #region Utilities
        private bool IsInstalled(string systemName)
        {
            var result = false;
            Connect();
            try
            {
                Sql = "SELECT SystemName FROM ScheduleBatch WHERE SystemName=@SystemName ";
                Log.InfoFormat("Time: {0}: SQL: {1} ", DateTime.Now, Sql);

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

                Log.InfoFormat("Time: {0}: RESULT: {1} ", DateTime.Now, result ? "Service Installed" : "Service Not Installed");
                sqlReader.Close();

            }
            catch (Exception ex)
            {
                Log.FatalFormat("Time: {0}: ERROR: {1} ", DateTime.Now, ex.Message);

            }
            Close();
            return result;
        }

        private void Exec(string systemName, int option)
        {
            Connect();
            try
            {
                Sql = "UPDATE ScheduleBatch SET Enabled=" + option + " WHERE SystemName=@SystemName ";
                Log.InfoFormat("Time: {0}: SQL: {1} ", DateTime.Now, Sql);

                Command = new SqlCommand(Sql, Connection);
                var pSystemName = new SqlParameter
                {
                    ParameterName = "@SystemName",
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    Value = systemName
                };
                Command.Parameters.Add(pSystemName);
                var i = Command.ExecuteNonQuery();
                Log.InfoFormat("Time: {0}: RESULT: {1} ", DateTime.Now, i);
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Time: {0}: ERROR: {1} ", DateTime.Now, ex.Message);
            }
            Close();
        }
        #endregion
    }
}