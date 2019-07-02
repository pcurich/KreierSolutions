using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Settings
{
    public class ScheduleBatchSettingsModel : BaseKsModel
    {
        [KsResourceDisplayName("Admin.Configuration.Settings.ScheduleBatch.ServiceName1")]
        public string ServiceName1 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ScheduleBatch.DayOfProcess1")]
        public int DayOfProcess1 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ScheduleBatch.ServiceName2")]
        public string ServiceName2 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ScheduleBatch.DayOfProcess2")]
        public int DayOfProcess2 { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Settings.ScheduleBatch.PreContributionNameNoCash")]
        public string ServicePreContributionNameNoCash { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ScheduleBatch.PreContributionSourceNoCash")]
        public string ServicePreContributionSourceNoCash { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Settings.ScheduleBatch.PreContributionNamePayedComplete")]
        public string ServicePreContributionNamePayedComplete { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ScheduleBatch.PreContributionSourcePayedComplete")]
        public string ServicePreContributionSourcePayedComplete { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Settings.ScheduleBatch.PreContributionNameIncomplete")]
        public string ServicePreContributionNameIncomplete { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ScheduleBatch.PreContributionSourceIncomplete")]
        public string ServicePreContributionSourceIncomplete { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Settings.ScheduleBatch.PreLoanNameNoCash")]
        public string ServicePreLoanNameNoCash { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ScheduleBatch.PreLoanSourceNoCash")]
        public string ServicePreLoanSourceNoCash { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Settings.ScheduleBatch.PreLoanNamePayedComplete")]
        public string ServicePreLoanNamePayedComplete { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ScheduleBatch.PreLoanSourcePayedComplete")]
        public string ServicePreLoanSourcePayedComplete { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Settings.ScheduleBatch.PreLoanNameIncomplete")]
        public string ServicePreLoanNameIncomplete { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ScheduleBatch.PreLoanSourceIncomplete")]
        public string ServicePreLoanSourceIncomplete { get; set; }
    }
}