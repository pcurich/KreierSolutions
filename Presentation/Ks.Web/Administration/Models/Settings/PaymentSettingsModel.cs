using FluentValidation.Attributes;
using Ks.Admin.Validators.Settings;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Settings
{
    [Validator(typeof(PaymentSettingsModelValidator))]
    public class PaymentSettingsModel : BaseKsModel
    {
        [KsResourceDisplayName("Admin.Configuration.Settings.PaymentSettings.TotalCycle")]
        public int TotalCycle { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Settings.PaymentSettings.DayOfPayment")]
        public int DayOfPayment { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Settings.PaymentSettings.NameAmount1")]
        public string NameAmount1 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.PaymentSettings.IsActiveAmount1")]
        public bool IsActiveAmount1 { set; get; }
        [KsResourceDisplayName("Admin.Configuration.Settings.PaymentSettings.Amount1")]
        public decimal Amount1 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.PaymentSettings.NameAmount2")]
        public string NameAmount2 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.PaymentSettings.IsActiveAmount2")]
        public bool IsActiveAmount2 { set; get; }
        [KsResourceDisplayName("Admin.Configuration.Settings.PaymentSettings.Amount2")]
        public decimal Amount2 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.PaymentSettings.NameAmount3")]
        public string NameAmount3{ get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.PaymentSettings.IsActiveAmount3")]
        public bool IsActiveAmount3 { set; get; }
        [KsResourceDisplayName("Admin.Configuration.Settings.PaymentSettings.Amount3")]
        public decimal Amount3 { get; set; }

        
    }
}