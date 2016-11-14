using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Ks.Batch.Util;
using Ks.Batch.Util.Model;

namespace Ks.Batch.Caja.Out
{
    public class Dao : DaoBase
    {
        private Dictionary<int, Info> ReportOut { get; set; }
        private Dictionary<int, string> FileOut { get; set; }

        private ScheduleBatch Batch { get; set; }

        public Dao(string connetionString)
            : base(connetionString)
        {
        }

        public List<string> Process(ScheduleBatch batch)
        {
            Batch = batch;
            try
            {
                Log.InfoFormat("Action: {0}", "Dao.Process(" + batch.SystemName + ")");

                List<int> customerIds;
                ReportOut = new Dictionary<int, Info>();

                GetCustomer(out customerIds);
                GetContributionPayments(FileOut, customerIds);

                if (FileOut.Count != 0)
                {
                    DeleteReport(Batch.PeriodYear.ToString("0000") + Batch.PeriodMonth.ToString("00"), Batch.SystemName);
                    CompleteCustomerName();
                    var guid = CreateReportIn(Batch, XmlHelper.Serialize2String(new List<Info>(ReportOut.Values)));
                    CreateReportOut(guid, Batch.PeriodYear.ToString("0000") + Batch.PeriodMonth.ToString("00"), "Ks.Batch.Caja.In");
                }
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "Dao.Process(" + batch.SystemName + ")", ex.Message);
            }

            return FileOut != null ? FileOut.Values.ToList() : null;
        }

        #region Util

        private void GetContributionPayments(Dictionary<int, string> customer, List<int> customerIds)
        {
            try
            {
                Log.InfoFormat("Action: {0}", "Dao.GetContributionPayments(" + string.Join(",", customerIds.ToArray()) + ")");

                Sql = "SELECT  c.CustomerId, cp.AmountTotal, cp.Amount1, cp.Amount2, cp.Amount3, cp.Number " +
                  "FROM ContributionPayment cp " +
                  "INNER JOIN  Contribution c on c.Id=cp.ContributionId " +
                  "WHERE c.CustomerId IN (" + string.Join(",", customerIds.ToArray()) + ") AND " +
                  "cp.StateId=@StateId AND " +
                  "YEAR(cp.ScheduledDateOnUtc)=@Year AND " +
                  "MONTH(cp.ScheduledDateOnUtc)=@Month  ";

                Command = new SqlCommand(Sql, Connection);

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
                var pStateId = new SqlParameter
                {
                    ParameterName = "@StateId",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input,
                    Value = ContributionState.Pendiente
                };
                Command.Parameters.Add(pYear);
                Command.Parameters.Add(pMonth);
                Command.Parameters.Add(pStateId);

                var sqlReader = Command.ExecuteReader();

                var reportOut2 = new Dictionary<int, Info>();
                var fileOut2 = new Dictionary<int, string>();
                var customerIds2 = new List<int>();

                while (sqlReader.Read())
                {
                    var line = string.Format("{0}  {1}{2}{3}", customer[sqlReader.GetInt32(0)],
                                                                Batch.PeriodYear,
                                                                Batch.PeriodMonth.ToString("00"),
                                                                (sqlReader.GetDecimal(1).ToString("n")).PadLeft(10, '0'));
                    Info info;
                    ReportOut.TryGetValue(sqlReader.GetInt32(0), out info);
                    if (info != null)
                    {
                        info.Year = Batch.PeriodYear;
                        info.Month = Batch.PeriodMonth;
                        info.AmountTotal = sqlReader.GetDecimal(1);
                        info.Amount1 = sqlReader.GetDecimal(2);
                        info.Amount2 = sqlReader.GetDecimal(3);
                        info.Amount3 = sqlReader.GetDecimal(4);
                        info.Number = sqlReader.GetInt32(5);
                        info.StateId = (int)ContributionState.Pendiente;
                    }
                    ReportOut.Remove(sqlReader.GetInt32(0));
                    reportOut2.Add(sqlReader.GetInt32(0), info);
                    fileOut2.Add(sqlReader.GetInt32(0), line);
                }

                sqlReader.Close();
                ReportOut.Clear();
                FileOut.Clear();

                foreach (var pk in reportOut2)
                {
                    customerIds2.Add(pk.Key);
                    ReportOut.Add(pk.Key, pk.Value);
                }

                foreach (var pk in fileOut2)
                {
                    FileOut.Add(pk.Key, pk.Value);
                }

                if (customerIds2.Count > 0)
                    UpdateData(customerIds2);
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "Dao.GetContributionPayments(" + string.Join(",", customerIds.ToArray()) + ")", ex.Message);
            }
        }

        private void GetCustomer(out List<int> customerIds)
        {
            customerIds = new List<int>();
            try
            {
                Log.InfoFormat("Action: {0}", "Dao.GetCustomer(" + string.Join(",", customerIds.ToArray()) + ")");

                Sql = "SELECT EntityId, Attribute =[Key], Value FROM GenericAttribute " +
                  " WHERE KeyGroup='Customer' and  [Key] in ('Dni','AdmCode') AND " +
                  " EntityId IN ( SELECT EntityId FROM GenericAttribute WHERE [Key]='MilitarySituationId' AND Value=2)";
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
                        if (ReportOut == null)
                            ReportOut = new Dictionary<int, Info>();

                        if (FileOut == null)
                            FileOut = new Dictionary<int, string>();

                        FileOut.Add(entityId, string.Format("0029600820{0}01LE{1}", admCode, dni));
                        customerIds.Add(entityId);
                        ReportOut.Add(entityId, new Info { CustomerId = entityId, AdminCode = admCode, HasAdminCode = true, Dni = dni, HasDni = true });
                        entityId = repeatEntityId = count = 0;
                    }
                }
                sqlReader.Close();
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "Dao.GetCustomer(" + string.Join(",", customerIds.ToArray()) + ")", ex.Message);
            }
        }

        private void UpdateData(List<int> customerIds)
        {
            try
            {
                Log.InfoFormat("Action: {0}", "Dao.UpdateData(" + string.Join(",", customerIds.ToArray()) + ")");

                Sql = "UPDATE ContributionPayment SET StateId =2 WHERE ID IN ( " +
                  " SELECT  cp.Id " +
                  " FROM ContributionPayment cp " +
                  " INNER JOIN  Contribution c on c.Id=cp.ContributionId " +
                  " WHERE c.CustomerId IN (" + string.Join(",", customerIds.ToArray()) + ") AND  " +
                  " YEAR(cp.ScheduledDateOnUtc)=@Year AND  " +
                  " MONTH(cp.ScheduledDateOnUtc)=@Month " +
                  " ) ";

                Command = new SqlCommand(Sql, Connection);

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

                Command.Parameters.Add(pYear);
                Command.Parameters.Add(pMonth);
                Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "Dao.UpdateData(" + string.Join(",", customerIds.ToArray()) + ")", ex.Message);
            }


        }

        private void CompleteCustomerName()
        {
            var result = GetUserNames(FileOut.Keys.ToList());

            foreach (var pair in result)
            {
                Info info;
                ReportOut.TryGetValue(pair.Key, out info);
                if (info == null)
                    continue;

                info.CompleteName = pair.Value;
                ReportOut.Remove(pair.Key);
                ReportOut.Add(pair.Key, info);
            }
        }

        #endregion
    }
}
