using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Ks.Batch.Util;
using Ks.Batch.Util.Model;
using Topshelf.Logging;

namespace Ks.Batch.Merge
{
    public class Dao : DaoBase
    {
        private new static readonly LogWriter Log = HostLogger.Get<Dao>();
        private List<Info> _listInfo = new List<Info>();


        public Dao(string connetionString)
            : base(connetionString)
        {
        }

        public Dictionary<Report, List<Info>> GetData()
        {
            Log.InfoFormat("Action: Extrayendo los datos de entrada y los de salida");

            var infoList = new Dictionary<Report, List<Info>>();
            try
            {
                Connect();
                if (IsConnected)
                {
                    Sql = "SELECT * FROM Report WHERE ParentKey in " +
                          " (SELECT TOP 1 ParentKey  FROM Report WHERE StateId=" + (int)ReportState.InProcess +
                          "  and len(name)<>0  " +
                          "group by ParentKey having count(ParentKey)=2) ";

                    Command = new SqlCommand(Sql, Connection);
                    var sqlReader = Command.ExecuteReader();
                    while (sqlReader.Read())
                    {
                        var listInfo = XmlHelper.XmlToObject<List<Info>>(sqlReader.GetString(3));
                        Log.InfoFormat("Action: Reporte extraido: {0} con {1} registros a sincornizar", sqlReader.GetString(2), listInfo.Count());

                        infoList.Add(new Report
                        {
                            Id = sqlReader.GetInt32(0),
                            Key = sqlReader.GetGuid(1),
                            Name = sqlReader.GetString(2),
                            Value = sqlReader.GetString(3),
                            StateId = sqlReader.GetInt32(5),
                            Period = sqlReader.GetString(6),
                            Source = sqlReader.GetString(7),
                            ParentKey = sqlReader.GetGuid(8),
                            DateUtc = sqlReader.GetDateTime(9)
                        },
                        listInfo);
                    }
                    sqlReader.Close();
                }
                Log.InfoFormat("Action: Extracción exitosa de {0} reportes", infoList.Count());
                return infoList;
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Time: {0}: Action: {1}", DateTime.Now, ex.Message);
                Close();
                return null;

            }

        }

        #region Copere

        /// <summary>
        /// 
        /// </summary>
        /// <param name="report">Resumen de la operacion</param>
        /// <param name="dataEntry">Datos que ingresan al sistema</param>
        /// <param name="dataInternal">Datos que salen del sistema</param>
        /// <param name="bankName">Entidad Bancaria</param>
        /// <param name="isPre">especifica si se va a realizar una simulacion o no</param>
        public void ProcessCopere(Report report, List<Info> dataEntry, List<Info> dataInternal, string bankName, bool isPre)
        {
            var infoContributionNoCash = new List<InfoContribution>(); // sin liquidez
            var infoContributionPayedComplete = new List<InfoContribution>(); // los pagos completos
            var infoContributionIncomplete = new List<InfoContribution>(); // los puchos

            var infoLoanNoCash = new List<InfoLoan>(); // sin liquidez
            var infoLoanPayedComplete = new List<InfoLoan>(); // los pagos completos
            var infoLoanIncomplete = new List<InfoLoan>(); // los puchos


            #region Split-List

            Info response;
            //Para cada registro enviado hacia el copere
            foreach (var recordInternal in dataInternal)
            {
                if (recordInternal.InfoContribution == null)
                    recordInternal.InfoContribution = new InfoContribution();

                recordInternal.InfoContribution.BankName = bankName;
                recordInternal.InfoContribution.Description = "Proceso automática por el sistema ACMR";

                //buscamos por el codigo administrativo en la lista de respuesta
                response = dataEntry.FirstOrDefault(x => x.AdminCode == recordInternal.AdminCode);

                //No el asociado dentro de la lista de entrada
                if (response == null)
                {
                    #region Sin liquidez en Contribution y Loan

                    #region Sin Liquidez en aportaciones

                    recordInternal.InfoContribution.StateId = (int)ContributionState.SinLiquidez;
                    infoContributionNoCash.Add(MappingContribution(recordInternal.InfoContribution));

                    #endregion

                    #region Sin Liquides en Prestamos

                    foreach (var infoLoan in recordInternal.InfoLoans)
                    {
                        infoLoan.StateId = (int)ContributionState.SinLiquidez;
                        infoLoan.BankName = bankName;
                        infoLoanNoCash.Add(MappingLoan(infoLoan));
                    }

                    #endregion

                    #endregion
                }
                else
                {
                    if (recordInternal.TotalContribution == response.TotalPayed)
                    {
                        #region Pagado completo en aportaciones y sin liquidez en prestamos

                        #region Pago completo de la aportacion

                        recordInternal.InfoContribution.StateId = (int)ContributionState.Pagado;
                        recordInternal.InfoContribution.AmountPayed = response.TotalPayed;
                        infoContributionPayedComplete.Add(MappingContribution(recordInternal.InfoContribution));

                        #endregion

                        #region Sin Liquides en Prestamos

                        foreach (var infoLoan in recordInternal.InfoLoans)
                        {
                            infoLoan.StateId = (int)ContributionState.SinLiquidez;
                            infoLoan.BankName = bankName;
                            infoLoanNoCash.Add(MappingLoan(infoLoan));
                        }

                        #endregion

                        #endregion
                    }

                    if (recordInternal.TotalContribution > response.TotalPayed)
                    {
                        #region Pago por puchos en aportacion  y sin liquidez en prestamos

                        #region Pago por puchos en aportaciones

                        recordInternal.InfoContribution.StateId = (int)ContributionState.PagoParcial;
                        recordInternal.InfoContribution.AmountPayed = response.TotalPayed;

                        infoContributionIncomplete.Add(MappingContribution(recordInternal.InfoContribution));

                        #endregion

                        #region Sin Liquides en Prestamos

                        foreach (var infoLoan in recordInternal.InfoLoans)
                        {
                            infoLoan.StateId = (int)ContributionState.SinLiquidez;
                            infoLoan.BankName = bankName;
                            infoLoanNoCash.Add(MappingLoan(infoLoan));
                        }

                        #endregion

                        #endregion
                    }

                    if (recordInternal.TotalContribution < response.TotalPayed)
                    {
                        #region Pago total en aportacion y en puchos en prestamos

                        #region Pago total en aportacion

                        recordInternal.InfoContribution.StateId = (int)ContributionState.Pagado;
                        recordInternal.InfoContribution.AmountPayed = recordInternal.InfoContribution.AmountTotal;
                        infoContributionPayedComplete.Add(MappingContribution(recordInternal.InfoContribution));

                        #endregion

                        response.TotalPayed -= recordInternal.InfoContribution.AmountTotal;

                        foreach (var infoLoan in recordInternal.InfoLoans)
                        {
                            if (response.TotalPayed >= infoLoan.MonthlyQuota)
                            {
                                #region Pago completo en prestamo

                                infoLoan.StateId = (int)ContributionState.Pagado;
                                infoLoan.MonthlyPayed = infoLoan.MonthlyQuota;
                                infoLoan.BankName = bankName;
                                infoLoanPayedComplete.Add(MappingLoan(infoLoan));

                                #endregion

                                response.TotalPayed -= infoLoan.MonthlyQuota;
                            }
                            else
                            {
                                if (response.TotalPayed > 0)
                                {
                                    #region Pago Parcial del prestamo

                                    infoLoan.StateId = (int)ContributionState.PagoParcial;
                                    infoLoan.MonthlyPayed = response.TotalPayed;
                                    infoLoan.BankName = bankName;
                                    infoLoanIncomplete.Add(MappingLoan(infoLoan));

                                    #endregion

                                    response.TotalPayed = 0;

                                }
                                else
                                {
                                    #region No hay liquidez para el pago

                                    infoLoan.StateId = (int)ContributionState.SinLiquidez;
                                    infoLoan.MonthlyPayed = 0;
                                    infoLoan.BankName = bankName;
                                    infoLoanNoCash.Add(MappingLoan(infoLoan));

                                    #endregion
                                }
                            }
                        }

                        #endregion
                    }
                }
            }

            #endregion

            #region ContributionPayment

            var source = "";
            var value = "";

            if (infoContributionPayedComplete.Count > 0)
            {
                UpdateContributionPaymentCopere(infoContributionPayedComplete, report.Period, isPre);
                if (isPre)
                {
                    source = ConfigurationManager.AppSettings["ContributionPayedComplete"];
                    value = XmlHelper.Serialize2String(new List<InfoContribution>(infoContributionPayedComplete));
                    AddPreReport(report, value, source);
                }
                    
            }
            if (infoContributionIncomplete.Count > 0)
            {
                UpdateContributionPaymentCopere(infoContributionIncomplete, report.Period, isPre);
                if (isPre)
                {
                    source = ConfigurationManager.AppSettings["ContributionIncomplete"];
                    value = XmlHelper.Serialize2String(new List<InfoContribution>(infoContributionIncomplete));
                    AddPreReport(report, value, source);
                }
            }
            if (infoContributionNoCash.Count > 0)
            {
                UpdateContributionPaymentCopere(infoContributionNoCash, report.Period, isPre);
                if (isPre)
                {
                    source = ConfigurationManager.AppSettings["ContributionNoCash"];
                    value = XmlHelper.Serialize2String(new List<InfoContribution>(infoContributionNoCash));
                    AddPreReport(report, value, source);
                }
            }

            #endregion

            #region LoanPayment
            if (infoLoanPayedComplete.Count > 0)
            {
                UpdateLoanPaymentCopere(infoLoanPayedComplete, report.Period, isPre);
                if (isPre)
                {
                    source = ConfigurationManager.AppSettings["LoanPayedComplete"];
                    value = XmlHelper.Serialize2String(new List<InfoLoan>(infoLoanPayedComplete));
                    AddPreReport(report, value, source);
                }
            }
            if (infoLoanIncomplete.Count > 0)
            {
                UpdateLoanPaymentCopere(infoLoanIncomplete, report.Period, isPre);
                if (isPre)
                {
                    source = ConfigurationManager.AppSettings["LoanIncomplete"];
                    value = XmlHelper.Serialize2String(new List<InfoLoan>(infoLoanIncomplete));
                    AddPreReport(report, value, source);
                }
            }
            if (infoLoanNoCash.Count > 0)
            {
                UpdateLoanPaymentCopere(infoLoanNoCash, report.Period, isPre);
                if (isPre)
                {
                    source = ConfigurationManager.AppSettings["LoanNoCash"];
                    value = XmlHelper.Serialize2String(new List<InfoLoan>(infoLoanNoCash));
                    AddPreReport(report, value, source);
                }
            }

            #endregion

            //si no es simulacion entonces si se cierra el flujo, caso contrario ya se actualizo en el pasado
            if (!isPre)
                CloseReport(report);
            
        }
        private void UpdateContributionPaymentCopere(List<InfoContribution> info, string period, bool isPre)
        {
            if (isPre)
            {
                Log.InfoFormat("Action: No se actualiza las àportaciones ya que se esta simulando el servicio");
            }
            else
            {
                try
                {
                    Log.InfoFormat("Action: {0}", "Dao.UpdateContributionPaymentCopere(" + string.Join(",", info.Select(x => x.ContributionPaymentId)) + ", " + period + ")");

                    var year = period.Substring(0, 4);
                    var month = period.Substring(4, 2);
                    var xml = XmlHelper.Serialize2String(info, true);
                    xml = xml.Replace('\n', ' ');
                    xml = xml.Replace('\r', ' ');
                    xml = xml.Replace("<?xml version=\"1.0\"?>", "");

                    Sql = "UpdateContributionPaymentCopere @XmlPackage,@Year, @Month";
                    Command = new SqlCommand(Sql, Connection);
                    Command.CommandTimeout = 1800;
                    Command.Parameters.AddWithValue("@XmlPackage", xml);
                    Command.Parameters.AddWithValue("@Year", Convert.ToInt32(year));
                    Command.Parameters.AddWithValue("@Month", Convert.ToInt32(month));
                    Command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Log.FatalFormat("Action: {0} Error: {1}", "Dao.UpdateContributionPaymentCopere(" + string.Join(",", info.Select(x => x.ContributionPaymentId)) + ", " + period + ")", ex.Message);
                }
            }

        }
        private void UpdateLoanPaymentCopere(List<InfoLoan> info, string period, bool isPre)
        {
            if (isPre)
            {
                Log.InfoFormat("Action: No se actualiza las àportaciones ya que se esta simulando el servicio");
            }
            else
            {
                try
                {
                    Log.InfoFormat("Action: {0}", "Dao.UpdateLoanPaymentCopere(" + string.Join(",", info.Select(x => x.LoanPaymentId)) + ", " + period + ")");

                    var year = period.Substring(0, 4);
                    var month = period.Substring(4, 2);
                    var xml = XmlHelper.Serialize2String(info, true);
                    xml = xml.Replace('\n', ' ');
                    xml = xml.Replace('\r', ' ');
                    xml = xml.Replace("<?xml version=\"1.0\"?>", "");

                    Sql = "UpdateLoanPaymentCopere @XmlPackage,@Year, @Month";
                    Command = new SqlCommand(Sql, Connection);
                    Command.CommandTimeout = 1800;
                    Command.Parameters.AddWithValue("@XmlPackage", xml);
                    Command.Parameters.AddWithValue("@Year", Convert.ToInt32(year));
                    Command.Parameters.AddWithValue("@Month", Convert.ToInt32(month));
                    Command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Log.FatalFormat("Action: {0} Error: {1}", "Dao.UpdateLoanPaymentCopere(" + string.Join(",", info.Select(x => x.LoanPaymentId)) + ", " + period + ")", ex.Message);
                }
            }
        }

        #endregion

        #region Caja

        public void ProcessCaja(Report report, List<Info> listResponse, List<Info> listRequest, string bankName, bool isPre)
        {
            var infoContributionNoCash = new List<InfoContribution>(); // sin liquidez
            var infoContributionPayedComplete = new List<InfoContribution>(); // los pagos completos
            var infoContributionIncomplete = new List<InfoContribution>(); // los puchos

            var infoLoanNoCash = new List<InfoLoan>(); // sin liquidez
            var infoLoanPayedComplete = new List<InfoLoan>(); // los pagos completos
            var infoLoanIncomplete = new List<InfoLoan>(); // los puchos


            #region SplitList

            Info response;
            foreach (var request in listRequest)
            {
                request.InfoContribution.BankName = bankName;
                request.InfoContribution.Description = "Proceso automática por el sistema ACMR";

                var info1 = request;
                response = listResponse.FirstOrDefault(x => x.AdminCode == info1.AdminCode);
                if (response == null)
                {
                    #region Sin liquidez en Contribution y Loan

                    #region Sin Liquidez en aportaciones

                    request.InfoContribution.StateId = (int)ContributionState.SinLiquidez;
                    infoContributionNoCash.Add(MappingContribution(request.InfoContribution));

                    #endregion

                    #region Sin Liquides en Prestamos

                    foreach (var infoLoan in request.InfoLoans)
                    {
                        infoLoan.StateId = (int)ContributionState.SinLiquidez;
                        infoLoan.BankName = bankName;
                        infoLoanNoCash.Add(MappingLoan(infoLoan));
                    }

                    #endregion

                    #endregion
                }
                else
                {
                    if (request.TotalContribution == response.TotalPayed)
                    {
                        #region Pagado completo en aportaciones y sin liquidez en prestamos

                        #region Pago completo de la aportacion

                        request.InfoContribution.StateId = (int)ContributionState.Pagado;
                        request.InfoContribution.AmountPayed = response.TotalPayed;
                        infoContributionPayedComplete.Add(MappingContribution(request.InfoContribution));

                        #endregion

                        #region Sin Liquides en Prestamos

                        foreach (var infoLoan in request.InfoLoans)
                        {
                            infoLoan.StateId = (int)ContributionState.SinLiquidez;
                            infoLoan.BankName = bankName;
                            infoLoanNoCash.Add(MappingLoan(infoLoan));
                        }

                        #endregion

                        #endregion
                    }

                    if (request.TotalContribution > response.TotalPayed)
                    {
                        #region Pago por puchos en aportacion  y sin liquidez en prestamos

                        #region Pago por puchos en aportaciones

                        request.InfoContribution.StateId = (int)ContributionState.PagoParcial;
                        request.InfoContribution.AmountPayed = response.TotalPayed;

                        infoContributionIncomplete.Add(MappingContribution(request.InfoContribution));

                        #endregion

                        #region Sin Liquides en Prestamos

                        foreach (var infoLoan in request.InfoLoans)
                        {
                            infoLoan.StateId = (int)ContributionState.SinLiquidez;
                            infoLoan.BankName = bankName;
                            infoLoanNoCash.Add(MappingLoan(infoLoan));
                        }

                        #endregion

                        #endregion
                    }

                    if (request.TotalContribution < response.TotalPayed)
                    {
                        #region Pago total en aportacion y en puchos en prestamos

                        #region Pago total en aportacion

                        request.InfoContribution.StateId = (int)ContributionState.Pagado;
                        request.InfoContribution.AmountPayed = request.InfoContribution.AmountTotal;
                        infoContributionPayedComplete.Add(MappingContribution(request.InfoContribution));

                        #endregion

                        response.TotalPayed -= request.InfoContribution.AmountTotal;

                        foreach (var infoLoan in request.InfoLoans)
                        {
                            if (response.TotalPayed >= infoLoan.MonthlyQuota)
                            {
                                #region Pago completo en prestamo

                                infoLoan.StateId = (int)ContributionState.Pagado;
                                infoLoan.MonthlyPayed = infoLoan.MonthlyQuota;
                                infoLoan.BankName = bankName;
                                infoLoanPayedComplete.Add(MappingLoan(infoLoan));

                                #endregion

                                response.TotalPayed -= infoLoan.MonthlyQuota;
                            }
                            else
                            {
                                if (response.TotalPayed > 0)
                                {
                                    #region Pago Parcial del prestamo

                                    infoLoan.StateId = (int)ContributionState.PagoParcial;
                                    infoLoan.MonthlyPayed = response.TotalPayed;
                                    infoLoan.BankName = bankName;
                                    infoLoanIncomplete.Add(MappingLoan(infoLoan));

                                    #endregion

                                    response.TotalPayed = 0;

                                }
                                else
                                {
                                    #region No hay liquidez para el pago

                                    infoLoan.StateId = (int)ContributionState.SinLiquidez;
                                    infoLoan.MonthlyPayed = 0;
                                    infoLoan.BankName = bankName;
                                    infoLoanNoCash.Add(MappingLoan(infoLoan));

                                    #endregion
                                }
                            }
                        }

                        #endregion
                    }
                }
            }

            #endregion

            #region ContributionPayment


            if (infoContributionPayedComplete.Count > 0)
            {
                var infoPartial = LinqExtensions.Split(infoContributionPayedComplete, 50);
                foreach (var info in infoPartial)
                    UpdateContributionPaymentCaja(info.ToList(), report.Period);
            }
            if (infoContributionIncomplete.Count > 0)
            {
                var infoPartial = LinqExtensions.Split(infoContributionIncomplete, 50);
                foreach (var info in infoPartial)
                    UpdateContributionPaymentCaja(info.ToList(), report.Period);
            }
            if (infoContributionNoCash.Count > 0)
            {
                var infoPartial = LinqExtensions.Split(infoContributionNoCash, 50);
                foreach (var info in infoPartial)
                    UpdateContributionPaymentCaja(info.ToList(), report.Period);
            }

            #endregion

            #region LoanPayment
            if (infoLoanPayedComplete.Count > 0)
            {
                var infoPartial = LinqExtensions.Split(infoLoanPayedComplete, 50);
                foreach (var info in infoPartial)
                    UpdateLoanPaymentCaja(info.ToList(), report.Period);
            }
            if (infoLoanIncomplete.Count > 0)
            {
                var infoPartial = LinqExtensions.Split(infoLoanIncomplete, 50);
                foreach (var info in infoPartial)
                    UpdateLoanPaymentCaja(info.ToList(), report.Period);
            }
            if (infoLoanNoCash.Count > 0)
            {
                var infoPartial = LinqExtensions.Split(infoLoanNoCash, 50);
                foreach (var info in infoPartial)
                    UpdateLoanPaymentCaja(info.ToList(), report.Period);
            }

            #endregion

            CloseReport(report);
        }
        private void UpdateContributionPaymentCaja(List<InfoContribution> info, string period)
        {
            try
            {
                Log.InfoFormat("Action: {0}", "Dao.UpdateContributionPaymentCaja(" + string.Join(",", info.Select(x => x.ContributionPaymentId)) + ", " + period + ")");

                var year = period.Substring(0, 4);
                var month = period.Substring(4, 2);
                var xml = XmlHelper.Serialize2String(info, true);
                xml = xml.Replace('\n', ' ');
                xml = xml.Replace('\r', ' ');
                xml = xml.Replace("<?xml version=\"1.0\"?>", "");

                Sql = "UpdateContributionPaymentCaja @XmlPackage,@Year, @Month";
                Command = new SqlCommand(Sql, Connection);
                Command.CommandTimeout = 1800;
                Command.Parameters.AddWithValue("@XmlPackage", xml);
                Command.Parameters.AddWithValue("@Year", Convert.ToInt32(year));
                Command.Parameters.AddWithValue("@Month", Convert.ToInt32(month));
                Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "Dao.UpdateContributionPaymentCaja(" + string.Join(",", info.Select(x => x.ContributionPaymentId)) + ", " + period + ")", ex.Message);
            }
        }
        private void UpdateLoanPaymentCaja(List<InfoLoan> info, string period)
        {
            try
            {
                Log.InfoFormat("Action: {0}", "Dao.UpdateLoanPayment(" + string.Join(",", info.Select(x => x.LoanPaymentId)) + ", " + period + ")");

                var year = period.Substring(0, 4);
                var month = period.Substring(4, 2);
                var xml = XmlHelper.Serialize2String(info, true);
                xml = xml.Replace('\n', ' ');
                xml = xml.Replace('\r', ' ');
                xml = xml.Replace("<?xml version=\"1.0\"?>", "");

                Sql = "UpdateLoanPaymentCaja @XmlPackage,@Year, @Month";
                Command = new SqlCommand(Sql, Connection);
                Command.CommandTimeout = 1800;
                Command.Parameters.AddWithValue("@XmlPackage", xml);
                Command.Parameters.AddWithValue("@Year", Convert.ToInt32(year));
                Command.Parameters.AddWithValue("@Month", Convert.ToInt32(month));
                Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "Dao.UpdateContributionPayment(" + string.Join(",", info.Select(x => x.LoanPaymentId)) + ", " + period + ")", ex.Message);
            }
        }

        #endregion

        #region Utilities

        private InfoContribution MappingContribution(InfoContribution info)
        {
            return new InfoContribution
            {
                ContributionPaymentId = info.ContributionPaymentId,
                Number = info.Number,
                NumberOld = info.NumberOld,
                ContributionId = info.ContributionId,
                Amount1 = info.Amount1,
                Amount2 = info.Amount2,
                Amount3 = info.Amount3,
                AmountOld = info.AmountOld,
                AmountTotal = info.AmountTotal,
                AmountPayed = info.AmountPayed,
                StateId = info.StateId,
                IsAutomatic = info.IsAutomatic,
                BankName = info.BankName,
                AccountNumber = info.AccountNumber,
                TransactionNumber = info.TransactionNumber,
                Reference = info.Reference,
                Description = info.Description
            };
        }
        private InfoLoan MappingLoan(InfoLoan info)
        {
            return new InfoLoan
            {
                LoanPaymentId = info.LoanPaymentId,
                Quota = info.Quota,
                LoanId = info.LoanId,
                MonthlyQuota = info.MonthlyQuota,
                MonthlyFee = info.MonthlyFee,
                MonthlyCapital = info.MonthlyCapital,
                MonthlyPayed = info.MonthlyPayed,
                StateId = info.StateId,
                IsAutomatic = info.IsAutomatic,
                BankName = info.BankName,
                AccountNumber = info.AccountNumber,
                TransactionNumber = info.TransactionNumber,
                Reference = info.Reference,
                Description = info.Description
            };
        }

        private void CloseReport(Report report)
        {
            try
            {
                Log.InfoFormat("Action: {0}", "Dao.CloseReport(" + report.ParentKey + ")");

                Sql = "UPDATE Report SET StateId=@StateId WHERE ParentKey=@ParentKey ";

                Command = new SqlCommand(Sql, Connection);
                Command.Parameters.AddWithValue("@StateId", ReportState.Completed);
                Command.Parameters.AddWithValue("@ParentKey", report.ParentKey);

                Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "Dao.CloseReport(" + report.ParentKey + ")", ex.Message);
            }

        }
        private void AddPreReport(Report report, string value, string source)
        {
            DeleteReport(report.Period, source);
            CreateReportPre(report, source, value);
        }

        private Dictionary<int, List<InfoContribution>> SplitInfoContribution(List<InfoContribution> data)
        {
            int x = 1;
            int y = 1;
            var result = new Dictionary<int, List<InfoContribution>>();

            List<InfoContribution> partial = new List<InfoContribution>();
            foreach (var infoContribution in data)
            {
                if (x > 500)
                {
                    x = 1;
                    result.Add(y, partial);
                    partial = new List<InfoContribution>();
                    y++;
                }
                else
                {
                    partial.Add(infoContribution);
                    x++;
                }
            }

            return result;
        }
        private Dictionary<int, List<InfoLoan>> SplitInfoLoan(List<InfoLoan> data)
        {
            int x = 1;
            int y = 1;
            var result = new Dictionary<int, List<InfoLoan>>();

            List<InfoLoan> partial = new List<InfoLoan>();
            foreach (var infoContribution in data)
            {
                if (x > 500)
                {
                    x = 1;
                    result.Add(y, partial);
                    partial = new List<InfoLoan>();
                    y++;
                }
                else
                {
                    partial.Add(infoContribution);
                    x++;
                }
            }

            return result;
        }

        #endregion

    }
}