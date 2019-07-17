using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
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

        public List<string> Process(string path, ScheduleBatch batch)
        {
            Batch = batch;
            try
            {
                Log.InfoFormat("Action: {0}", "Dao.Process(" + batch.SystemName + ")");

                ReportOut = new Dictionary<int, Info>();

                GetCustomer(out List<int> customerIds);

                GetContributionPayments(customerIds);

                #region Write to xml file
                using (var stream = new MemoryStream())
                {
                    var properties = new[] { "MesProc", "CodDes", "NumAdm", "Monto", "FechaDesem", "HoraDesem", "NroCuota", "TotalCuotas", "Saldo" };
                    var data = new Dictionary<int, Dictionary<int, string>>();
                    var index = 0;
                    foreach (var info in ReportOut.Values)
                    {
                        var tmp = new Dictionary<int, string>
                        {
                            { 0, info.Year.ToString() + info.Month.ToString("D2") },
                            { 1, "8001" },
                            { 2, info.AdminCode },
                            { 3, info.InfoContribution.AmountTotal.ToString() }
                        };
                        data.Add(index, tmp);
                        index++;
                    }
                    var fileName = Path.Combine(Path.Combine(path, Batch.FolderMoveToDone), "8001-" + FileHelper.GetDateFormat(DateTime.Now) + ".xls");
                    if (File.Exists(fileName))
                        File.Delete(fileName);
                    ExcelFile.CreateReport("Aportaciones", 1, stream, properties, data, fileName);
                }
                #endregion

                GetLoanPayment(customerIds);

                #region Write to xml file
                using (var stream = new MemoryStream())
                {
                    var properties = new[] { "MES_PROCESO", "NRO_ADMINIST", "COD_DESCUENTO", "MTO_DESCUENTO", "NUM_CUOTAS", "TOT_CUOTAS", "FEC_DESEMBOLSO", "HOR_DESEMBOLSO", "MTO_SALDO_PRES" };
                    var data = new Dictionary<int, Dictionary<int, string>>();
                    var index = 0;
                    foreach (var info in ReportOut.Values)
                    {
                        if (info.InfoLoans != null)
                        {
                            foreach (var loan in info.InfoLoans)
                            {
                                var tmp = new Dictionary<int, string>
                                {
                                    { 0, info.Year.ToString() + info.Month.ToString("D2") },
                                    { 1, info.AdminCode },
                                    { 2, "8033" },
                                    { 3, loan.MonthlyQuota.ToString()},
                                    { 4, loan.Quota.ToString() },
                                    { 5, loan.Period.ToString() },
                                    { 6, FileHelper.GetDate(DateTime.Now) },
                                    { 7, FileHelper.GetTime(DateTime.Now) },
                                    { 8, loan.NoPayedYet.ToString() }
                                };
                                data.Add(index, tmp);
                                index++;
                            }
                        }
                    }
                    var fileName = Path.Combine(Path.Combine(path, Batch.FolderMoveToDone), "8033-" + FileHelper.GetDateFormat(DateTime.Now) + ".xls");
                    if (File.Exists(fileName))
                        File.Delete(fileName);
                    ExcelFile.CreateReport("Apoyo", 1, stream, properties, data, fileName);
                }
                #endregion

                //copere send contribution and loan in separates
                MergeData(customerIds);

                if (FileOut.Count != 0)
                {
                    DeleteReport(Batch.PeriodYear.ToString("0000") + Batch.PeriodMonth.ToString("00"), Batch.SystemName);
                    DeleteReport(Batch.PeriodYear.ToString("0000") + Batch.PeriodMonth.ToString("00"), "Ks.Batch.Copere.In");
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

        /// <summary>
        /// 1)
        ///     Busco los Dnis y los AdmCode de aquellos militares en situacion militar en estado CustomerMilitarySituation.Actividad
        /// </summary>
        /// <param name="customerIds">Retorna los Ids encontrados para este servicio</param>
        private void GetCustomer(out List<int> customerIds)
        {
            customerIds = new List<int>();
            try
            {
                Log.InfoFormat("Action: 1) {0}", "Buscando a los miliatares activos en estado de activisad (code = 1)");

                Sql = " SELECT EntityId, Attribute =[Key], Value FROM GenericAttribute " +
                      " WHERE KeyGroup='Customer' and  [Key] in ('Dni','AdmCode') AND " +
                      " EntityId IN ( SELECT EntityId FROM GenericAttribute " +
                      " WHERE [Key]='MilitarySituationId' AND Value=" + (int)CustomerMilitarySituation.Actividad + " AND EntityId IN " +
                      " (SELECT Id FROM CUSTOMER WHERE ACTIVE=" + (int)State.Active + "  ) ) " +
                      " ORDER BY 1 ";

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
                        ReportOut.Add(entityId, new Info { InfoContribution = null, InfoLoans = null, CustomerId = entityId, AdminCode = admCode, HasAdminCode = true, Dni = dni, HasDni = true });
                        entityId = repeatEntityId = count = 0;
                    }
                }
                sqlReader.Close();
                Log.InfoFormat("Action: {0} {1}", "Cantidad de Militares activos en estado de actividad  = ", customerIds.Count);
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "Dao.GetCustomer(" + string.Join(",", customerIds.ToArray()) + ")", ex.Message);
            }
        }

        /// <summary>
        /// 2)
        ///     Busco las Contribuciones en estado ContributionState.Pendiente
        ///     Las cuotas son conseguidas del valor que posee el Batch
        /// </summary>
        /// <param name="customerIds">lista de Ids a ser consultados</param>
        private void GetContributionPayments(List<int> customerIds)
        {
            try
            {
                Log.InfoFormat("Action: 2) {0} {1} {2}", "Buscamos las contribuciones pendientes de los ", customerIds.Count, " aportantes");

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
                      "c.active=" + (int)State.Active + " AND " +
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
                var customerIds2 = new List<int>();

                while (sqlReader.Read())
                {
                    ReportOut.TryGetValue(sqlReader.GetInt32(sqlReader.GetOrdinal("CustomerId")), out Info info);

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
                    ReportOut[sqlReader.GetInt32(0)] = info;
                }
                sqlReader.Close();

                foreach (var pk in ReportOut)
                {
                    if (pk.Value.InfoContribution != null)
                        customerIds2.Add(pk.Key);
                }

                Log.InfoFormat("Action: 2) {0} {1} {2}", "Se encontraron", customerIds2.Count, "Aportantantes con aportaciones pendientes");

                if (customerIds2.Count > 0 && Batch.UpdateData)
                    UpdateDataContribution(customerIds2);

            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "Dao.GetContributionPayments(" + string.Join(",", customerIds.ToArray()) + ")", ex.Message);
            }
        }

        /// <summary>
        /// 3) 
        ///     Actualizo las aportaciones al estado en proceso
        /// </summary>
        /// <param name="customerIds">Ids a quienes van a tener que afectar el cambio</param>
        private void UpdateDataContribution(List<int> customerIds)
        {
            Log.InfoFormat("Action: 3) {0} {1} {2}", "actualizamos los", customerIds.Count, "aportantes al estado en proceso (ContributionState.EnProceso=2)");
            UpdateDataContribution(customerIds, Batch.PeriodYear, Batch.PeriodMonth);
        }

        /// <summary>
        /// 4)
        ///     Busco los apoyos en estado LoanState.Pendiente
        ///     Las cuotas son conseguidas del valor que posee el Batch
        /// </summary>
        /// <param name="customerIds"></param>
        private void GetLoanPayment(List<int> customerIds)
        {
            try
            {
                Log.InfoFormat("Action: 4) {0} {1} {2}", "Buscamos los apoyos pendientes de los ", customerIds.Count, " aportantes");

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
                      " ISNULL(LP.Description,'') as Description, " +
                      " L.Period as Period, " +
                      " ISNULL(X.NoPayedYet,L.TOTALAMOUNT) AS NoPayedYet " +
                      " FROM LoanPayment LP " +
                      " INNER JOIN Loan L on L.Id=lp.LoanId " +
                      " LEFT JOIN (" +
                      "             SELECT LoanId, SUM(_L.TOTALAMOUNT - ISNULL(MonthlyPayed,0)) as NoPayedYet  " +
                      "             FROM  LoanPayment _LP INNER JOIN Loan _L on _L.Id = _LP.LoanId " +
                      "             WHERE _LP.StateId = "+(int)LoanState.Pagado+" AND " +
                      "                   _L.Active= "+(int)State.Active+" AND " +
                      "                   _L.CustomerId IN (" + string.Join(",", customerIds.ToArray()) + ") " +
                      "             GROUP BY _LP.LoanId)X ON X.LoanId = L.Id" +
                      " WHERE " +
                      " L.CustomerId IN (" + string.Join(",", customerIds.ToArray()) + ") and " +
                      " L.Active=" + (int)State.Active + " AND" +
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
                    Value = (int)LoanState.Pendiente
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

                if (customerIds2.Count > 0 && Batch.UpdateData)
                    UpdateDataLoan(customerIds2);

            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "Dao.GetLoanPayments(" + string.Join(",", customerIds.ToArray()) + ")", ex.Message);
            }
        }

        /// <summary>
        /// 5) 
        ///     Actualizo los apoyos al estado en proceso
        /// </summary>
        /// <param name="customerIds">Ids a quienes van a tener que afectar el cambio</param>
        private void UpdateDataLoan(List<int> customerIds)
        {
            Log.InfoFormat("Action: 5) {0} {1} {2}", "actualizamos los", customerIds.Count, "aportantes al estado en proceso (ContributionState.EnProceso=2)");
            UpdateDataLoan(customerIds, Batch.PeriodYear, Batch.PeriodMonth);
        }

        private void MergeData(IEnumerable<int> customerIds)
        {
            var customerIds2 = new List<int>();
            var fileOutTem = new Dictionary<int, string>();
            var reportOut2 = new Dictionary<int, Info>();

            foreach (var customerId in customerIds)
            {
                if (ReportOut[customerId].InfoContribution != null || ReportOut[customerId].InfoLoans != null)
                {
                    string data;
                    customerIds2.Add(customerId);
                    reportOut2.Add(customerId, ReportOut[customerId]);
                    FileOut.TryGetValue(customerId, out data);
                    if (data != null)
                        fileOutTem.Add(customerId, data);
                }
            }

            FileOut.Clear();
            ReportOut.Clear();

            foreach (var customerId in customerIds2)
                FileOut.Add(customerId, fileOutTem[customerId]);

            foreach (var customerId in customerIds2)
                ReportOut.Add(customerId, reportOut2[customerId]);

            var contributions = new List<string>();
            var loans = new List<string>();

            foreach (var customerId in customerIds2)
            {
                if (FileOut.ContainsKey(customerId))
                {
                    if (ReportOut[customerId].TotalContribution > 0)
                    {
                        var lineContribution = string.Format("{0}        {1}{2}",
                        FileOut[customerId].Replace(AMOUNT,
                            (Math.Round(ReportOut[customerId].TotalContribution * 100)).
                                ToString(CultureInfo.InvariantCulture).PadLeft(13, '0')).Replace(".", ""),
                                Batch.PeriodYear, Batch.PeriodMonth.ToString("00"));

                        contributions.Add(lineContribution);
                    }

                    if (ReportOut[customerId].TotalLoan > 0)
                    {
                        foreach (var infoLoan in ReportOut[customerId].InfoLoans)
                        {
                            if (infoLoan.MonthlyQuota > 0)
                            {
                                var lineLoan = string.Format("{0}        {1}{2}",
                                    FileOut[customerId].Replace(AMOUNT,
                                        (Math.Round(infoLoan.MonthlyQuota * 100)).
                                            ToString(CultureInfo.InvariantCulture).PadLeft(13, '0')).Replace(".", "").Replace(",", ""),
                                    Batch.PeriodYear, Batch.PeriodMonth.ToString("00"));

                                loans.Add(lineLoan);
                            }
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
                Description = sqlReader.GetString(sqlReader.GetOrdinal("Description")),
                Period = sqlReader.GetInt32(sqlReader.GetOrdinal("Period")),
                NoPayedYet = sqlReader.GetDecimal(sqlReader.GetOrdinal("NoPayedYet")),
            };
        }

        private void CompleteCustomerName()
        {
            var result = GetUserNames(FileOut.Keys.ToList());

            foreach (var pair in result)
            {
                ReportOut.TryGetValue(pair.Key, out Info info);
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
