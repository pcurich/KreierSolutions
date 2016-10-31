using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Ks.Batch.Contribution.In
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

        public int Process(List<Info> info)
        {
            try
            {
                Sql = "insert into Reports (Name,Value,StateId,DateUtc) values (@Name,@Value,@StateId,@DateUtc)";

                Command = new SqlCommand(Sql, Connection);
                Command.Parameters.AddWithValue("@Name", "Contributions.Out");
                Command.Parameters.AddWithValue("@Value", XmlHelper.Serialize2String(info));
                Command.Parameters.AddWithValue("@StateId", 1);
                Command.Parameters.AddWithValue("@DateUtc", DateTime.UtcNow);

                return Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return -1;
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
