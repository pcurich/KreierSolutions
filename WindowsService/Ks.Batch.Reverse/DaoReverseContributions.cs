using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Ks.Batch.Util;
using Ks.Batch.Util.Model;
using Topshelf.Logging;

namespace Ks.Batch.Reverse
{
    public class DaoReverseContributions : DaoBase
    {
        private new static readonly LogWriter Log = HostLogger.Get<DaoReverseContributions>();
        private List<Info> _listInfo = new List<Info>();
        private List<int> _customerIds;
        private Dictionary<int, Info> ReportOut { get; set; }
        private Dictionary<int, string> FileOut { get; set; }

        private ScheduleBatch _batch;

        public DaoReverseContributions(string connetionString)
            : base(connetionString)
        {
        }

        public bool StartReverse(int code,   ScheduleBatch batch)
        {
            var result = true;

            try
            {
                _batch = batch;
                

                //Si es OUT el estado es 2 y el IN esta en 1

                var infoList = FindReport(_batch.SystemName, _batch.PeriodYear, _batch.PeriodMonth, 
                    _batch.SystemName.ToUpper().Contains("OUT") ? (int)ReportState.InProcess : (int)ReportState.Waiting);                

                var contributionPaymentIds= (from x in infoList select x).Select(a => a.InfoContribution.ContributionPaymentId).ToList();
                var loanIds = (from x in infoList select x.InfoLoans);
                var loanPaymentIds = new List<int>();

                foreach (var lp in loanIds)
                {
                    loanPaymentIds.AddRange(lp.Select(x => x.LoanPaymentId));
                }

                if(contributionPaymentIds.Count>0 && loanPaymentIds.Count > 0)
                {
                    result = RevertDataContribution(string.Join(",", contributionPaymentIds.ToArray())) && result;
                    result = RevertDataLoan(string.Join(",", loanPaymentIds.ToArray())) && result;
                }

            }
            catch(Exception e)
            {
                return result;
            }
            return result;
        }   

        private bool RevertDataContribution(string ids)
        {
            try
            {
                Sql = " UPDATE ContributionPayment " +
                      " SET StateId = " + (int)ContributionState.Pendiente + " , "+
                      " AmountPayed = 0.0, " +
                      " BankName = '', " +
                      " AccountNumber = '', " +
                      " TransactionNumber = '', " +
                      " Reference = '', " +
                      " Description = '', " +
                      " ProcessedDateOnUtc = null " +
                      " WHERE ID IN " +
                      " ( " + ids + ")";

                Command = new SqlCommand(Sql, Connection);
                Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action:  Error: {0}", ex.Message);
                return false;
            }
            return true;
        }

        private bool RevertDataLoan(string ids)
        {
            try
            {
                Sql = " UPDATE LoanPayment " +
                      " SET StateId = " + (int)LoanState.Pendiente + ", "+
                      " MonthlyPayed = 0.0, "+
                      " BankName = '', " +
                      " AccountNumber = '', " +
                      " TransactionNumber = '', " +
                      " Reference = '', " +
                      " Description = '', " +
                      " ProcessedDateOnUtc = null " +
                      " WHERE ID IN " +
                      " ( " + ids + ")";

                Command = new SqlCommand(Sql, Connection);
                Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action:  Error: {0}", ex.Message);
                return false;
            }
            return true;
        }

    }
}
