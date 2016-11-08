using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Ks.Batch.Util;
using Ks.Batch.Util.Model;

namespace Ks.Batch.Copere.Out
{
    public class Dao : DaoBase
    {
        private Dictionary<int, Info> ReportOut { get; set; }
        private ScheduleBatch Batch { get; set; }

        private const string AMOUNT = "AMOUNT";
        private const string NUMBER = "001";

        public Dao(string connetionString)
            : base(connetionString)
        {
        }

        public List<string> Process(ScheduleBatch batch)
        {
            List<string> result;
            Batch = batch;
            try
            {
                Log.InfoFormat("Time: {0}: Action: {1}", DateTime.Now, "Start to Select");

                List<int> customerIds;
                ReportOut = new Dictionary<int, Info>();
                var temp = GetCustomer(out customerIds);
                result = GetContributionPayments(temp, customerIds);

                if (result != null)
                {
                    DeleteReport();
                    CompleteCustomerName(temp);
                    var guid = CreateReportIn();
                    CreateReportOut(guid);
                    Log.InfoFormat("Time: {0}: Action: {1}", DateTime.Now, result.Count + " records readed");
                }

            }
            catch (Exception ex)
            {
                Log.FatalFormat("Time: {0} Error: {1}", DateTime.Now, ex.Message);
                return null;
            }
            return result;
        }

        #region Util

        private List<string> GetContributionPayments(Dictionary<int, string> customer, List<int> customerIds)
        {
            var result = new List<string>();
            Sql = "SELECT  c.CustomerId, cp.AmountTotal " +
                  "FROM ContributionPayment cp " +
                  "INNER JOIN  Contribution c on c.Id=cp.ContributionId " +
                  "WHERE c.CustomerId IN (@CustomerId) AND " +
                  "YEAR(cp.ScheduledDateOnUtc)=@Year AND " +
                  "MONTH(cp.ScheduledDateOnUtc)=@Month  ";

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
                Value = Batch.PeriodYear
            };
            var pMonth = new SqlParameter
            {
                ParameterName = "@Month",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input,
                Value = Batch.PeriodMonth
            };

            Command.Parameters.Add(pCustomerIds);
            Command.Parameters.Add(pYear);
            Command.Parameters.Add(pMonth);

            var sqlReader = Command.ExecuteReader();
            var hasValue = false;
            while (sqlReader.Read())
            {
                hasValue = true;
                var line = string.Format("{0}        {1}{2}",
                    customer[sqlReader.GetInt32(0)].Replace(AMOUNT, (Math.Round(sqlReader.GetDecimal(1) * 100).ToString() .PadLeft(13, '0'))), Batch.PeriodYear, Batch.PeriodMonth.ToString("00"));
                Info info;
                ReportOut.TryGetValue(sqlReader.GetInt32(0), out info);
                if (info != null)
                {
                    info.Year = Batch.PeriodYear;
                    info.Month = Batch.PeriodMonth;
                    info.Total = sqlReader.GetDecimal(1);
                }
                ReportOut.Remove(sqlReader.GetInt32(0));
                ReportOut.Add(sqlReader.GetInt32(0), info);
                result.Add(line);
            }
            sqlReader.Close();

            UpdateData(customerIds);

            return hasValue ? result : null;
        }

        private Dictionary<int, string> GetCustomer(out List<int> customerIds)
        {
            customerIds = new List<int>();
            var temp = new Dictionary<int, string>();
            Sql = "SELECT EntityId, Attribute =[Key], Value FROM GenericAttribute " +
                  "WHERE KeyGroup='Customer' and  [Key] in ('Dni','AdmCode') AND " +
                  "EntityId IN ( SELECT EntityId FROM GenericAttribute WHERE [Key]='MilitarySituationId' AND Value=1)";
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
                    temp.Add(entityId, string.Format("8A{0}8001{1}0000000000000{2}", admCode, AMOUNT, NUMBER));
                    customerIds.Add(entityId);
                    ReportOut.Add(entityId, new Info { CustomerId = entityId, AdminCode = admCode, HasAdminCode = true, Dni = dni, HasDni = true });
                    entityId = repeatEntityId = count = 0;
                }
            }
            sqlReader.Close();
            return temp;
        }

        private void UpdateData(List<int> customerIds)
        {
            Sql = "UPDATE ContributionPayment SET StateId =2 WHERE ID IN ( " +
                  " SELECT  cp.Id " +
                  " FROM ContributionPayment cp " +
                  " INNER JOIN  Contribution c on c.Id=cp.ContributionId " +
                  " WHERE c.CustomerId IN (@CustomerId) AND  " +
                  " YEAR(cp.ScheduledDateOnUtc)=@Year AND  " +
                  " MONTH(cp.ScheduledDateOnUtc)=@Month " +
                  " ) ";

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
                Value = Batch.PeriodYear
            };
            var pMonth = new SqlParameter
            {
                ParameterName = "@Month",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input,
                Value = Batch.PeriodMonth
            };

            Command.Parameters.Add(pCustomerIds);
            Command.Parameters.Add(pYear);
            Command.Parameters.Add(pMonth);

            Command.ExecuteNonQuery();


        }

        private void CompleteCustomerName(Dictionary<int, string> temp)
        {
            var result=GetUserNames(temp.Keys.ToList());

            foreach (var pair in result)
            {
                Info info;
                ReportOut.TryGetValue(pair.Key, out info);
                if (info == null) 
                    continue;
                
                info.CompleteName = pair.Value;
                ReportOut.Remove(pair.Key);
                ReportOut.Add(pair.Key,info);
            }
        }

        private void DeleteReport()
        {
            try
            {
                Sql = " DELETE Reports WHERE Period=@Period ";

                Command = new SqlCommand(Sql, Connection);
                Command.Parameters.AddWithValue("@Period", Batch.PeriodYear.ToString("0000") + Batch.PeriodMonth.ToString("00"));
                Command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Log.FatalFormat("Time: {0} Error: {1}", DateTime.Now, ex.Message);
            }
        }
        private Guid CreateReportIn()
        {
            var guid = Guid.NewGuid();
            try
            {
                Sql = " INSERT INTO Reports " +
                      " ([Key],Name,Value,PathBase,StateId,Period,Source, ParentKey,DateUtc)" +
                      " VALUES " +
                      " (@Key,@Name,@Value,@PathBase,@StateId,@Period,@Source,@ParentKey,@DateUtc)";


                Command = new SqlCommand(Sql, Connection);
                Command.Parameters.AddWithValue("@Key", guid);
                Command.Parameters.AddWithValue("@Name", string.Format("Archivos para la caja en el periodo - {0}", Batch.PeriodYear.ToString("0000") + Batch.PeriodMonth.ToString("00")));
                Command.Parameters.AddWithValue("@Value", XmlHelper.Serialize2String(new List<Info>(ReportOut.Values)));
                Command.Parameters.AddWithValue("@PathBase", Batch.PathBase);
                Command.Parameters.AddWithValue("@StateId", 1);
                Command.Parameters.AddWithValue("@Period", Batch.PeriodYear.ToString("0000") + Batch.PeriodMonth.ToString("00"));
                Command.Parameters.AddWithValue("@Source", Batch.SystemName);
                Command.Parameters.AddWithValue("@ParentKey", guid);
                Command.Parameters.AddWithValue("@DateUtc", DateTime.UtcNow);

                Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Time: {0} Error: {1}", DateTime.Now, ex.Message);
            }
            return guid;
        }
        private void CreateReportOut(Guid guid)
        {
            try
            {
                Sql = " INSERT INTO Reports " +
                      " ([Key],Name,Value,PathBase,StateId,Period,Source, ParentKey,DateUtc)" +
                      " VALUES " +
                      " (@Key,@Name,@Value,@PathBase,@StateId,@Period,@Source,@ParentKey,@DateUtc)";

                Guid.NewGuid();
                Command = new SqlCommand(Sql, Connection);
                Command.Parameters.AddWithValue("@Key", Guid.NewGuid());
                Command.Parameters.AddWithValue("@Name", string.Format("Archivos para la caja en el periodo - {0}", Batch.PeriodYear.ToString("0000") + Batch.PeriodMonth.ToString("00")));
                Command.Parameters.AddWithValue("@Value", XmlHelper.Serialize2String(new List<Info>(ReportOut.Values)));
                Command.Parameters.AddWithValue("@PathBase", Batch.PathBase);
                Command.Parameters.AddWithValue("@StateId", 1);
                Command.Parameters.AddWithValue("@Period", Batch.PeriodYear.ToString("0000") + Batch.PeriodMonth.ToString("00"));
                Command.Parameters.AddWithValue("@Source", "Ks.Batch.Copere.In");
                Command.Parameters.AddWithValue("@ParentKey", guid);
                Command.Parameters.AddWithValue("@DateUtc", DateTime.UtcNow);

                Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Time: {0} Error: {1}", DateTime.Now, ex.Message);
            }
        }

        #endregion
    }
}
