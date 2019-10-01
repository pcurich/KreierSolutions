using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Ks.Batch.Util;
using Ks.Batch.Util.Model;

namespace Ks.Batch.Caja.Out
{
    public class Dao : DaoBase
    {
        private const int CODE = (int)CustomerMilitarySituation.Retiro;

        private Dictionary<int, Info> ReportOut { get; set; }
        private Dictionary<int, string> FileOut { get; set; }
        private List<string> Result { get; set; }
        private ScheduleBatch Batch { get; set; }

        public Dao(string connetionString) : base(connetionString){}

        public List<string> Process(string path,ScheduleBatch batch)
        {
            Batch = batch;
            try
            {
                CultureInfo culture = new CultureInfo("es-PE");
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;

                GetCustomer(out List<int> customerIds);

                GetContributionPayments(customerIds);

                #region Write to xml file
                using (var stream = new MemoryStream())
                {
                    try
                    {
                        var nameFile = Path.Combine(Path.Combine(path, Batch.FolderMoveToDone),string.Format("6008_{0}00_APORT.xlsx", Batch.PeriodYear.ToString("0000") + Batch.PeriodMonth.ToString("00")));
                        Log.InfoFormat("Action: Creamos el archivo {0}", nameFile);

                        var properties = new[] { "MesProc", "CodDes", "NumAdm", "Monto", "FechaDesem", "HoraDesem", "NroCuota", "TotalCuotas", "Saldo" };
                        var data = new Dictionary<int, Dictionary<int, string>>();
                        var index = 0;
                        foreach (var info in ReportOut.Values)
                        {
                            if (info.InfoContribution != null)
                            {
                                var tmp = new Dictionary<int, string>
                                {
                                    { 0, info.Year.ToString() + info.Month.ToString("D2") },
                                    { 1, "APORT" },
                                    { 2, info.AdminCode },
                                    { 3, info.InfoContribution.AmountTotal.ToString() }
                                };
                                data.Add(index, tmp);
                                index++;
                            }
                            else
                            {
                                Log.ErrorFormat("Action: Parar revisar : {0}", info.AdminCode);
                            }
                        }
                        if (File.Exists(nameFile))
                            File.Delete(nameFile);
                        ExcelFile.CreateReport("Aportaciones", 1, stream, properties, data, nameFile);

                    }catch(Exception ex)
                    {
                        Log.FatalFormat("Action: Error al crear el archivo : {0}", ex.InnerException);
                    }
                    
                }
                #endregion

                GetLoanPayment(customerIds);

                #region Write to xml file
                using (var stream = new MemoryStream())
                {
                    try
                    {
                        var nameFile = Path.Combine(Path.Combine(path, Batch.FolderMoveToDone),string.Format("6008_{0}00_ASE.xlsx", Batch.PeriodYear.ToString("0000") + Batch.PeriodMonth.ToString("00")));
                        Log.InfoFormat("Action: Creamos el archivo {0}", nameFile);

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
                                    { 2, "ASE" },
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
                        
                        if (File.Exists(nameFile))
                            File.Delete(nameFile);
                        ExcelFile.CreateReport("Apoyo", 1, stream, properties, data, nameFile);
                    }
                    catch (Exception ex)
                    {
                        Log.FatalFormat("Action: Error al crear el archivo : {0}", ex.InnerException);
                    }

                }
                #endregion

                MergeData(customerIds);

                if (FileOut.Count != 0)
                {
                    DeleteReport(Batch.PeriodYear.ToString("0000") + Batch.PeriodMonth.ToString("00"), Batch.SystemName);
                    DeleteReport(Batch.PeriodYear.ToString("0000") + Batch.PeriodMonth.ToString("00"), "Ks.Batch.Caja.In");
                    CompleteCustomerName();
                    var guid = CreateReportIn(Batch, XmlHelper.Serialize2String(new List<Info>(ReportOut.Values)));
                    CreateReportOut(guid, Batch.PeriodYear.ToString("0000") + Batch.PeriodMonth.ToString("00"),"Ks.Batch.Caja.In");
                }
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "Dao.Process(" + batch.SystemName + ")", ex.InnerException);
            }

            return Result != null ? Result.ToList() : null;
        }

        #region Process

        private void GetCustomer(out List<int> customerIds)
        {
            customerIds = new List<int>();
            GetCustomer(CODE, out customerIds, out Dictionary<int, Info> tReportOut, fileOut: out Dictionary<int, string> tFileOut);
            ReportOut = tReportOut;
            FileOut = tFileOut;
        }

        private void GetContributionPayments(List<int> customerIds)
        {
            GetContributionPayments(customerIds, Batch.PeriodYear, Batch.PeriodMonth, Batch.UpdateData, ReportOut, state: (int)ContributionState.Pendiente);
        } 
        
        private void GetLoanPayment(List<int> customerIds)
        {
            GetLoanPayment(customerIds, Batch.PeriodYear, Batch.PeriodMonth, Batch.UpdateData, ReportOut, state: (int)LoanState.Pendiente);
        }

        #endregion

        #region Util

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

            
            
            var contributions = (from customerId in customerIds2 where FileOut.ContainsKey(customerId) 
                                 let total = ReportOut[customerId].TotalContribution + ReportOut[customerId].TotalLoan 
                                 select 
                                 string.Format("{0}  {1}{2}{3}", 
                                 FileOut[customerId], 
                                 Batch.PeriodYear, 
                                 Batch.PeriodMonth.ToString("00"), 
                                 (total.ToString("n")).PadLeft(10, '0'))).ToList();

            Result = new List<string>();

            foreach (var contribution in contributions)
                Result.Add(contribution);
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
