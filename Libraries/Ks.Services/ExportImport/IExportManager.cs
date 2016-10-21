using System.Collections.Generic;
using Ks.Core.Domain.Directory;
using System.IO;
using Ks.Core.Domain.Report;

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

        void ExportReportContributionPaymentToXlsx(Stream stream, IList<ReportContributionPayment> reportContributionPayment);
        
    }
}