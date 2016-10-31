using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Ks.Batch.Sync
{
    public class DaoService
    {
        public string ConnetionString = @"Data Source=IDEA-PC\MSSQLSERVER2014;Initial Catalog=ACMR;Integrated Security=True;Persist Security Info=False";
        public SqlConnection Connection;
        public SqlCommand Command;
        public string Sql;
        public SqlDataReader DataReader;


        public void Connect()
        {
            Connection = new SqlConnection(ConnetionString);
            Connection.Open();
        }

        public List<ScheduleBatch> Process()
        {
            var result = new List<ScheduleBatch>();
            try
            {
                Sql = "Select * from ScheduleBatch";
                Command = new SqlCommand(Sql, Connection);
                var sqlReader = Command.ExecuteReader();
                while (sqlReader.Read())
                {
                    result.Add(new ScheduleBatch
                    {
                        Id = sqlReader.GetInt32(0),
                        Name = sqlReader.GetString(1),
                        SystemName = sqlReader.GetString(2),
                        PathRead = sqlReader.GetString(3),
                        PathLog = sqlReader.GetString(4),
                        PathMoveToDone = sqlReader.GetString(5),
                        PathMoveToError = sqlReader.GetString(6),
                        FrecuencyId = sqlReader.GetInt32(7),
                        StartExecutionOnUtc = sqlReader.GetDateTime(8),
                        NextExecutionOnUtc = sqlReader.GetDateTime(9),
                        LastExecutionOnUtc = sqlReader.GetDateTime(10),
                        Enabled = sqlReader.GetBoolean(11)
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void Close()
        {
            DataReader.Close();
            Command.Dispose();
            Connection.Close();
        }
    }
}