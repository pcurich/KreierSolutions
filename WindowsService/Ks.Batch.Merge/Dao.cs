using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Ks.Batch.Util;
using Ks.Batch.Util.Model;
using Ks.Batch.Util.Model.Summary;
using Topshelf.Logging;

namespace Ks.Batch.Merge
{
    public class Dao : DaoBase
    {
        public ServiceSetting ServiceSetting { get; set; }

        private new static readonly LogWriter Log = HostLogger.Get<Dao>();
        private List<Info> _listInfo = new List<Info>();


        public Dao(string connetionString) : base(connetionString)
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
                    Sql = " SELECT * FROM Report WHERE StateId = @StateId and  ParentKey in " +
                          " (SELECT TOP 1 ParentKey  FROM Report WHERE StateId= @ReportStateId  group by ParentKey having count(ParentKey)=2) ";

                    Command = new SqlCommand(Sql, Connection);
                    Command.Parameters.AddWithValue("@StateId", (int)ReportState.InProcess);
                    Command.Parameters.AddWithValue("@ReportStateId", (int)ReportState.InProcess);
                    var sqlReader = Command.ExecuteReader();
                    while (sqlReader.Read())
                    {
                        var listInfo = XmlHelper.XmlToObject<List<Info>>(sqlReader.GetString(3));
                        Log.InfoFormat("Action: Reporte extraido: {0} con {1} registros a sincornizar para el periodo {2} ", sqlReader.GetString(2), listInfo.Count(), sqlReader.GetString(6));

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
                Log.InfoFormat("Action: Extracción exitosa de {0} registros", infoList.Count());
                return infoList;
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Time: {0}: Action: {1}", DateTime.Now, ex.Message);
                Close();
                return null;
            }
        }

        #region Caja + Copere
        public IDictionary<string, object> Process(Report report,List<Info> inData, List<Info> outData, string bankName, string accountNumber)
        {
            var infoContributionNoCash = new List<InfoContribution>(); // sin liquidez
            var infoContributionPayedComplete = new List<InfoContribution>(); // los pagos completosD:\Mis Documentos\GitHub\KreierSolutions\WindowsService\Ks.Batch.Util\Model\Summary\SummaryOut.cs
            var infoContributionIncomplete = new List<InfoContribution>(); // los puchos

            var infoContributionNextQuota = new List<InfoContribution>(); // proximas cuotas 

            var infoLoanNoCash = new List<InfoLoan>(); // sin liquidez
            var infoLoanPayedComplete = new List<InfoLoan>(); // los pagos completos
            var infoLoanIncomplete = new List<InfoLoan>(); // los puchos

            var infoReturnPayment = new List<Info>();//devoluciones

            var infoLoanNextQuota = new List<InfoLoan>(); // proximas cuotas 

            #region SplitList

            Info inLine;
            foreach (var outLine in outData)
            {
                outLine.InfoContribution.BankName = bankName;
                outLine.InfoContribution.Description = "Proceso automática por el sistema ACMR";
                outLine.InfoContribution.AccountNumber = accountNumber;
                outLine.InfoContribution.TransactionNumber = report.ParentKey.ToString();

                inLine = inData.FirstOrDefault(x => x.AdminCode == outLine.AdminCode);
                if (inLine == null)
                {
                    //Caso de prueba 201907-V2-005 - 1
                    #region Sin liquidez en Contribution y Loan

                    #region Sin Liquidez en aportaciones Y acumula NEXT

                    outLine.InfoContribution.StateId = (int)ContributionState.SinLiquidez;
                    outLine.InfoContribution.AmountPayed = 0;
                    infoContributionNoCash.Add(MappingContribution(outLine.InfoContribution));
                    infoContributionNextQuota.Add(BuildNextContributionQuota(MappingContribution(outLine.InfoContribution)));

                    #endregion

                    #region Sin Liquides en Prestamos

                    foreach (var infoLoan in outLine.InfoLoans)
                    {
                        infoLoan.StateId = (int)LoanState.SinLiquidez;
                        infoLoan.BankName = bankName;
                        infoLoan.AccountNumber = accountNumber;
                        infoLoan.TransactionNumber = report.ParentKey.ToString();
                        infoLoan.Description = "No hay informacion enviada desde " + bankName;
                        infoLoanNoCash.Add(MappingLoan(infoLoan));
                        infoLoanNextQuota.Add(BuildNextLoanQuota(MappingLoan(infoLoan)));
                    }

                    #endregion

                    #endregion
                }
                else
                {
                    if (!inLine.IsUnique)
                    {
                        #region Find by Contribution and Loan Total

                        #region contribution

                        if (inLine.TotalContribution == 0)
                        {
                            #region Pago Sin Liquidez (==0)

                            outLine.InfoContribution.StateId = (int)ContributionState.SinLiquidez;
                            outLine.InfoContribution.AmountPayed = inLine.TotalContribution;
                            infoContributionNoCash.Add(MappingContribution(outLine.InfoContribution));
                            infoContributionNextQuota.Add(BuildNextContributionQuota(MappingContribution(outLine.InfoContribution)));

                            #endregion

                            inLine.TotalPayed = inLine.TotalPayed - inLine.TotalContribution;
                        }

                        if (outLine.TotalContribution == inLine.TotalContribution)
                        {
                            #region Pago completo de la aportacion

                            outLine.InfoContribution.StateId = (int)ContributionState.Pagado;
                            outLine.InfoContribution.AmountPayed = inLine.TotalContribution;
                            infoContributionPayedComplete.Add(MappingContribution(outLine.InfoContribution));

                            #endregion

                            inLine.TotalPayed = inLine.TotalPayed - inLine.TotalContribution;
                        }

                        if (outLine.TotalContribution < inLine.TotalContribution)
                        {
                            #region Pago raro en donde lo que se recepciona es mayor a lo que se pide

                            outLine.InfoContribution.StateId = (int)ContributionState.Pagado;
                            outLine.InfoContribution.AmountPayed = outLine.TotalContribution;
                            infoContributionPayedComplete.Add(MappingContribution(outLine.InfoContribution));

                            #endregion

                            inLine.TotalPayed = inLine.TotalPayed - outLine.TotalContribution;
                        }

                        if (outLine.TotalContribution > inLine.TotalContribution && inLine.TotalContribution > 0)
                        {
                            #region Pago por puchos en aportaciones Y Acumula NEXT

                            outLine.InfoContribution.StateId = (int)ContributionState.PagoParcial;
                            outLine.InfoContribution.AmountPayed = inLine.TotalContribution;
                            infoContributionIncomplete.Add(MappingContribution(outLine.InfoContribution));
                            infoContributionNextQuota.Add(BuildNextContributionQuota(MappingContribution(outLine.InfoContribution)));

                            #endregion

                            inLine.TotalPayed = inLine.TotalPayed - inLine.TotalContribution;
                        }


                        #endregion

                        #region loan

                        var outLineOrderded = outLine.InfoLoans.OrderBy(x => x.LoanPaymentId).ToList();
                        foreach (var outInfoLoan in outLineOrderded)
                        {
                            outInfoLoan.BankName = bankName;
                            outInfoLoan.AccountNumber = accountNumber;
                            outInfoLoan.TransactionNumber = report.ParentKey.ToString();

                            if (inLine.TotalLoan == 0)
                            {
                                #region no vino data del cobro

                                outInfoLoan.StateId = (int)LoanState.SinLiquidez;
                                outInfoLoan.Description = "Monto enviado desde " + bankName + " no cubre el pago de la cuota";
                                infoLoanNoCash.Add(MappingLoan(outInfoLoan));
                                infoLoanNextQuota.Add(BuildNextLoanQuota(MappingLoan(outInfoLoan)));

                                #endregion

                                inLine.TotalPayed = inLine.TotalPayed - inLine.TotalLoan;
                                continue;
                            }

                            if (inLine.TotalLoan == outInfoLoan.MonthlyQuota)
                            {
                                #region pago completo

                                outInfoLoan.StateId = (int)LoanState.Pagado;
                                outInfoLoan.Description = "Monto enviado desde " + bankName + " pago completo";
                                outInfoLoan.MonthlyPayed = inLine.TotalLoan;
                                infoLoanPayedComplete.Add(MappingLoan(outInfoLoan));

                                #endregion

                                inLine.TotalPayed = inLine.TotalPayed - outInfoLoan.MonthlyPayed;
                                inLine.TotalLoan = 0;
                                continue;
                            }

                            if (outInfoLoan.MonthlyQuota < inLine.TotalLoan)
                            {
                                #region Pago Compelto por exceso

                                outInfoLoan.StateId = (int)LoanState.Pagado;
                                outInfoLoan.MonthlyPayed = outInfoLoan.MonthlyQuota;
                                outInfoLoan.Description = "Pago realizado correctamente por la interfaz " + bankName;
                                infoLoanPayedComplete.Add(MappingLoan(outInfoLoan));

                                #endregion

                                inLine.TotalPayed = inLine.TotalPayed - outInfoLoan.MonthlyPayed;
                                inLine.TotalLoan = inLine.TotalLoan - outInfoLoan.MonthlyPayed;
                                continue;
                            }

                            if (outInfoLoan.MonthlyQuota > inLine.TotalLoan && inLine.TotalLoan>0)
                            {
                                #region pago parcial

                                outInfoLoan.StateId = (int)LoanState.PagoParcial;
                                outInfoLoan.MonthlyPayed = inLine.TotalLoan;
                                outInfoLoan.Description = "Pago parcial enviada desde " + bankName + ". Se creara una nueva cuota al final";
                                infoLoanIncomplete.Add(MappingLoan(outInfoLoan));
                                infoLoanNextQuota.Add(BuildNextLoanQuota(MappingLoan(outInfoLoan)));

                                #endregion

                                inLine.TotalPayed = inLine.TotalPayed - outInfoLoan.MonthlyPayed;
                                inLine.TotalLoan = 0;
                                continue;
                            }

                        }

                        #endregion

                        #endregion
                    }
                    else
                    {
                        #region Find by TotalPayed

                        if (outLine.TotalContribution == inLine.TotalPayed)
                        {
                            //Caso de prueba 201907-V2-005 - 2
                            #region Pagado completo en aportaciones y sin liquidez en prestamos

                            #region Pago completo de la aportacion

                            outLine.InfoContribution.StateId = (int)ContributionState.Pagado;
                            outLine.InfoContribution.AmountPayed = inLine.TotalPayed;
                            infoContributionPayedComplete.Add(MappingContribution(outLine.InfoContribution));

                            #endregion

                            #region Sin Liquides en Prestamos

                            foreach (var infoLoan in outLine.InfoLoans)
                            {
                                infoLoan.StateId = (int)LoanState.SinLiquidez;
                                infoLoan.BankName = bankName;
                                infoLoan.AccountNumber = accountNumber;
                                infoLoan.TransactionNumber = report.ParentKey.ToString();
                                infoLoan.Description = "Monto enviado desde " + bankName + " no cubre el pago de la cuota";
                                infoLoanNoCash.Add(MappingLoan(infoLoan));
                                infoLoanNextQuota.Add(BuildNextLoanQuota(MappingLoan(infoLoan)));
                            }

                            #endregion

                            #endregion

                            inLine.TotalPayed = 0;
                            continue;
                        }

                        if (outLine.TotalContribution > inLine.TotalPayed)
                        {
                            //Caso de prueba 201907-V2-005 - 3
                            #region Pago por puchos en aportacion  y sin liquidez en prestamos

                            #region Pago por puchos en aportaciones Y Acumula NEXT

                            outLine.InfoContribution.StateId = (int)ContributionState.PagoParcial;
                            outLine.InfoContribution.AmountPayed = inLine.TotalPayed;
                            infoContributionIncomplete.Add(MappingContribution(outLine.InfoContribution));
                            infoContributionNextQuota.Add(BuildNextContributionQuota(MappingContribution(outLine.InfoContribution)));

                            #endregion

                            #region Sin Liquides en Prestamos

                            foreach (var infoLoan in outLine.InfoLoans)
                            {
                                infoLoan.StateId = (int)LoanState.SinLiquidez;
                                infoLoan.BankName = bankName;
                                infoLoan.AccountNumber = accountNumber;
                                infoLoan.TransactionNumber = report.ParentKey.ToString();
                                infoLoan.Description = "Monto enviado desde " + bankName + " no cubre el pago de la cuota";
                                infoLoanNoCash.Add(MappingLoan(infoLoan));
                                infoLoanNextQuota.Add(BuildNextLoanQuota(MappingLoan(infoLoan)));
                            }

                            #endregion

                            #endregion

                            inLine.TotalPayed = 0;
                            continue;
                        }

                        if (outLine.TotalContribution < inLine.TotalPayed)
                        {
                            //Caso de prueba 201907-V2-005 - 3
                            #region Pago total en aportacion y en puchos en prestamos

                            #region Pago total en aportacion

                            outLine.InfoContribution.StateId = (int)ContributionState.Pagado;
                            outLine.InfoContribution.AmountPayed = outLine.InfoContribution.AmountTotal;
                            infoContributionPayedComplete.Add(MappingContribution(outLine.InfoContribution));

                            #endregion

                            inLine.TotalPayed -= outLine.InfoContribution.AmountTotal;
                            var outLineOrderdedById = outLine.InfoLoans.OrderBy(x => x.LoanPaymentId).ToList();

                            foreach (var infoLoan in outLineOrderdedById)
                            {
                                infoLoan.BankName = bankName;
                                infoLoan.AccountNumber = accountNumber;
                                infoLoan.TransactionNumber = report.ParentKey.ToString();

                                if (inLine.TotalPayed == 0)
                                {
                                    //Este caso no podria darse debido a que se valido en
                                    //Caso de prueba 201907-V2-005 - 2
                                    #region No hay liquidez para el APOYO

                                    infoLoan.StateId = (int)LoanState.SinLiquidez;
                                    infoLoan.MonthlyPayed = 0;
                                    infoLoan.Description = "Monto enviado desde " + bankName + " no cubre el pago de la cuota";

                                    infoLoanNoCash.Add(MappingLoan(infoLoan));
                                    infoLoanNextQuota.Add(BuildNextLoanQuota(MappingLoan(infoLoan)));
                                    continue;

                                    #endregion
                                }

                                if (infoLoan.MonthlyQuota > inLine.TotalPayed)
                                {
                                    //Caso de prueba 201907-V2-005 - 4
                                    #region Pago Parcial del prestamo

                                    infoLoan.StateId = (int)LoanState.PagoParcial;
                                    infoLoan.MonthlyPayed = inLine.TotalPayed;
                                    infoLoan.Description = "Pago parcial enviada desde " + bankName + ". Se creara una nueva cuota al final";
                                    infoLoanIncomplete.Add(MappingLoan(infoLoan));
                                    infoLoanNextQuota.Add(BuildNextLoanQuota(MappingLoan(infoLoan)));

                                    inLine.TotalPayed = 0;
                                    continue;

                                    #endregion
                                    //Caso de prueba 201907-V2-005 - 6
                                }

                                if (infoLoan.MonthlyQuota <= inLine.TotalPayed)
                                {
                                    //Caso de prueba 201907-V2-005 - 5
                                    #region Pago completo en prestamo

                                    infoLoan.StateId = (int)LoanState.Pagado;
                                    infoLoan.MonthlyPayed = infoLoan.MonthlyQuota;
                                    infoLoan.Description = "Pago realizado correctamente por la interfaz " + bankName;
                                    infoLoanPayedComplete.Add(MappingLoan(infoLoan));

                                    #endregion
                                    //Caso de prueba 201907-V2-005 - 7

                                    inLine.TotalPayed -= infoLoan.MonthlyQuota;
                                    continue;
                                }
                            }

                            #endregion
                        }

                        #endregion
                    }

                    #region Return Payment
                    if (inLine.TotalPayed > 0)
                    {
                        inLine.CustomerId = outLine.CustomerId;
                        infoReturnPayment.Add(inLine);
                    }
                    #endregion
                }
            }

            #endregion

            var result = new Dictionary<string, object>
            {
                { "infoContributionNoCash", infoContributionNoCash },
                { "infoContributionPayedComplete", infoContributionPayedComplete },
                { "infoContributionIncomplete", infoContributionIncomplete },
                { "infoContributionNextQuota", infoContributionNextQuota },
                { "infoLoanNoCash", infoLoanNoCash },
                { "infoLoanPayedComplete", infoLoanPayedComplete },
                { "infoLoanIncomplete", infoLoanIncomplete },
                { "infoLoanNextQuota", infoLoanNextQuota },
                { "infoReturnPayment",infoReturnPayment}
            };

            return result; 
        }

        public void UpdateDateBaseWithResult(Report report,IDictionary<string, object> data)
        {
            var infoContributionNoCash = (List<InfoContribution>)data["infoContributionNoCash"]; // sin liquidez
            var infoContributionPayedComplete = (List<InfoContribution>)data["infoContributionPayedComplete"]; // los pagos completosD:\Mis Documentos\GitHub\KreierSolutions\WindowsService\Ks.Batch.Util\Model\Summary\SummaryOut.cs
            var infoContributionIncomplete = (List<InfoContribution>)data["infoContributionIncomplete"]; // los puchos

            var infoContributionNextQuota = (List<InfoContribution>)data["infoContributionNextQuota"]; // proximas cuotas 

            var infoLoanNoCash = (List<InfoLoan>)data["infoLoanNoCash"]; // sin liquidez
            var infoLoanPayedComplete = (List<InfoLoan>)data["infoLoanPayedComplete"];// los pagos completos
            var infoLoanIncomplete = (List<InfoLoan>)data["infoLoanIncomplete"];// los puchos

            var infoReturnPayment = (List<Info>)data["infoReturnPayment"];// devoluciones

            var infoLoanNextQuota = (List<InfoLoan>)data["infoLoanNextQuota"]; // proximas cuotas 

            var source = "";
            var value = "";

            #region ContributionPayment

            if (infoContributionPayedComplete.Count > 0)
            {
                source = ServiceSetting.ContributionPayedComplete;
                ServiceSetting.SummaryMerge.FileContributionPayedCompleteTotal = infoContributionPayedComplete.Count;

                value = XmlHelper.Serialize2String(new List<InfoContribution>(infoContributionPayedComplete));
                AddPreReport(report, value, source);

                if (!ServiceSetting.IsPre)
                {
                    var infoPartial = LinqExtensions.Split(infoContributionPayedComplete, 50);
                    foreach (var info in infoPartial)
                        UpdateContributionPayment(info.ToList());
                }
            }

            if (infoContributionIncomplete.Count > 0)
            {
                source = ServiceSetting.ContributionIncomplete;
                ServiceSetting.SummaryMerge.FileContributionIncompleteTotal = infoContributionIncomplete.Count;

                value = XmlHelper.Serialize2String(new List<InfoContribution>(infoContributionIncomplete));
                AddPreReport(report, value, source);

                if (!ServiceSetting.IsPre)
                {
                    var infoPartial = LinqExtensions.Split(infoContributionIncomplete, 50);
                    foreach (var info in infoPartial)
                        UpdateContributionPayment(info.ToList());
                }
            }

            if (infoContributionNoCash.Count > 0)
            {
                source = ServiceSetting.ContributionNoCash;
                ServiceSetting.SummaryMerge.FileContributionNoCashTotal = infoContributionNoCash.Count;

                value = XmlHelper.Serialize2String(new List<InfoContribution>(infoContributionNoCash));
                AddPreReport(report, value, source);

                if (!ServiceSetting.IsPre)
                {
                    var infoPartial = LinqExtensions.Split(infoContributionNoCash, 50);
                    foreach (var info in infoPartial)
                        UpdateContributionPayment(info.ToList());
                }
            }

            //if (infoContributionNextQuota.Count > 0)
            //{
            //    source = ServiceSetting.ContributionNextQuota;
            //    value = XmlHelper.Serialize2String(new List<InfoContribution>(infoContributionNextQuota));
            //    AddPreReport(report, value, source);

            //    if (!ServiceSetting.IsPre)
            //    {
            //        UpdateNextContributionPayment(infoContributionNextQuota);
            //    }
            //}

            #endregion

            #region LoanPayment

            if (infoLoanPayedComplete.Count > 0)
            {
                source = ServiceSetting.LoanPayedComplete;
                ServiceSetting.SummaryMerge.DataBaseLoanPayedCompleteTotal = infoLoanPayedComplete.Count;

                value = XmlHelper.Serialize2String(new List<InfoLoan>(infoLoanPayedComplete));
                AddPreReport(report, value, source);
                if (!ServiceSetting.IsPre)
                {
                    var infoPartial = LinqExtensions.Split(infoLoanPayedComplete, 50);
                    foreach (var info in infoPartial)
                        UpdateLoanPayment(info.ToList());
                }
            }

            if (infoLoanIncomplete.Count > 0)
            {
                source = ServiceSetting.LoanIncomplete;
                ServiceSetting.SummaryMerge.DataBaseLoanIncompleteTotal = infoLoanIncomplete.Count;

                value = XmlHelper.Serialize2String(new List<InfoLoan>(infoLoanIncomplete));
                AddPreReport(report, value, source);

                if (!ServiceSetting.IsPre)
                {
                    var infoPartial = LinqExtensions.Split(infoLoanIncomplete, 50);
                    foreach (var info in infoPartial)
                        UpdateLoanPayment(info.ToList());
                }
            }

            if (infoLoanNoCash.Count > 0)
            {
                source = ServiceSetting.LoanNoCash;
                ServiceSetting.SummaryMerge.DataBaseLoanNoCashTotal = infoLoanNoCash.Count;

                value = XmlHelper.Serialize2String(new List<InfoLoan>(infoLoanNoCash));
                AddPreReport(report, value, source);

                if (!ServiceSetting.IsPre)
                {
                    var infoPartial = LinqExtensions.Split(infoLoanNoCash, 50);
                    foreach (var info in infoPartial)
                        UpdateLoanPayment(info.ToList());
                }
            }

            if (infoLoanNextQuota.Count > 0)
            {
                source = ServiceSetting.LoanNextQuota;
                ServiceSetting.SummaryMerge.DataBaseLoanNextQuota = infoLoanNextQuota.Count;

                value = XmlHelper.Serialize2String(new List<InfoLoan>(infoLoanNextQuota));
                AddPreReport(report, value, source);

                if (!ServiceSetting.IsPre)
                {
                    UpdateNextLoanPayment(infoLoanNextQuota);
                }
            }

            #endregion

            #region ReturnPayment

            if (!ServiceSetting.IsPre)
            {
                foreach (var inLine in infoReturnPayment)
                {
                    ReturnPayment(inLine);
                } 
            }
                

            #endregion

            //si no es simulacion entonces si se cierra el flujo, caso contrario ya se actualizo en el pasado
            if (!ServiceSetting.IsPre)
            {
                CloseReport(report);
                CreateReportSummaryUpLoad(report, "SummaryMerge", XmlHelper.Serialize2String(ServiceSetting.SummaryMerge));
            }
        }
        #endregion

        #region Utilities

        #region Contribution + Loan + Next

        private void UpdateContributionPayment(List<InfoContribution> info)
        {
            try
            {
                Log.InfoFormat("Action: {0}", "Dao.UpdateContributionPayment(" + string.Join(",", info.Select(x => x.ContributionPaymentId)) + ")");

                var xml = XmlHelper.Serialize2String(info, true);
                xml = xml.Replace('\n', ' ');
                xml = xml.Replace('\r', ' ');
                xml = xml.Replace("<?xml version=\"1.0\"?>", "");

                Sql = "UpdateContributionPayment @XmlPackage";
                Command = new SqlCommand(Sql, Connection);
                Command.CommandTimeout = 1800;
                Command.Parameters.AddWithValue("@XmlPackage", xml);
                Command.ExecuteNonQuery();

                Log.InfoFormat("Action: {0}", "Dao.UpdateContributionPayment " + info.Count() + " actualizados correctamente ");
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "Dao.UpdateContributionPayment(" + string.Join(",", info.Select(x => x.ContributionPaymentId)) + ")", ex.Message);
            }
        }

        private void UpdateNextContributionPayment(List<InfoContribution> info)
        {
            try
            {
                Log.InfoFormat("Action: {0}", "Dao.UpdateNextContributionPayment(" + string.Join(",", info.Select(x => x.ContributionPaymentId)) + ")");

                var xml = XmlHelper.Serialize2String(info, true);
                xml = xml.Replace('\n', ' ');
                xml = xml.Replace('\r', ' ');
                xml = xml.Replace("<?xml version=\"1.0\"?>", "");

                Sql = "UpdateNextContributionPayment @XmlPackage";
                Command = new SqlCommand(Sql, Connection);
                Command.CommandTimeout = 1800;
                Command.Parameters.AddWithValue("@XmlPackage", xml);
                Command.ExecuteNonQuery();

                Log.InfoFormat("Action: {0}", "Dao.UpdateNextContributionPayment " + info.Count() + " actualizados correctamente ");
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "Dao.UpdateNextContributionPayment(" + string.Join(",", info.Select(x => x.ContributionPaymentId)) + ")", ex.Message);
            }
        }

        private void UpdateLoanPayment(List<InfoLoan> info)
        {
            try
            {
                Log.InfoFormat("Action: {0}", "Dao.UpdateLoanPayment(" + string.Join(",", info.Select(x => x.LoanPaymentId)));

                var xml = XmlHelper.Serialize2String(info, true);
                xml = xml.Replace('\n', ' ');
                xml = xml.Replace('\r', ' ');
                xml = xml.Replace("<?xml version=\"1.0\"?>", "");
                  
                Sql = "UpdateLoanPayment @XmlPackage ";
                Command = new SqlCommand(Sql, Connection);
                Command.CommandTimeout = 1800;
                Command.Parameters.AddWithValue("@XmlPackage", xml);
                Command.ExecuteNonQuery();

                Log.InfoFormat("Action: {0}", "Dao.UpdateLoanPayment " + info.Count() + " actualizados correctamente ");
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "Dao.UpdateLoanPayment(" + string.Join(",", info.Select(x => x.LoanPaymentId)) + ")", ex.Message);
            }
        }

        private void UpdateNextLoanPayment(List<InfoLoan> info)
        {
            try
            {
                Log.InfoFormat("Action: {0}", "Dao.UpdateNextLoanPayment(" + string.Join(",", info.Select(x => x.LoanPaymentId)) + ")");

                var xml = XmlHelper.Serialize2String(info, true);
                xml = xml.Replace('\n', ' ');
                xml = xml.Replace('\r', ' ');
                xml = xml.Replace("<?xml version=\"1.0\"?>", "");

                Sql = "UpdateNextLoanPayment @XmlPackage ";
                Command = new SqlCommand(Sql, Connection);
                Command.CommandTimeout = 1800;
                Command.Parameters.AddWithValue("@XmlPackage", xml);
                Command.ExecuteNonQuery();

                Log.InfoFormat("Action: {0}", "Dao.UpdateNextLoanPayment " + info.Count() + " actualizados correctamente ");
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "Dao.UpdateNextLoanPayment(" + string.Join(",", info.Select(x => x.LoanPaymentId)) + ")", ex.Message);
            }
        }

        #endregion

        public void ReturnPayment(Info info)
        {
            var returnPayment = new ReturnPayment
            {
                AmountToPay = info.TotalPayed,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                PaymentNumber = -1,
                StateId = (int)ReturnPaymentState.Creado,
                ReturnPaymentTypeId = (int)ReturnPaymentType.Excedente,
                CustomerId = info.CustomerId
            };

            var workFlow = new WorkFlow
            {
                CustomerCreatedId = 4,
                EntityId = 0,
                EntityName = "Ks.Core.Domain.Contract.ReturnPayment",
                RequireCustomer = false,
                RequireSystemRole = true,
                SystemRoleApproval = "Employee",
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                Active = true,
                Title = "Nueva Devolución",
                Description = "Se requiere una revision para una devolucion creada a partir de un exceso registrado el dia " +
                                    DateTime.Today.ToShortDateString() +
                                    " durante la carga de la interfaz proveniente de " + info.InfoContribution.BankName,
                GoTo = "Admin/ReturnPayment/Edit/ReturnPaymentId"
            };

            CreateReturnPayment(returnPayment, workFlow);

        }

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

        private InfoContribution BuildNextContributionQuota(InfoContribution infoContribution)
        {
            //Si viene de una sin liquidez entonces el valor de infoContribution.AmountPayed =  0
            //de lo contrario es de un pago parcial y infoContribution.AmountPayed es lo que pago por ende hay la resta 
            //de lo que debio pagar menos lo que pago y resulta lo que debe como una cuota antigua acumulada en el prox pago
            infoContribution.AmountOld = infoContribution.AmountTotal - infoContribution.AmountPayed;
            infoContribution.NumberOld = infoContribution.Number;

            infoContribution.Description = "Cuota aumentada debido a la cuota Nro " +
                                            infoContribution.Number.ToString("D3") +
                                            " debido a " +
                                            (infoContribution.StateId == (int)ContributionState.SinLiquidez ? "Sin Liquidez" : "Cobro Parcial");
            //Monto total se calcula en SP
            infoContribution.Number++;
            infoContribution.StateId = (int)ContributionState.Pendiente;
            return infoContribution;
        }

        private InfoLoan BuildNextLoanQuota(InfoLoan infoLoan)
        {
            infoLoan.StateId = (int)LoanState.Pendiente;
            infoLoan.IsAutomatic = true;

            // Si viene de una sin liquidez entonces el valor de infoLoan.MonthlyPayed = 0
            if (infoLoan.MonthlyPayed == 0)
            {
                infoLoan.Description = "Cuota generada automaticamente por falta de liquidez en la couta Nº " + infoLoan.Quota;

            }
            if (infoLoan.MonthlyPayed > 0)
            {
                //viene de un pago parcial 
                infoLoan.Description = "Cuota generada automaticamente por pago parcial en la couta Nº " + infoLoan.Quota;
                var ratio = 1 - (infoLoan.MonthlyPayed / infoLoan.MonthlyQuota);
                infoLoan.MonthlyQuota = Math.Round(infoLoan.MonthlyQuota * ratio, 2);
                infoLoan.MonthlyFee = Math.Round(infoLoan.MonthlyFee * ratio, 2);
                infoLoan.MonthlyCapital = Math.Round(infoLoan.MonthlyCapital * ratio, 2);
                infoLoan.MonthlyPayed = 0;

            }
            else
            {
                //esto es de una devolucion porque viene un valor negativo
                //TODO

            }
            return infoLoan;
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
