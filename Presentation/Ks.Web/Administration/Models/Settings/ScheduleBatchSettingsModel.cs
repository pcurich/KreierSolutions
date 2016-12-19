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
    }
}