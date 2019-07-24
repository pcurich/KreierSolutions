using Ks.Batch.Util;
using Ks.Batch.Util.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf.Logging;

namespace Ks.Batch.Reverse
{
    public class DaoReverseLoan : DaoBase
    {
        private new static readonly LogWriter Log = HostLogger.Get<DaoReverseLoan>();
        private List<Info> _listInfo = new List<Info>();

        public DaoReverseLoan(string connetionString)
            : base(connetionString)
        {
        }


        public void RevertDataLoan(int periodYear, int periodMonth)
        {
            //try
            //{
            //    Sql = "UPDATE LoanPayment SET StateId = " + (int)LoanState.Pendiente + " WHERE ID IN ( " +
            //      " SELECT  lp.Id " +
            //      " FROM LoanPayment lp " +
            //      " INNER JOIN  Loan l on l.Id=lp.LoanId " +
            //      " WHERE lp.StateId=@StateId AND " +
            //      " l.Active=" + (int)State.Active + " " +
            //      " YEAR(lp.ScheduledDateOnUtc)=@Year AND  " +
            //      " MONTH(lp.ScheduledDateOnUtc)=@Month  ) ";

            //    Command = new SqlCommand(Sql, Connection);

            //    var pStateId = new SqlParameter
            //    {
            //        ParameterName = "@StateId",
            //        SqlDbType = SqlDbType.Int,
            //        Direction = ParameterDirection.Input,
            //        Value = (int)LoanState.EnProceso
            //    };

            //    var pYear = new SqlParameter
            //    {
            //        ParameterName = "@Year",
            //        SqlDbType = SqlDbType.Int,
            //        Direction = ParameterDirection.Input,
            //        Value = periodYear
            //    };
            //    var pMonth = new SqlParameter
            //    {
            //        ParameterName = "@Month",
            //        SqlDbType = SqlDbType.Int,
            //        Direction = ParameterDirection.Input,
            //        Value = periodMonth
            //    };

            //    Command.Parameters.Add(pStateId);
            //    Command.Parameters.Add(pYear);
            //    Command.Parameters.Add(pMonth);
            //    Command.ExecuteNonQuery();
            //}
            //catch (Exception ex)
            //{
            //    Log.FatalFormat("Action: {0} Error: {1}", "DaoReverseContributions.RevertDataLoan(" + periodYear + " - " + periodMonth + ")", ex.Message);
            //}
        }

    }
}
