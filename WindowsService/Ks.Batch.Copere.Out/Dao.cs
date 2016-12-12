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
        private Dictionary<int, string> FileOut { get; set; }

        private List<string> Result { get; set; }

        private ScheduleBatch Batch { get; set; }

        private const string AMOUNT = "AMOUNT";
        private const string NUMBER = "001";

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
                GetContributionPayments(customerIds);
                GetLoanPayment(customerIds);
                //copere send contribution and loan in separates
                MergeData(customerIds);

                if (FileOut.Count != 0)
                {
                    DeleteReport(Batch.PeriodYear.ToString("0000") + Batch.PeriodMonth.ToString("00"), Batch.SystemName);
                    CompleteCustomerName();
                    var guid = CreateReportIn(Batch, XmlHelper.Serialize2String(new List<Info>(ReportOut.Values)));
                    CreateReportOut(guid, Batch.PeriodYear.ToString("0000") + Batch.PeriodMonth.ToString("00"), "Ks.Batch.Copere.In");
                }

            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "Dao.Process(" + batch.SystemName + ")", ex.Message);
            }

            return Result != null ? Result.ToList() : null;
        }

        #region Util

        private void GetContributionPayments(List<int> customerIds)
        {
            try
            {
                Log.InfoFormat("Action: {0}", "Dao.GetContributionPayments(" + string.Join(",", customerIds.ToArray()) + ")");

                Sql = "SELECT " +
                      "c.CustomerId as CustomerId,  " +
                      "cp.Id as ContributionPaymentId, " +
                      "cp.Number as Number, " +
                      "cp.NumberOld as NumberOld,  " +
                      "c.Id as ContributionId,  " +
                      "cp.Amount1 as Amount1, " +
                      "cp.Amount2 as Amount2, " +
                      "cp.Amount3 as Amount3, " +
                      "cp.AmountOld as AmountOld, " +
                      "cp.AmountTotal as AmountTotal, " +
                      "cp.AmountPayed as AmountPayed, " +
                      "cp.StateId as StateId, " +
                      "cp.IsAutomatic as IsAutomatic, " +
                      "isnull(cp.BankName,'') as BankName, " +
                      "isnull(cp.AccountNumber,'') as AccountNumber, " +
                      "isnull(cp.TransactionNumber,'') as TransactionNumber, " +
                      "isnull(cp.Reference,'') as Reference, " +
                      "isnull(cp.Description,'') as Description " +
                      "FROM ContributionPayment cp " +
                      "INNER JOIN  Contribution c on c.Id=cp.ContributionId " +
                      "WHERE c.CustomerId IN (" + string.Join(",", customerIds.ToArray()) + ") AND " +
                      "cp.StateId=@StateId AND " +
                      "c.active=1 AND " +
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
                    Value = (int)ContributionState.Pendiente
                };
                Command.Parameters.Add(pYear);
                Command.Parameters.Add(pMonth);
                Command.Parameters.Add(pStateId);

                var sqlReader = Command.ExecuteReader();

                var reportOut2 = new Dictionary<int, Info>();
                var customerIds2 = new List<int>();

                while (sqlReader.Read())
                {

                    Info info;
                    ReportOut.TryGetValue(sqlReader.GetInt32(sqlReader.GetOrdinal("CustomerId")), out info);

                    if (info != null)
                    {
                        info.Year = Batch.PeriodYear;
                        info.Month = Batch.PeriodMonth;
                        info.CustomerId = sqlReader.GetInt32(0);
                        info.InfoContribution = new InfoContribution
                        {
                            ContributionPaymentId = sqlReader.GetInt32(sqlReader.GetOrdinal("ContributionPaymentId")),
                            Number = sqlReader.GetInt32(sqlReader.GetOrdinal("Number")),
                            NumberOld = sqlReader.GetInt32(sqlReader.GetOrdinal("NumberOld")),
                            ContributionId = sqlReader.GetInt32(sqlReader.GetOrdinal("ContributionId")),
                            Amount1 = sqlReader.GetDecimal(sqlReader.GetOrdinal("Amount1")),
                            Amount2 = sqlReader.GetDecimal(sqlReader.GetOrdinal("Amount2")),
                            Amount3 = sqlReader.GetDecimal(sqlReader.GetOrdinal("Amount3")),
                            AmountOld = sqlReader.GetDecimal(sqlReader.GetOrdinal("AmountOld")),
                            AmountTotal = sqlReader.GetDecimal(sqlReader.GetOrdinal("AmountTotal")),
                            AmountPayed = sqlReader.GetDecimal(sqlReader.GetOrdinal("AmountPayed")),
                            StateId = (int)ContributionState.Pendiente,
                            IsAutomatic = sqlReader.GetBoolean(sqlReader.GetOrdinal("IsAutomatic")),
                            BankName = sqlReader.GetString(sqlReader.GetOrdinal("BankName")),
                            AccountNumber = sqlReader.GetString(sqlReader.GetOrdinal("AccountNumber")),
                            TransactionNumber = sqlReader.GetString(sqlReader.GetOrdinal("TransactionNumber")),
                            Reference = sqlReader.GetString(sqlReader.GetOrdinal("Reference")),
                            Description = sqlReader.GetString(sqlReader.GetOrdinal("Description")),
                        };

                        info.TotalContribution = sqlReader.GetDecimal(sqlReader.GetOrdinal("AmountTotal"));
                    }
                    ReportOut.Remove(sqlReader.GetInt32(0));
                    reportOut2.Add(sqlReader.GetInt32(0), info);
                }
                sqlReader.Close();
                ReportOut.Clear();

                foreach (var pk in reportOut2)
                {
                    customerIds2.Add(pk.Key);
                    ReportOut.Add(pk.Key, pk.Value);
                }

                var fileOutTem = new Dictionary<int, string>();
                foreach (var customerId in customerIds2)
                {
                    string data;
                    FileOut.TryGetValue(customerId, out data);
                    if (data != null)
                        fileOutTem.Add(customerId, data);
                }

                FileOut.Clear();

                foreach (var customerId in customerIds2)
                    FileOut.Add(customerId, fileOutTem[customerId]);


                if (customerIds2.Count > 0)
                    UpdateDataContribution(customerIds2);


            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "Dao.GetContributionPayments(" + string.Join(",", customerIds.ToArray()) + ")", ex.Message);
            }
        }

        private void GetLoanPayment(List<int> customerIds)
        {

            try
            {
                Log.InfoFormat("Action: {0}", "Dao.GetLoanPayments(" + string.Join(",", customerIds.ToArray()) + ")");

                Sql = " SELECT " +
                      " L.CustomerId as CustomerId, " +
                      " LP.Id as LoanPaymentId, " +
                      " L.Id as LoanId, " +
                      " LP.Quota as Quota, " +
                      " LP.MonthlyQuota as MonthlyQuota, " +
                      " LP.MonthlyFee as MonthlyFee, " +
                      " LP.MonthlyCapital as MonthlyCapital, " +
                      " LP.MonthlyPayed AS MonthlyPayed, " +
                      " LP.StateId as StateId, " +
                      " LP.IsAutomatic as IsAutomatic, " +
                      " ISNULL(LP.BankName,'') as BankName, " +
                      " ISNULL(LP.AccountNumber,'') as AccountNumber, " +
                      " ISNULL(LP.TransactionNumber,'') as TransactionNumber, " +
                      " ISNULL(LP.Reference,'') as Reference, " +
                      " ISNULL(LP.Description,'') as Description " +
                      " FROM LoanPayment LP INNER JOIN Loan L on L.Id=lp.LoanId " +
                      " WHERE " +
                      " L.CustomerId IN (" + string.Join(",", customerIds.ToArray()) + ") and " +
                      " L.Active=1 AND" + //Just Active 
                      " LP.StateId=@StateId and " +
                      " YEAR(lp.ScheduledDateOnUtc)=@Year and " +
                      " MONTH(lp.ScheduledDateOnUtc)=@Month " +
                      " ORDER BY 1 ";

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
                    Value = (int)ContributionState.Pendiente
                };
                Command.Parameters.Add(pYear);
                Command.Parameters.Add(pMonth);
                Command.Parameters.Add(pStateId);

                var sqlReader = Command.ExecuteReader();

                Info info = null;

                while (sqlReader.Read())
                {
                    if (info == null)
                    {
                        ReportOut.TryGetValue(sqlReader.GetInt32(sqlReader.GetOrdinal("CustomerId")), out info);
                        if (info != null) info.InfoLoans = new List<InfoLoan>();
                    }

                    if (info != null && info.CustomerId == sqlReader.GetInt32(sqlReader.GetOrdinal("CustomerId")))
                    {
                        info.InfoLoans.Add(AddNewLoan(sqlReader));
                        info.TotalLoan += sqlReader.GetDecimal(sqlReader.GetOrdinal("MonthlyQuota"));

                        if (ReportOut.ContainsKey(info.CustomerId))
                            ReportOut[info.CustomerId] = info;
                    }
                    else
                    {
                        //another customer so we should save the last one and get the new customer and put the same infoloans
                        if (info != null)
                        {
                            ReportOut[info.CustomerId] = info;

                            ReportOut.TryGetValue(sqlReader.GetInt32(sqlReader.GetOrdinal("CustomerId")), out info);
                            if (info != null) info.InfoLoans = new List<InfoLoan>();

                            if (info != null &&
                                info.CustomerId == sqlReader.GetInt32(sqlReader.GetOrdinal("CustomerId")))
                            {
                                info.InfoLoans.Add(AddNewLoan(sqlReader));
                                info.TotalLoan += sqlReader.GetDecimal(sqlReader.GetOrdinal("MonthlyQuota"));
                            }
                        }
                    }
                }
                sqlReader.Close();

                var customerIds2 = new List<int>();

                foreach (var repo in ReportOut)
                {
                    if (repo.Value.TotalLoan > 0)
                        customerIds2.Add(repo.Value.CustomerId);
                }

                if (customerIds2.Count > 0)
                    UpdateDataLoan(customerIds2);

            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "Dao.GetLoanPayments(" + string.Join(",", customerIds.ToArray()) + ")", ex.Message);
            }
        }

        private void MergeData(IEnumerable<int> customerIds)
        {
            var contributions = new List<string>();
            var loans = new List<string>();

            foreach (var customerId in customerIds)
            {
                if (FileOut.ContainsKey(customerId))
                {
                    var lineContribution = string.Format("{0}        {1}{2}",
                    FileOut[customerId].Replace(AMOUNT,
                        (Math.Round(ReportOut[customerId].TotalContribution * 100)).
                            ToString(CultureInfo.InvariantCulture).PadLeft(13, '0')).Replace(".", ""),
                            Batch.PeriodYear, Batch.PeriodMonth.ToString("00"));

                    contributions.Add(lineContribution);

                    string lineLoan = null;
                    if (ReportOut[customerId].TotalLoan > 0)
                    {
                        foreach (var infoLoan in ReportOut[customerId].InfoLoans)
                        {
                            lineLoan = string.Format("{0}        {1}{2}",
                            FileOut[customerId].Replace(AMOUNT,
                            (Math.Round(infoLoan.MonthlyQuota * 100)).
                                ToString(CultureInfo.InvariantCulture).PadLeft(13, '0')).Replace(".", "").Replace(",", ""),
                                Batch.PeriodYear, Batch.PeriodMonth.ToString("00"));

                            loans.Add(lineLoan);
                        }
                    }
                }
            }

            Result = new List<string>();

            foreach (var contribution in contributions)
                Result.Add(contribution);

            foreach (var loan in loans)
                Result.Add(loan);
        }

        private static InfoLoan AddNewLoan(IDataRecord sqlReader)
        {
            return new InfoLoan
            {
                LoanPaymentId = sqlReader.GetInt32(sqlReader.GetOrdinal("LoanPaymentId")),
                LoanId = sqlReader.GetInt32(sqlReader.GetOrdinal("LoanId")),
                Quota = sqlReader.GetInt32(sqlReader.GetOrdinal("Quota")),
                MonthlyQuota = sqlReader.GetDecimal(sqlReader.GetOrdinal("MonthlyQuota")),
                MonthlyFee = sqlReader.GetDecimal(sqlReader.GetOrdinal("MonthlyFee")),
                MonthlyCapital = sqlReader.GetDecimal(sqlReader.GetOrdinal("MonthlyCapital")),
                MonthlyPayed = sqlReader.GetDecimal(sqlReader.GetOrdinal("MonthlyPayed")),
                StateId = sqlReader.GetInt32(sqlReader.GetOrdinal("StateId")),
                IsAutomatic = sqlReader.GetBoolean(sqlReader.GetOrdinal("IsAutomatic")),
                BankName = sqlReader.GetString(sqlReader.GetOrdinal("BankName")),
                AccountNumber = sqlReader.GetString(sqlReader.GetOrdinal("AccountNumber")),
                TransactionNumber = sqlReader.GetString(sqlReader.GetOrdinal("TransactionNumber")),
                Reference = sqlReader.GetString(sqlReader.GetOrdinal("Reference")),
                Description = sqlReader.GetString(sqlReader.GetOrdinal("Description"))
            };
        }

        private void GetCustomer(out List<int> customerIds)
        {
            customerIds = new List<int>();
            try
            {
                Log.InfoFormat("Action: {0}", "Dao.GetCustomer(" + string.Join(",", customerIds.ToArray()) + ")");

                Sql = " SELECT EntityId, Attribute =[Key], Value FROM GenericAttribute " +
                      " WHERE KeyGroup='Customer' and  [Key] in ('Dni','AdmCode') AND " +
                      " EntityId IN ( SELECT EntityId FROM GenericAttribute WHERE [Key]='MilitarySituationId' AND Value=1)";

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

                        FileOut.Add(entityId, string.Format("8A{0}8001{1}0000000000000{2}", admCode, AMOUNT, NUMBER));
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

        private void UpdateDataContribution(List<int> customerIds)
        {
            try
            {
                Log.InfoFormat("Action: {0}", "Dao.UpdateDataContribution(" + string.Join(",", customerIds.ToArray()) + ")");

                Sql = "UPDATE ContributionPayment SET StateId =2 WHERE ID IN ( " +
                  " SELECT  cp.Id " +
                  " FROM ContributionPayment cp " +
                  " INNER JOIN  Contribution c on c.Id=cp.ContributionId " +
                  " WHERE c.CustomerId IN (" + string.Join(",", customerIds.ToArray()) + ") AND  " +
                  " YEAR(cp.ScheduledDateOnUtc)=@Year AND  " +
                  " MONTH(cp.ScheduledDateOnUtc)=@Month  ) ";

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
                Log.FatalFormat("Action: {0} Error: {1}", "Dao.UpdateDataContribution(" + string.Join(",", customerIds.ToArray()) + ")", ex.Message);
            }
        }

        private void UpdateDataLoan(List<int> customerIds)
        {
            try
            {
                Log.InfoFormat("Action: {0}", "Dao.UpdateDataLoan(" + string.Join(",", customerIds.ToArray()) + ")");

                Sql = "UPDATE LoanPayment SET StateId =2 WHERE ID IN ( " +
                  " SELECT  cp.Id " +
                  " FROM LoanPayment cp " +
                  " INNER JOIN  Loan c on c.Id=cp.LoanId " +
                  " WHERE c.CustomerId IN (" + string.Join(",", customerIds.ToArray()) + ") AND  " +
                  " YEAR(cp.ScheduledDateOnUtc)=@Year AND  " +
                  " MONTH(cp.ScheduledDateOnUtc)=@Month  ) ";

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
                Log.FatalFormat("Action: {0} Error: {1}", "Dao.UpdateDataLoan(" + string.Join(",", customerIds.ToArray()) + ")", ex.Message);
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
