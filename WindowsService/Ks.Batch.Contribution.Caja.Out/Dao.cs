using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Ks.Batch.Util;

namespace Ks.Batch.Contribution.Caja.Out
{
    public class Dao : DaoBase
    {
        public Dao(string connetionString)
            : base(connetionString)
        {
        }

        public List<string> Process(ScheduleBatch batch)
        {
            List<string> result;
            try
            {
                Log.InfoFormat("Time: {0}: Action: {1}", DateTime.Now, "Start to Select");

                List<int> customerIds;
                var temp = GetCustomer(out customerIds);
                result = GetContributionPayments(batch, temp, customerIds);

                Log.InfoFormat("Time: {0}: Action: {1}", DateTime.Now, result.Count + " records readed");
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Time: {0} Error: {1}", DateTime.Now, ex.Message);
                return null;
            }
            return result;
        }

        private List<string> GetContributionPayments(ScheduleBatch batch, Dictionary<int, string> customer, List<int> customerIds)
        {
            var result = new List<string>();
            Sql = "SELECT  c.CustomerId, cp.AmountTotal " +
                  "FROM ContributionPayment cp " +
                  "INNER JOIN  Contribution c on c.Id=cp.ContributionId " +
                  "WHERE c.CustomerId IN (@CustomerId) AND " +
                  "YEAR(cp.ScheduledDateOnUtc)=@Year AND " +
                  "MONTH(cp.ScheduledDateOnUtc)=@Month AND " +
                  "DAY(cp.ScheduledDateOnUtc)=@Day";

            Command = new SqlCommand(Sql, Connection);
            var pCustomerIds = new SqlParameter
            {
                ParameterName = "@CustomerId",
                SqlDbType = SqlDbType.NVarChar,
                Direction = ParameterDirection.Input,
                Value = string.Join(",", customerIds.ToArray())
            };
            var pYear = new SqlParameter
            {
                ParameterName = "@Year",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input,
                Value = batch.PeriodYear
            };
            var pMonth = new SqlParameter
            {
                ParameterName = "@Month",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input,
                Value = batch.PeriodMonth
            };
            var pDay = new SqlParameter
            {
                ParameterName = "@Day",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input,
                Value = batch.StartExecutionOnUtc.Value.Day
            };


            Command.Parameters.Add(pCustomerIds);
            Command.Parameters.Add(pYear);
            Command.Parameters.Add(pMonth);
            Command.Parameters.Add(pDay);

            var sqlReader = Command.ExecuteReader();

            while (sqlReader.Read())
            {
                var line = string.Format("{0}  {1}{2}{3}", customer[sqlReader.GetInt32(0)],
                                                            batch.PeriodYear,
                                                            batch.PeriodMonth.ToString("00"),
                                                            (sqlReader.GetDecimal(1).ToString("n")).PadLeft(10, '0'));
                result.Add(line);
            }
            sqlReader.Close();
            return result;
        }

        private Dictionary<int, string> GetCustomer(out List<int> customerIds)
        {
            customerIds = new List<int>();
            var temp = new Dictionary<int, string>();
            Sql = "SELECT EntityId, Attribute =[Key], Value FROM GenericAttribute " +
                  "WHERE KeyGroup='Customer' and  [Key] in ('Dni','AdmCode') AND " +
                  "EntityId IN ( SELECT EntityId FROM GenericAttribute WHERE [Key]='MilitarySituationId' AND Value=2)";
            Command = new SqlCommand(Sql, Connection);
            var sqlReader = Command.ExecuteReader();

            var count = 0;
            var admCode = "";
            var dni = "";

            var entityId = 0;
            var repeatEntityId = 0;

            while (sqlReader.Read())
            {
                if (sqlReader.GetString(1).Equals("AdmCode"))
                {
                    count++;
                    admCode = sqlReader.GetString(2);
                    entityId = sqlReader.GetInt32(0);
                }
                if (sqlReader.GetString(1).Equals("Dni"))
                {
                    count++;
                    dni = sqlReader.GetString(2);
                    repeatEntityId = sqlReader.GetInt32(0);
                }
                if (count == 2 && entityId == repeatEntityId)
                {
                    temp.Add(entityId, string.Format("0029600820{0}01LE{1}", admCode, dni));
                    customerIds.Add(entityId);
                    entityId = repeatEntityId = count = 0;
                }
            }
            sqlReader.Close();
            return temp;
        }
    }
}
