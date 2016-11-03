using System;
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

        public void Install(ScheduleBatch batch)
        {
            Connect();
            try
            {
                if (!IsInstalled(batch.SystemName))
                {
                    Sql = "INSERT INTO ScheduleBatch " +
                          "(Name, SystemName, PathBase, FrecuencyId, PeriodYear, PeriodMonth,Enabled) " +
                          "VALUES " +
                          "(@Name, @SystemName, @PathBase, @FrecuencyId, @PeriodYear, @PeriodMonth, @Enabled) ";

                    Log.InfoFormat("Time: {0}: SQL: {1} ", DateTime.Now, Sql);

                    var pName = new SqlParameter
                    {
                        ParameterName = "@Name",
                        SqlDbType = SqlDbType.NVarChar,
                        Direction = ParameterDirection.Input,
                        Value = batch.Name
                    };
                    var pSystemName = new SqlParameter
                    {
                        ParameterName = "@SystemName",
                        SqlDbType = SqlDbType.NVarChar,
                        Direction = ParameterDirection.Input,
                        Value = batch.SystemName
                    };
                    var pPathBase = new SqlParameter
                    {
                        ParameterName = "@PathBase",
                        SqlDbType = SqlDbType.NVarChar,
                        Direction = ParameterDirection.Input,
                        Value = batch.PathBase
                    };
                    var pFrecuencyId = new SqlParameter
                    {
                        ParameterName = "@FrecuencyId",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        Value = 0
                    };
                    var pPeriodMonth = new SqlParameter
                    {
                        ParameterName = "@PeriodMonth",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        Value = 0
                    };
                    var pPeriodYear = new SqlParameter
                    {
                        ParameterName = "@PeriodYear",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        Value = 0
                    };
                    var pEnabled= new SqlParameter
                    {
                        ParameterName = "@Enabled=",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,
                        Value = 0
                    };

                    Command.Parameters.Add(pName);
                    Command.Parameters.Add(pSystemName);
                    Command.Parameters.Add(pPathBase);
 
                    Command.Parameters.Add(pFrecuencyId);
                    Command.Parameters.Add(pPeriodMonth);
                    Command.Parameters.Add(pPeriodYear);
                    Command.Parameters.Add(pEnabled);

                    Command = new SqlCommand(Sql, Connection);
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

        private bool IsInstalled(string systemName)
        {
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
                var result = false;

                while (sqlReader.Read())
                    result = true;

                Log.InfoFormat("Time: {0}: RESULT: {1} ", DateTime.Now, result ? "Service Installed" : "Service Not Installed");
                sqlReader.Close();
                return result;

            }
            catch (Exception ex)
            {
                Log.FatalFormat("Time: {0}: ERROR: {1} ", DateTime.Now, ex.Message);
            }
            Close();
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
    }
}