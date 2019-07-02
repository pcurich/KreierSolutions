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
    public class Dao : DaoBase
    {
        private new static readonly LogWriter Log = HostLogger.Get<Dao>();
        private List<Info> _listInfo = new List<Info>();


        public Dao(string connetionString)
            : base(connetionString)
        {
        }

        public void RevertDataContribution(int periodYear, int periodMonth)
        {
            try
            {
                Sql = "UPDATE ContributionPayment SET StateId = " + (int)ContributionState.Pendiente + " WHERE ID IN ( " +
                  " SELECT  cp.Id " +
                  " FROM ContributionPayment cp " +
                  " INNER JOIN  Contribution c on c.Id=cp.ContributionId " +
                  " WHERE cp.StateId=@StateId AND " +
                  " c.Active="+(int)State.Active+" "+
                  " YEAR(cp.ScheduledDateOnUtc)=@Year AND  " +
                  " MONTH(cp.ScheduledDateOnUtc)=@Month  ) ";

                Command = new SqlCommand(Sql, Connection);

                var pStateId = new SqlParameter
                {
                    ParameterName = "@StateId",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input,
                    Value = (int)ContributionState.EnProceso
                };

                var pYear = new SqlParameter
                {
                    ParameterName = "@Year",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input,
                    Value = periodYear
                };
                var pMonth = new SqlParameter
                {
                    ParameterName = "@Month",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input,
                    Value = periodMonth
                };

                Command.Parameters.Add(pStateId);
                Command.Parameters.Add(pYear);
                Command.Parameters.Add(pMonth);
                Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "Dao.RevertDataContribution("+periodYear+" - "+periodMonth+")", ex.Message);
            }
        }

        public void RevertDataLoan(int periodYear, int periodMonth)
        {
            try
            {
                Sql = "UPDATE LoanPayment SET StateId = " + (int)LoanState.Pendiente + " WHERE ID IN ( " +
                  " SELECT  lp.Id " +
                  " FROM LoanPayment lp " +
                  " INNER JOIN  Loan l on l.Id=lp.LoanId " +
                  " WHERE lp.StateId=@StateId AND " +
                  " l.Active=" + (int)State.Active + " " +
                  " YEAR(lp.ScheduledDateOnUtc)=@Year AND  " +
                  " MONTH(lp.ScheduledDateOnUtc)=@Month  ) ";

                Command = new SqlCommand(Sql, Connection);

                var pStateId = new SqlParameter
                {
                    ParameterName = "@StateId",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input,
                    Value = (int)LoanState.EnProceso
                };

                var pYear = new SqlParameter
                {
                    ParameterName = "@Year",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input,
                    Value = periodYear
                };
                var pMonth = new SqlParameter
                {
                    ParameterName = "@Month",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input,
                    Value = periodMonth
                };

                Command.Parameters.Add(pStateId);
                Command.Parameters.Add(pYear);
                Command.Parameters.Add(pMonth);
                Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Action: {0} Error: {1}", "Dao.RevertDataLoan(" + periodYear + " - " + periodMonth + ")", ex.Message);
            }
        }

    }
}
