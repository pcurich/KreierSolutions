﻿using System;
using System.Collections.Generic;
using Ks.Core.Domain.Reports;

namespace Ks.Services.Reports
{
    public interface IReportService
    {
        IList<ReportGlobal> GetGlobalReport(int year, int month, int type, string copere, string caja);
        IList<ReportLoan> GetLoan(int fromYear, int fromMonth, int fromDay, int toYear, int toMonth, int toDay, int type);
        IList<ReportLoanDetail> GetDetailLoan(int fromYear, int fromMonth, int fromDay, int toYear, int toMonth, int toDay, int type);
        IList<ReportContributionDetail> GetDetailContribution(int fromYear, int fromMonth, int fromDay, int toYear, int toMonth, int toDay, int type);
        IList<ReportSummaryContribution> GetSummaryContribution(int  minYear, int maxYear, int typeId);
        IList<ReportSummaryContribution> GetSummaryContribution(int minYear,int minMonth, int maxYear,int maxMonth, int typeId);

        IList<ReportBenefit> GetContributionBenefit(int fromMonth, int fromYear, int toMonth, int toYear, int typeId,int sourceId);

        IList<ReportMilitarSituation> GetMilitarSituation(int militarySituationId, int loanState = -1, int contributionState = -1);
        List<Info> GetInfo(string source, string period);
        IList<ReportBankPayment> GetBankPayment(int fromYear, int fromMonth, int fromDay, int toYear, int toMonth, int toDay, int typeId = -1, int sourceId = -1);
        IList<ReportInterfaceLoan> GetInterfaceLoan(int yearId, int monthId, int type, int state);
        IList<ReportInterfaceContribution> GetInterfaceContribution(int yearId, int monthId, int type, int state);

        IList<ReportInterfaceLoan> GetInterfaceLoanByAdminCode(string admCodes,int yearId, int monthId, int type, int state);
        IList<ReportInterfaceContribution> GetInterfaceContributionByAdminCode(string admCodes,int yearId, int monthId, int type, int state);

        IList<ReportChecks> GetChecks(int yearFrom, int monthFrom, int dayFrom, int yearTo, int monthTo, int dayTo, int typeId);
        IList<ReportCustomer> GetCustomer();

        bool CanRevertBatch(string period, string source1, int state1, string source2, int state2);

        List<ReportSummaryMerge> ExportReportSummaryMergeFromDataBase(int month, int year, int type);
        List<ReportSummaryMerge> ExportReportSummaryMergeDetailsFromDataBase(int month, int year, int type, int stateId);

        List<ReportSummaryMerge> ExportReportSummaryMergeFromFile(int month, int year, int type);
    }
}