using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Ks.Core;
using Ks.Core.Data;
using Ks.Core.Domain.Reports;
using Ks.Data;

namespace Ks.Services.Reports
{
    public class ReportService : IReportService
    {
        #region Fields

        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;

        #endregion

        #region Constructor

        public ReportService(IDataProvider dataProvider, IDbContext dbContext)
        {
            _dataProvider = dataProvider;
            _dbContext = dbContext;
        }

        #endregion

        #region Methods

        public virtual IList<ReportGlobal> GetGlobalReport(int year, int month, int type, string copere, string caja)
        {
            if (year == 0 || month == 0)
                return new List<ReportGlobal>();

            var pYear = _dataProvider.GetParameter();
            pYear.ParameterName = "Year";
            pYear.Value = year;
            pYear.DbType = DbType.Int32;

            var pMonth = _dataProvider.GetParameter();
            pMonth.ParameterName = "Month";
            pMonth.Value = month;
            pMonth.DbType = DbType.Int32;

            var pType = _dataProvider.GetParameter();
            pType.ParameterName = "Type";
            pType.Value = type;
            pType.DbType = DbType.Int32;

            var pCopere = _dataProvider.GetParameter();
            pCopere.ParameterName = "Copere";
            pCopere.Value = copere;
            pCopere.DbType = DbType.String;

            var pCaja = _dataProvider.GetParameter();
            pCaja.ParameterName = "Caja";
            pCaja.Value = caja;
            pCaja.DbType = DbType.String;

            var pNameReport = _dataProvider.GetParameter();
            pNameReport.ParameterName = "NameReport";
            pNameReport.Value = "GlobalReport";
            pNameReport.DbType = DbType.String;

            var pReportState = _dataProvider.GetParameter();
            pReportState.ParameterName = "ReportState";
            pReportState.Value = (int)ReportState.Completed;
            pReportState.DbType = DbType.Int32;

            var pSource = _dataProvider.GetParameter();
            pSource.ParameterName = "Source";
            pSource.Value = "Ks.Services.Report.GetGlobalReport";
            pSource.DbType = DbType.String;

            var pTotalRecords = _dataProvider.GetParameter();
            pTotalRecords.ParameterName = "TotalRecords";
            pTotalRecords.Direction = ParameterDirection.Output;
            pTotalRecords.DbType = DbType.Int32;

            //invoke stored procedure
            var data = _dbContext.ExecuteStoredProcedureList<Report>("ReportGlobal", pYear, pMonth, pType, pCopere, pCaja, pNameReport, pReportState, pSource, pTotalRecords);

            //return products
            var totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            var firstOrDefault = data.FirstOrDefault();
            if (firstOrDefault != null && firstOrDefault.Value != null)
                return new List<ReportGlobal>(XmlHelper.XmlToObject<List<ReportGlobal>>(firstOrDefault.Value));

            return new List<ReportGlobal>();
        }

        public virtual IList<ReportLoanDetail> GetDetailLoan(int fromYear, int fromMonth, int fromDay, int toYear, int toMonth, int toDay, int type, int state)
        {
            if (fromYear == 0 || fromMonth == 0 || fromDay == 0 || toYear == 0 || toMonth == 0 || toDay == 0 || state == 0 || type == 0)
                return new List<ReportLoanDetail>();

            var pFromYear = _dataProvider.GetParameter();
            pFromYear.ParameterName = "FromYear";
            pFromYear.Value = fromYear;
            pFromYear.DbType = DbType.Int32;

            var pFromMonth = _dataProvider.GetParameter();
            pFromMonth.ParameterName = "FromMonth";
            pFromMonth.Value = fromMonth;
            pFromMonth.DbType = DbType.Int32;

            var pFromDay = _dataProvider.GetParameter();
            pFromDay.ParameterName = "FromDay";
            pFromDay.Value = fromDay;
            pFromDay.DbType = DbType.Int32;

            var pToYear = _dataProvider.GetParameter();
            pToYear.ParameterName = "ToYear";
            pToYear.Value = toYear;
            pToYear.DbType = DbType.Int32;

            var pToMonth = _dataProvider.GetParameter();
            pToMonth.ParameterName = "ToMonth";
            pToMonth.Value = toMonth;
            pToMonth.DbType = DbType.Int32;

            var pToDay = _dataProvider.GetParameter();
            pToDay.ParameterName = "ToDay";
            pToDay.Value = toDay;
            pToDay.DbType = DbType.Int32;

            var pType = _dataProvider.GetParameter();
            pType.ParameterName = "Type";
            pType.Value = type == 1 ? 0 : type == 2 ? 1 : 2; //1 Todos 2 Copere 3 Caja
            pType.DbType = DbType.Int32;

            var pState = _dataProvider.GetParameter();
            pState.ParameterName = "State";
            pState.Value = state == 1 ? 2 : state == 2 ? 1 : 0; //1 Todos 2 Vigente 3 Cancelado
            pState.DbType = DbType.Int32;

            var pNameReport = _dataProvider.GetParameter();
            pNameReport.ParameterName = "NameReport";
            pNameReport.Value = "LoanDetailReport";
            pNameReport.DbType = DbType.String;

            var pReportState = _dataProvider.GetParameter();
            pReportState.ParameterName = "ReportState";
            pReportState.Value = (int)ReportState.Completed;
            pReportState.DbType = DbType.Int32;

            var pSource = _dataProvider.GetParameter();
            pSource.ParameterName = "Source";
            pSource.Value = "Ks.Services.Report.GetDetailLoan";
            pSource.DbType = DbType.String;

            var pTotalRecords = _dataProvider.GetParameter();
            pTotalRecords.ParameterName = "TotalRecords";
            pTotalRecords.Direction = ParameterDirection.Output;
            pTotalRecords.DbType = DbType.Int32;

            //invoke stored procedure
            var data = _dbContext.ExecuteStoredProcedureList<Report>("ReportLoanDetails", pFromYear, pFromMonth, pFromDay, pToYear, pToMonth, pToDay, pType, pState, pNameReport, pReportState, pSource, pTotalRecords);

            //return products
            var totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            var firstOrDefault = data.FirstOrDefault();
            if (firstOrDefault != null && firstOrDefault.Value != null)
                return new List<ReportLoanDetail>(XmlHelper.XmlToObject<List<ReportLoanDetail>>(firstOrDefault.Value));

            return new List<ReportLoanDetail>();
        }

        public virtual IList<ReportSummaryContribution> GetSummaryContribution(int minYear, int maxYear, int typeId)
        {
            if (minYear == 0 || maxYear == 0)
                return new List<ReportSummaryContribution>();

            var pFromYear = _dataProvider.GetParameter();
            pFromYear.ParameterName = "FromYear";
            pFromYear.Value = minYear;
            pFromYear.DbType = DbType.Int32;

            var pToYear = _dataProvider.GetParameter();
            pToYear.ParameterName = "ToYear";
            pToYear.Value = maxYear;
            pToYear.DbType = DbType.Int32;

            var pTypeSource = _dataProvider.GetParameter();
            pTypeSource.ParameterName = "TypeSource"; //1 todos 2copere 3 caja
            pTypeSource.Value = typeId == 2 ? 1 : typeId == 3 ? 2 : 0;
            pTypeSource.DbType = DbType.Int32;

            var pNameReport = _dataProvider.GetParameter();
            pNameReport.ParameterName = "NameReport";
            pNameReport.Value = "SummaryContributionReport";
            pNameReport.DbType = DbType.String;

            var pReportState = _dataProvider.GetParameter();
            pReportState.ParameterName = "ReportState";
            pReportState.Value = (int)ReportState.Completed;
            pReportState.DbType = DbType.Int32;

            var pSource = _dataProvider.GetParameter();
            pSource.ParameterName = "Source";
            pSource.Value = "Ks.Services.Report.GetSummaryContribution";
            pSource.DbType = DbType.String;

            var pTotalRecords = _dataProvider.GetParameter();
            pTotalRecords.ParameterName = "TotalRecords";
            pTotalRecords.Direction = ParameterDirection.Output;
            pTotalRecords.DbType = DbType.Int32;

            //invoke stored procedure
            var data = _dbContext.ExecuteStoredProcedureList<Report>("SummaryReportContribution", pFromYear, pToYear, pTypeSource, pNameReport, pReportState, pSource, pTotalRecords);

            //return products
            var totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            var firstOrDefault = data.FirstOrDefault();
            if (firstOrDefault != null && firstOrDefault.Value != null)
                return new List<ReportSummaryContribution>(XmlHelper.XmlToObject<List<ReportSummaryContribution>>(firstOrDefault.Value));

            return new List<ReportSummaryContribution>();
        }

        public virtual IList<ReportBenefit> GetBenefit(DateTime value, DateTime dateTime, int typeId, int sourceId)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}