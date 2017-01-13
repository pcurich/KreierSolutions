using System;
using System.Collections.Generic;
using Ks.Core.Domain.Reports;

namespace Ks.Services.Reports
{
    public interface IReportService
    {
        IList<ReportGlobal> GetGlobalReport(int year, int month, int type, string copere, string caja);
        IList<ReportLoanDetail> GetDetailLoan(int fromYear, int fromMonth, int fromDay, int toYear, int toMonth, int toDay, int type, int state);
        IList<ReportSummaryContribution> GetSummaryContribution(int  minYear, int maxYear, int typeId);

        IList<ReportBenefit> GetContributionBenefit(int fromMonth, int fromYear, int toMonth, int toYear, int typeId,
            int sourceId);
    }
}