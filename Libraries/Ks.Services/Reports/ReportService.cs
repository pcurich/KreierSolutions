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

        private readonly IRepository<Report> _reportRepository;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;

        #endregion

        #region Constructor

        public ReportService(IDataProvider dataProvider, IDbContext dbContext, IRepository<Report> reportRepository)
        {
            _reportRepository = reportRepository;
            _dataProvider = dataProvider;
            _dbContext = dbContext;
        }

        #endregion

        #region Methods

        public virtual bool CanRevertBatch(string period, string source1, int state1, string source2, int state2)
        {
            var query = from cr in _reportRepository.Table
                        orderby cr.Id
                        where ((cr.Source == source1 && cr.StateId == state1) || (cr.Source == source2 && cr.StateId == state2)) && cr.Period == period  
                        select cr;
            var canBeDeleted = query.Count();

            return canBeDeleted == 2;
        }

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

        public virtual IList<ReportLoanDetail> GetDetailLoan(int fromYear, int fromMonth, int fromDay, int toYear, int toMonth, int toDay, int type    )
        {
            if (fromYear == 0 || fromMonth == 0 || fromDay == 0 || toYear == 0 || toMonth == 0 || toDay == 0   || type == 0)
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
            var data = _dbContext.ExecuteStoredProcedureList<Report>("ReportLoanDetails", pFromYear, pFromMonth, pFromDay, pToYear, pToMonth, pToDay, pType,  pNameReport, pReportState, pSource, pTotalRecords);

            //return products
            var totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            var firstOrDefault = data.FirstOrDefault();
            if (firstOrDefault != null && firstOrDefault.Value != null)
                return new List<ReportLoanDetail>(XmlHelper.XmlToObject<List<ReportLoanDetail>>(firstOrDefault.Value));

            return new List<ReportLoanDetail>();
        }

        public virtual IList<ReportContributionDetail> GetDetailContribution(int fromYear, int fromMonth, int fromDay, int toYear, int toMonth, int toDay, int type )
        {
            if (fromYear == 0 || fromMonth == 0 || fromDay == 0 || toYear == 0 || toMonth == 0 || toDay == 0   || type == 0)
                return new List<ReportContributionDetail>();

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

            var pNameReport = _dataProvider.GetParameter();
            pNameReport.ParameterName = "NameReport";
            pNameReport.Value = "ContributionDetailReport";
            pNameReport.DbType = DbType.String;

            var pReportState = _dataProvider.GetParameter();
            pReportState.ParameterName = "ReportState";
            pReportState.Value = (int)ReportState.Completed;
            pReportState.DbType = DbType.Int32;

            var pSource = _dataProvider.GetParameter();
            pSource.ParameterName = "Source";
            pSource.Value = "Ks.Services.Report.GetDetailContribution";
            pSource.DbType = DbType.String;

            var pTotalRecords = _dataProvider.GetParameter();
            pTotalRecords.ParameterName = "TotalRecords";
            pTotalRecords.Direction = ParameterDirection.Output;
            pTotalRecords.DbType = DbType.Int32;

            //invoke stored procedure
            var data = _dbContext.ExecuteStoredProcedureList<Report>("ReportContributionDetails", pFromYear, pFromMonth, pFromDay, pToYear, pToMonth, pToDay, pType, pNameReport, pReportState, pSource, pTotalRecords);

            //return products
            var totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            var firstOrDefault = data.FirstOrDefault();
            if (firstOrDefault != null && firstOrDefault.Value != null)
                return new List<ReportContributionDetail>(XmlHelper.XmlToObject<List<ReportContributionDetail>>(firstOrDefault.Value));

            return new List<ReportContributionDetail>();
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

        public virtual IList<ReportSummaryContribution> GetSummaryContribution(int minYear,int minMonth, int maxYear,int maxMonth, int typeId)
        {
            if (minYear == 0 || maxYear == 0 || minMonth==0 || maxMonth==0)
                return new List<ReportSummaryContribution>();

            var pFromYear = _dataProvider.GetParameter();
            pFromYear.ParameterName = "FromYear";
            pFromYear.Value = minYear;
            pFromYear.DbType = DbType.Int32;

            var pFromMonth = _dataProvider.GetParameter();
            pFromMonth.ParameterName = "FromMonth";
            pFromMonth.Value = minMonth;
            pFromMonth.DbType = DbType.Int32;

            var pToYear = _dataProvider.GetParameter();
            pToYear.ParameterName = "ToYear";
            pToYear.Value = maxYear;
            pToYear.DbType = DbType.Int32;

            var pToMonth = _dataProvider.GetParameter();
            pToMonth.ParameterName = "ToMonth";
            pToMonth.Value = maxMonth;
            pToMonth.DbType = DbType.Int32;

            var pTypeSource = _dataProvider.GetParameter();
            pTypeSource.ParameterName = "TypeSource"; //1 todos 2copere 3 caja
            pTypeSource.Value = typeId == 2 ? 1 : typeId == 3 ? 2 : 0;
            pTypeSource.DbType = DbType.Int32;

            var pNameReport = _dataProvider.GetParameter();
            pNameReport.ParameterName = "NameReport";
            pNameReport.Value = "SummaryContributionReport - year and month";
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
            var data = _dbContext.ExecuteStoredProcedureList<Report>("SummaryReportContribution2", pFromYear, pFromMonth, pToYear, pToMonth, pTypeSource, pNameReport, pReportState, pSource, pTotalRecords);

            //return products
            var totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            var firstOrDefault = data.FirstOrDefault();
            if (firstOrDefault != null && firstOrDefault.Value != null)
                return new List<ReportSummaryContribution>(XmlHelper.XmlToObject<List<ReportSummaryContribution>>(firstOrDefault.Value));

            return new List<ReportSummaryContribution>();
        }

        public virtual IList<ReportBenefit> GetContributionBenefit(int fromMonth, int fromYear, int toMonth, int toYear, int typeId, int sourceId)
        {
            if (typeId == 0 || sourceId == 0)
                return new List<ReportBenefit>();

            var pFromMonth = _dataProvider.GetParameter();
            pFromMonth.ParameterName = "FromMonth";
            pFromMonth.Value = fromMonth;
            pFromMonth.DbType = DbType.Int32;

            var pFromYear = _dataProvider.GetParameter();
            pFromYear.ParameterName = "FromYear";
            pFromYear.Value = fromYear;
            pFromYear.DbType = DbType.Int32;

            var pToMonth = _dataProvider.GetParameter();
            pToMonth.ParameterName = "ToMonth";
            pToMonth.Value = toMonth;
            pToMonth.DbType = DbType.Int32;

            var pToYear = _dataProvider.GetParameter();
            pToYear.ParameterName = "ToYear";
            pToYear.Value = toYear;
            pToYear.DbType = DbType.Int32;

            var pType = _dataProvider.GetParameter();
            pType.ParameterName = "Type"; //1 todos 2copere 3 caja
            pType.Value = typeId == 2 ? 1 : typeId == 3 ? 2 : 0;
            pType.DbType = DbType.Int32;

            var pBenefitId = _dataProvider.GetParameter();
            pBenefitId.ParameterName = "BenefitId";
            pBenefitId.Value = sourceId;
            pBenefitId.DbType = DbType.Int32;

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
            pSource.Value = "Ks.Services.Report.GetContributionBenefit";
            pSource.DbType = DbType.String;

            var pTotalRecords = _dataProvider.GetParameter();
            pTotalRecords.ParameterName = "TotalRecords";
            pTotalRecords.Direction = ParameterDirection.Output;
            pTotalRecords.DbType = DbType.Int32;

            //invoke stored procedure
            var data = _dbContext.ExecuteStoredProcedureList<Report>("ReportContributionBenefit", pFromYear, pFromMonth, pToYear, pToMonth, pType, pBenefitId, pNameReport, pReportState, pSource, pTotalRecords);

            //return products
            var totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            var firstOrDefault = data.FirstOrDefault();
            if (firstOrDefault != null && firstOrDefault.Value != null)
                return new List<ReportBenefit>(XmlHelper.XmlToObject<List<ReportBenefit>>(firstOrDefault.Value));

            return new List<ReportBenefit>();
        }

        public virtual IList<ReportMilitarSituation> GetMilitarSituation(int militarySituationId, int loanState = -1, int contributionState = -1)
        {
            var pMilitarSituation = _dataProvider.GetParameter();
            pMilitarSituation.ParameterName = "MilitarSituation";
            pMilitarSituation.Value = militarySituationId;
            pMilitarSituation.DbType = DbType.Int32;

            var pLoanState = _dataProvider.GetParameter();
            pLoanState.ParameterName = "LoanState";
            pLoanState.Value = loanState;
            pLoanState.DbType = DbType.Int32;

            var pContributionState = _dataProvider.GetParameter();
            pContributionState.ParameterName = "ContributionState";
            pContributionState.Value = contributionState;
            pContributionState.DbType = DbType.Int32;

            var pNameReport = _dataProvider.GetParameter();
            pNameReport.ParameterName = "NameReport";
            pNameReport.Value = "MilitarSituation";
            pNameReport.DbType = DbType.String;

            var pReportState = _dataProvider.GetParameter();
            pReportState.ParameterName = "ReportState";
            pReportState.Value = (int)ReportState.Completed;
            pReportState.DbType = DbType.Int32;

            var pSource = _dataProvider.GetParameter();
            pSource.ParameterName = "Source";
            pSource.Value = "Ks.Services.Report.GetMilitarSituation";
            pSource.DbType = DbType.String;

            var pTotalRecords = _dataProvider.GetParameter();
            pTotalRecords.ParameterName = "TotalRecords";
            pTotalRecords.Direction = ParameterDirection.Output;
            pTotalRecords.DbType = DbType.Int32;

            //invoke stored procedure
            var data = _dbContext.ExecuteStoredProcedureList<Report>("ReportMilitarSituation", pMilitarSituation, pLoanState, pContributionState, pNameReport, pReportState, pSource, pTotalRecords);

            //return products
            var totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            var firstOrDefault = data.FirstOrDefault();
            if (firstOrDefault != null && firstOrDefault.Value != null)
                return new List<ReportMilitarSituation>(XmlHelper.XmlToObject<List<ReportMilitarSituation>>(firstOrDefault.Value));

            return new List<ReportMilitarSituation>();
        }

        public virtual IList<ReportBankPayment> GetBankPayment(int fromYear, int fromMonth, int fromDay, int toYear, int toMonth, int toDay, int typeId = -1, int sourceId = -1)
        {
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
            pType.Value = typeId;
            pType.DbType = DbType.Int32;

            var pData = _dataProvider.GetParameter();
            pData.ParameterName = "Data";
            pData.Value = sourceId;
            pData.DbType = DbType.Int32;

            var pNameReport = _dataProvider.GetParameter();
            pNameReport.ParameterName = "NameReport";
            pNameReport.Value = "SummaryBankPayment";
            pNameReport.DbType = DbType.String;

            var pReportState = _dataProvider.GetParameter();
            pReportState.ParameterName = "ReportState";
            pReportState.Value = (int)ReportState.Completed;
            pReportState.DbType = DbType.Int32;

            var pSource = _dataProvider.GetParameter();
            pSource.ParameterName = "Source";
            pSource.Value = "Ks.Services.Report.GetSummaryBankPayment";
            pSource.DbType = DbType.String;

            var pTotalRecords = _dataProvider.GetParameter();
            pTotalRecords.ParameterName = "TotalRecords";
            pTotalRecords.Direction = ParameterDirection.Output;
            pTotalRecords.DbType = DbType.Int32;

            //invoke stored procedure
            var data = _dbContext.ExecuteStoredProcedureList<Report>("ReportSummaryBankPayment",
                pFromYear, pFromMonth, pFromDay, pToYear, pToMonth, pToDay, pType, pData,
                pNameReport, pReportState, pSource, pTotalRecords);

            //return products
            var totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
             var firstOrDefault = data.FirstOrDefault();
            if (firstOrDefault != null && firstOrDefault.Value != null)
                return new List<ReportBankPayment>(XmlHelper.XmlToObject<List<ReportBankPayment>>(firstOrDefault.Value));

            return new List<ReportBankPayment>();
        }

        public virtual IList<ReportInterfaceLoan> GetInterfaceLoan(int yearId, int monthId, int type, int state)
        {
            var pYear = _dataProvider.GetParameter();
            pYear.ParameterName = "Year";
            pYear.Value = yearId;
            pYear.DbType = DbType.Int32;

            var pMonth = _dataProvider.GetParameter();
            pMonth.ParameterName = "Month";
            pMonth.Value = monthId;
            pMonth.DbType = DbType.Int32;

            var pType = _dataProvider.GetParameter();
            pType.ParameterName = "Type";
            pType.Value = type;
            pType.DbType = DbType.Int32;

            var pState = _dataProvider.GetParameter();
            pState.ParameterName = "State";
            pState.Value = state;
            pState.DbType = DbType.Int32;

            var pNameReport = _dataProvider.GetParameter();
            pNameReport.ParameterName = "NameReport";
            pNameReport.Value = "InterfaceLoan";
            pNameReport.DbType = DbType.String;

            var pReportState = _dataProvider.GetParameter();
            pReportState.ParameterName = "ReportState";
            pReportState.Value = (int)ReportState.Completed;
            pReportState.DbType = DbType.Int32;

            var pSource = _dataProvider.GetParameter();
            pSource.ParameterName = "Source";
            pSource.Value = "Ks.Services.Report.GetInterfaceLoan";
            pSource.DbType = DbType.String;

            var pTotalRecords = _dataProvider.GetParameter();
            pTotalRecords.ParameterName = "TotalRecords";
            pTotalRecords.Direction = ParameterDirection.Output;
            pTotalRecords.DbType = DbType.Int32;

            //invoke stored procedure
            var data = _dbContext.ExecuteStoredProcedureList<Report>("ReportInterfaceLoan",
                pYear, pMonth, pType,pState,pNameReport, pReportState, pSource, pTotalRecords);

            //return products
            var totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            var firstOrDefault = data.FirstOrDefault();
            if (firstOrDefault != null && firstOrDefault.Value != null)
                return new List<ReportInterfaceLoan>(XmlHelper.XmlToObject<List<ReportInterfaceLoan>>(firstOrDefault.Value));

            return new List<ReportInterfaceLoan>();
        }

        public virtual IList<ReportInterfaceContribution> GetInterfaceContribution(int yearId, int monthId, int type, int state)
        {
            var pYear = _dataProvider.GetParameter();
            pYear.ParameterName = "Year";
            pYear.Value = yearId;
            pYear.DbType = DbType.Int32;

            var pMonth = _dataProvider.GetParameter();
            pMonth.ParameterName = "Month";
            pMonth.Value = monthId;
            pMonth.DbType = DbType.Int32;

            var pType = _dataProvider.GetParameter();
            pType.ParameterName = "Type";
            pType.Value = type;
            pType.DbType = DbType.Int32;

            var pState = _dataProvider.GetParameter();
            pState.ParameterName = "State";
            pState.Value = state;
            pState.DbType = DbType.Int32;

            var pNameReport = _dataProvider.GetParameter();
            pNameReport.ParameterName = "NameReport";
            pNameReport.Value = "InterfaceContribution";
            pNameReport.DbType = DbType.String;

            var pReportState = _dataProvider.GetParameter();
            pReportState.ParameterName = "ReportState";
            pReportState.Value = (int)ReportState.Completed;
            pReportState.DbType = DbType.Int32;

            var pSource = _dataProvider.GetParameter();
            pSource.ParameterName = "Source";
            pSource.Value = "Ks.Services.Report.GetInterfaceContribution";
            pSource.DbType = DbType.String;

            var pTotalRecords = _dataProvider.GetParameter();
            pTotalRecords.ParameterName = "TotalRecords";
            pTotalRecords.Direction = ParameterDirection.Output;
            pTotalRecords.DbType = DbType.Int32;

            //invoke stored procedure
            var data = _dbContext.ExecuteStoredProcedureList<Report>("ReportInterfaceContribution",
                pYear, pMonth, pType,pState,
                pNameReport, pReportState, pSource, pTotalRecords);

            //return products
            var totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            var firstOrDefault = data.FirstOrDefault();
            if (firstOrDefault != null && firstOrDefault.Value != null)
                return new List<ReportInterfaceContribution>(XmlHelper.XmlToObject<List<ReportInterfaceContribution>>(firstOrDefault.Value));

            return new List<ReportInterfaceContribution>();
        }

        public virtual IList<ReportChecks> GetChecks(int fromYear, int fromMonth, int fromDay, int toYear, int toMonth, int toDay, int type)
        {
            if (fromYear == 0 || fromMonth == 0 || fromDay == 0 || toYear == 0 || toMonth == 0 || toDay == 0)
                return new List<ReportChecks>();

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
            pType.Value = type; //0 TODOS, 1 LOAN  , 2 RETURNPAYMENT  3 BENEFIT
            pType.DbType = DbType.Int32;

            var pNameReport = _dataProvider.GetParameter();
            pNameReport.ParameterName = "NameReport";
            pNameReport.Value = "GetChecks";
            pNameReport.DbType = DbType.String;

            var pReportState = _dataProvider.GetParameter();
            pReportState.ParameterName = "ReportState";
            pReportState.Value = (int)ReportState.Completed;
            pReportState.DbType = DbType.Int32;

            var pSource = _dataProvider.GetParameter();
            pSource.ParameterName = "Source";
            pSource.Value = "Ks.Services.Report.GetChecks";
            pSource.DbType = DbType.String;

            var pTotalRecords = _dataProvider.GetParameter();
            pTotalRecords.ParameterName = "TotalRecords";
            pTotalRecords.Direction = ParameterDirection.Output;
            pTotalRecords.DbType = DbType.Int32;

            //invoke stored procedure
            var data = _dbContext.ExecuteStoredProcedureList<Report>("ReportChecks", pFromYear, pFromMonth, pFromDay, pToYear, pToMonth, pToDay, pType, pNameReport, pReportState, pSource, pTotalRecords);

            //return products
            var totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            var firstOrDefault = data.FirstOrDefault();
            if (firstOrDefault != null && firstOrDefault.Value != null)
                return new List<ReportChecks>(XmlHelper.XmlToObject<List<ReportChecks>>(firstOrDefault.Value));

            return new List<ReportChecks>();
        }

        public virtual IList<ReportCustomer> GetCustomer()
        {
            var pNameReport = _dataProvider.GetParameter();
            pNameReport.ParameterName = "NameReport";
            pNameReport.Value = "GetCustomer";
            pNameReport.DbType = DbType.String;

            var pReportState = _dataProvider.GetParameter();
            pReportState.ParameterName = "ReportState";
            pReportState.Value = (int)ReportState.Completed;
            pReportState.DbType = DbType.Int32;

            var pSource = _dataProvider.GetParameter();
            pSource.ParameterName = "Source";
            pSource.Value = "Ks.Services.Report.GetCustomer";
            pSource.DbType = DbType.String;

            var pTotalRecords = _dataProvider.GetParameter();
            pTotalRecords.ParameterName = "TotalRecords";
            pTotalRecords.Direction = ParameterDirection.Output;
            pTotalRecords.DbType = DbType.Int32;

            //invoke stored procedure
            var data = _dbContext.ExecuteStoredProcedureList<Report>("ReportCustomer", pNameReport, pReportState, pSource, pTotalRecords);

            //return products
            var totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            var firstOrDefault = data.FirstOrDefault();
            if (firstOrDefault != null && firstOrDefault.Value != null)
                return new List<ReportCustomer>(XmlHelper.XmlToObject<List<ReportCustomer>>(firstOrDefault.Value));

            return new List<ReportCustomer>();
        }
        public virtual List<Info> GetInfo(string source, string period)
        {
            var sql = "SELECT value FROM report WHERE Period=@Period and Source=@Source";

            var pPeriod = _dataProvider.GetParameter();
            pPeriod.ParameterName = "Period";
            pPeriod.Value = period;
            pPeriod.DbType = DbType.String;

            var pSource = _dataProvider.GetParameter();
            pSource.ParameterName = "Source";
            pSource.Value = source;
            pSource.DbType = DbType.String;

            var data = _dbContext.SqlQuery<string>(sql, pPeriod, pSource);

            var firstOrDefault = data.FirstOrDefault();
            if (firstOrDefault != null || firstOrDefault.Length > 1)
                return new List<Info>(XmlHelper.XmlToObject<List<Info>>(firstOrDefault));

            return null;
        }

        #endregion

    }
}