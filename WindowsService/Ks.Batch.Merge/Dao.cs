using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Ks.Batch.Util;
using Ks.Batch.Util.Model;
using Topshelf.Logging;

namespace Ks.Batch.Merge
{
    public class Dao : DaoBase
    {
        private static readonly LogWriter Log = HostLogger.Get<Dao>();
        private List<Info> _listInfo = new List<Info>();


        public Dao(string connetionString)
            : base(connetionString)
        {
        }

        public Dictionary<Report, List<Info>> GetData()
        {
            Log.InfoFormat("Time: {0}: Action: {1}", DateTime.Now, "Ks.Batch.Merge.Dao.GetData()");

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
                        XmlHelper.XmlToObject<List<Info>>(sqlReader.GetString(3)));
                    }
                    sqlReader.Close();
                }
                return infoList;
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Time: {0}: Action: {1}", DateTime.Now, ex.Message);
                Close();
                return null;

            }

        }


        public void Process(Report report, List<Info> listResponse, List<Info> listRequest, string bankName)
        {
            var infoEquals = new List<Info>(); //aountIn == amountOut  Pago completo
            var infoNotIn = new List<Info>(); //aountIn >0 aountOut ==0 No tiene liquidez
            var infoLoss = new List<Info>(); //aountIn >0 aountOut >0 pago por puchos 

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
                    #region Sin Liquidez en aportaciones y en prestamos
                    request.InfoContribution.StateId = (int)ContributionState.SinLiquidez;
                    foreach (var infoLoan in request.InfoLoans)
                        infoLoan.StateId = (int)ContributionState.SinLiquidez;
                     
                    infoNotIn.Add(request);
                    #endregion
                }
                else
                {
                    if (request.TotalContribution == response.TotalPayed)
                    {
                        #region Pagado Total en Aportaciones y sin liquidez en prestamos
                        request.InfoContribution.StateId = (int)ContributionState.Pagado;
                        request.InfoContribution.AmountPayed = response.TotalPayed;
                        foreach (var infoLoan in request.InfoLoans)
                            infoLoan.StateId = (int)ContributionState.SinLiquidez;
                        infoEquals.Add(request);
                        #endregion
                    }
                    if(request.TotalContribution > response.TotalPayed))
                    {
                        #region Pago por puchos en aportacion y sin pago en prestamos
                        request.InfoContribution.StateId = (int)ContributionState.PagoParcial;
                        request.InfoContribution.AmountPayed = response.TotalPayed;
                        foreach (var infoLoan in request.InfoLoans)
                            infoLoan.StateId = (int)ContributionState.SinLiquidez;
                        infoLoss.Add(request);
                        #endregion
                    } 
                    if(request.TotalContribution < response.TotalPayed))
                    {
                        #region Pago por puchos en aportacion y sin pago en prestamos
                        request.InfoContribution.StateId = (int)ContributionState.Pagado;
                        request.InfoContribution.AmountPayed = request.InfoContribution.AmountTotal;
                        response.TotalPayed -= request.InfoContribution.AmountTotal;

                        foreach (var infoLoan in request.InfoLoans)
                        {
                            if (response.TotalPayed >= infoLoan.MonthlyQuota)
                            {
                                infoLoan.StateId = (int)ContributionState.Pagado;
                                infoLoan.MonthlyPayed=infoLoan.MonthlyQuota;
                                response.TotalPayed -= infoLoan.MonthlyQuota;
                            }
                            else
                            {
                                if (response.TotalPayed > 0)
                                {
                                    infoLoan.StateId = (int) ContributionState.PagoParcial;
                                    infoLoan.MonthlyPayed = response.TotalPayed;
                                    response.TotalPayed = 0;
                                }
                                else
                                {
                                    infoLoan.StateId = (int) ContributionState.SinLiquidez;
                                    infoLoan.MonthlyPayed = 0;
                                }
                            }
                        }
                        
                            
                        infoLoss.Add(request);
                        #endregion
                    }
                }
            }

            #endregion

            if (infoEquals.Count > 0)
            {
                UpdateContributionPayment(infoEquals, report.Period);
            }
            if (infoNotIn.Count > 0)
            {
                UpdateContributionPayment(infoNotIn, report.Period);
            }
            if (infoLoss.Count > 0)
            {
                UpdateContributionPayment(infoLoss, report.Period);
            }
            CloseReport(report);
        }


        #region Utilities
        private void UpdateContributionPayment(List<Info> info, string period)
        {
            try
            {
                Log.InfoFormat("Action: {0}", "Dao.UpdateContributionPayment(" + string.Join(",", info.Select(x => x.AdminCode)) + ", " + period + ")");

                var year = period.Substring(0, 4);
                var month = period.Substring(4, 2);
                var xml = XmlHelper.Serialize2String(info, true);
                xml = xml.Replace('\n', ' ');
                xml = xml.Replace('\r', ' ');
                xml = xml.Replace("<?xml version=\"1.0\"?>", "");

                Sql = "UpdateContributionPayment @XmlPackage,@Year, @Month";
                Command = new SqlCommand(Sql, Connection);
                Command.Parameters.AddWithValue("@XmlPackage", xml);
                Command.Parameters.AddWithValue("@Year", Convert.ToInt32(year));
                Command.Parameters.AddWithValue("@Month", Convert.ToInt32(month));
                Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "Dao.UpdateContributionPayment(" + string.Join(",", info.Select(x => x.AdminCode)) + ", " + period + ")", ex.Message);
            }
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

        #endregion

    }
}