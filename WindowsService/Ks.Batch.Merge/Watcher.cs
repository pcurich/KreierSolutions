using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using Ks.Batch.Util;
using Ks.Batch.Util.Model;
using Ks.Batch.Util.Model.Summary;

namespace Ks.Batch.Merge
{
    public class Watcher
    {
        private static List<Info> _copereOut;
        private static List<Info> _copereIn;
        private static List<Info> _cajaOut;
        private static List<Info> _cajaIn;
        private static Report _reportCopere;
        private static Report _reportCaja;

        private static ScheduleBatch Batch { get; set; }
        private static ServiceSetting ServiceSetting;

        public static void FileCreated(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(1000 * 3); //3 Sec because is not atomic

            ReadServiceSetting(e.Name);
            ReadBatchService();

            LoadDataFromReport();

            var dao = new Dao(ServiceSetting.Connection);
            dao.Connect();
            dao.ServiceSetting = ServiceSetting;

            var account = Batch.SystemName + "." + Batch.PeriodYear + "." + Batch.PeriodMonth.ToString("D2");

            if (_copereOut != null && _copereIn != null && _copereOut.Count > 0 && _copereIn.Count > 0 && e.Name.ToUpper().Contains("COPERE"))
            {
                Batch.PeriodYear = int.Parse(_reportCopere.Period.Substring(0, 4));
                Batch.PeriodMonth = int.Parse(_reportCopere.Period.Substring(4, 2));
                var resultList = dao.Process(_reportCopere, _copereIn, _copereOut, "Copere", account);
                dao.UpdateDateBaseWithResult(_reportCopere, resultList);
            }
            if (_cajaOut != null && _cajaIn != null && _cajaOut.Count > 0 && _cajaIn.Count > 0 && e.Name.ToUpper().Contains("CAJA"))
            {
                Batch.PeriodYear = int.Parse(_reportCaja.Period.Substring(0, 4));
                Batch.PeriodMonth = int.Parse(_reportCaja.Period.Substring(4, 2));
                var resultList = dao.Process(_reportCaja, _cajaIn, _cajaOut, "Caja", account);
                dao.UpdateDateBaseWithResult(_reportCaja, resultList);
            }

            #region  update working end
            Batch.Enabled = false;
            Batch.LastExecutionOnUtc = DateTime.UtcNow;
            dao.UpdateScheduleBatch(Batch);
            #endregion

            dao.Close();
            File.Delete(e.FullPath);
        }

        #region Util

        protected static void SplitList(Dictionary<Report, List<Info>> listData)
        {
            foreach (var data in listData)
            {
                switch (data.Key.Source)
                {
                    case "Ks.Batch.Copere.Out":
                        {
                            _copereOut = data.Value;
                            _reportCopere = data.Key;
                            break;
                        }
                    case "Ks.Batch.Copere.In":
                        {
                            _copereIn = data.Value;
                            _reportCopere = data.Key;
                            break;
                        }
                    case "Ks.Batch.Caja.Out":
                        {
                            _cajaOut = data.Value;
                            _reportCaja = data.Key;
                            break;
                        }
                    case "Ks.Batch.Caja.In":
                        {
                            _cajaIn = data.Value;
                            _reportCaja = data.Key;
                            break;
                        }
                }

            }

            ServiceSetting.SummaryMerge.FileContributionTotal = _copereIn != null ? _copereIn.Count(c => c.TotalContribution > 0) : _cajaIn.Count(c => c.TotalContribution > 0);
            ServiceSetting.SummaryMerge.FileContributionAmount = _copereIn != null ? _copereIn.Sum(c => c.TotalContribution) : _cajaIn.Sum(c => c.TotalContribution);
            ServiceSetting.SummaryMerge.FileLoanTotal = _copereIn != null ? _copereIn.Count(c => c.TotalLoan > 0) : _cajaIn.Count(c => c.TotalLoan > 0);
            ServiceSetting.SummaryMerge.FileLoanAmount = _copereIn != null ? _copereIn.Sum(c => c.TotalLoan) : _cajaIn.Sum(c => c.TotalLoan);
        }

        private static void ReadServiceSetting(string fileName)
        {
            ServiceSetting = new ServiceSetting
            {
                SummaryMerge = new SummaryMerge(),
                Path = ConfigurationManager.AppSettings["Path"],
                DefaultCulture = ConfigurationManager.AppSettings["DefaultCulture"],
                Connection = ConfigurationManager.ConnectionStrings["ACMR"].ConnectionString,
                SysName = ConfigurationManager.AppSettings["SysName"],
                ContributionPayedComplete = ConfigurationManager.AppSettings["ContributionPayedComplete"],
                ContributionIncomplete = ConfigurationManager.AppSettings["ContributionIncomplete"],
                ContributionNoCash = ConfigurationManager.AppSettings["ContributionNoCash"],
                ContributionNextQuota = ConfigurationManager.AppSettings["ContributionNextQuota"],
                LoanPayedComplete = ConfigurationManager.AppSettings["LoanPayedComplete"],
                LoanIncomplete = ConfigurationManager.AppSettings["LoanIncomplete"],
                LoanNoCash = ConfigurationManager.AppSettings["LoanNoCash"],
                LoanNextQuota = ConfigurationManager.AppSettings["LoanNextQuota"],
                IsPre = fileName.ToUpper().Contains("PRE") ? true : false
            }; 
 
        }

        private static void ReadBatchService()
        {
            var dao = new Dao(ServiceSetting.Connection);
            dao.Connect();
            Batch = dao.GetScheduleBatch(ServiceSetting.SysName);
            Batch.Enabled = true;
            Batch.LastExecutionOnUtc = DateTime.UtcNow;
            dao.UpdateScheduleBatch(Batch);
            dao.Close();
             
        }

        private static void LoadDataFromReport()
        {
            var dao = new Dao(ServiceSetting.Connection);
            dao.Connect();
            //Data to calculate
            var listData = dao.GetData();

            //load _copereOut or _copereIn or _cajaOut or _cajaIn
            if (listData.Count == 2)
                SplitList(listData);

            dao.Close();
        }

        #endregion
    }
}