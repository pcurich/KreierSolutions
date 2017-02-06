using System;
using System.Collections.Generic;
using Ks.Core.Domain.Directory;
using System.IO;
using Ks.Core.Domain.Batchs;
using Ks.Core.Domain.Contract;
using Ks.Core.Domain.Customers;
using Ks.Core.Domain.Reports;

namespace Ks.Services.ExportImport
{
    /// <summary>
    /// Export manager interface
    /// </summary>
    public partial interface IExportManager
    {
        /// <summary>
        /// Export states to TXT
        /// </summary>
        /// <param name="states">States</param>
        /// <returns>Result in TXT (string) format</returns>
        string ExportStatesToTxt(IList<StateProvince> states);

        void ExportReportContributionPaymentToXlsx(Stream stream, Customer customer, Contribution contribution, IList<ReportContributionPayment> reportContributionPayment);

        void ExportReportLoanPaymentToXlsx(Stream stream, Customer customer, Loan loan, IList<ReportLoanPayment> reportLoanPayment);
        void ExportReportLoanPaymentKardexToXlsx(Stream stream, Customer customer, Loan loan, IList<ReportLoanPaymentKardex> reportLoanPaymentKardex);
        void ExportReportContributionBenefitToXlsx(Stream stream, Customer customer, ContributionBenefit contributionBenefit, IList<ReportContributionBenefit> reportContributionBenefit);
        void ExportGlobalReportToXlsx(MemoryStream stream, int year, int month, IList<ReportGlobal> globalReport);
        void ExportDetailLoanToXlsx(MemoryStream stream, DateTime from, DateTime to, string source, IList<ReportLoanDetail> reportLoan);
        void ExportSummaryContributionToXlsx(MemoryStream stream, int fromId, int toId, int typeId, IList<ReportSummaryContribution> summaryContribution);
        void ExportBenefitToXlsx(MemoryStream stream, Benefit getBenefitById, IList<ReportBenefit> benefit);
        void ExportMilitarSituationToXlsx(MemoryStream stream, string militarySituation, IList<ReportMilitarSituation> militarSituations);
        string ExportScheduleTxt(ScheduleBatch schedule);
    }
}