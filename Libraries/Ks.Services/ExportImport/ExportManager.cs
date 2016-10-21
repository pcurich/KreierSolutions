using System;
using System.Collections.Generic;
using System.Text;
using Ks.Core.Domain.Directory;
using System.IO;
using Ks.Core.Domain.Report;

namespace Ks.Services.ExportImport
{
    /// <summary>
    /// Export manager
    /// </summary>
    public partial class ExportManager : IExportManager
    {
        #region Methods

        /// <summary>
        /// Export states to TXT
        /// </summary>
        /// <param name="states">States</param>
        /// <returns>Result in TXT (string) format</returns>
        public virtual string ExportStatesToTxt(IList<StateProvince> states)
        {
            if (states == null)
                throw new ArgumentNullException("states");

            const string SEPARATOR = ",";
            var sb = new StringBuilder();
            foreach (var state in states)
            {
                sb.Append(state.Country.TwoLetterIsoCode);
                sb.Append(SEPARATOR);
                sb.Append(state.Name);
                sb.Append(SEPARATOR);
                sb.Append(state.Abbreviation);
                sb.Append(SEPARATOR);
                sb.Append(state.Published);
                sb.Append(SEPARATOR);
                sb.Append(state.DisplayOrder);
                sb.Append(Environment.NewLine);  //new line
            }
            return sb.ToString();
        }

        public virtual void ExportReportContributionPaymentToXlsx(Stream stream, IList<ReportContributionPayment> reportContributionPayment)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

                using (var xlPackage = new ExcelPackage(stream))
                {

                }
        }

        #endregion
    }
}