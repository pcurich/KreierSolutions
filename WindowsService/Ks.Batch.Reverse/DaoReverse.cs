using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Ks.Batch.Util;
using Ks.Batch.Util.Model;
using Topshelf.Logging;

namespace Ks.Batch.Reverse
{
    public class DaoReverse : DaoBase
    {
        private new static readonly LogWriter Log = HostLogger.Get<DaoReverse>();
        private List<Info> _listInfo = new List<Info>();
        private Dictionary<int, string> FileOut { get; set; }

        private ScheduleBatch _batch;

        public DaoReverse(string connetionString)
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

                Log.InfoFormat("Action: Datos encontrados {0}", infoList.Count());

                var contributionPaymentIds= (from x in infoList select x).Select(a => a.InfoContribution.ContributionPaymentId).ToList();
                var loanIds = (from x in infoList select x.InfoLoans);
                var loanPaymentIds = new List<int>();

                foreach (var lp in loanIds)
                {
                    loanPaymentIds.AddRange(lp.Select(x => x.LoanPaymentId));
                }
                
                if (contributionPaymentIds.Count>0 && loanPaymentIds.Count > 0)
                {
                    Log.InfoFormat("Action: Aportacions revertidas {0}", contributionPaymentIds.Count());
                    result = RevertDataContribution(string.Join(",", contributionPaymentIds.ToArray())) && result;
                    Log.InfoFormat("Action: Aportacions revertidas Status {0}", result);

                    Log.InfoFormat("Action: Apoyos revertidos {0}", loanPaymentIds.Count());
                    result = RevertDataLoan(string.Join(",", loanPaymentIds.ToArray())) && result;
                    Log.InfoFormat("Action: Apoyos revertidos Status {0}", result);
                }

            }
            catch(Exception )
            {
                return result;
            }
            return result;
        }   

        private bool RevertDataContribution(string ids)
        {
            try
            {
                Log.InfoFormat("Action: Inicio de RevertDataContribution");

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

                Log.InfoFormat("Action: Fin de RevertDataContribution {0}", ids);
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
                Log.InfoFormat("Action: Inicio de RevertDataLoan");
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
                Log.InfoFormat("Action: Fin de RevertDataLoan {0}", ids);
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
