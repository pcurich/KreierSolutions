using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Settings
{
    public class PaymentSettingsModel : BaseKsModel
    {
        [KsResourceDisplayName("Admin.Configuration.Settings.PaymentSettings.Amount")]
        public decimal Amount { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Settings.PaymentSettings.DayOfPayment")]
        public int DayOfPayment { get; set; }
    }
}