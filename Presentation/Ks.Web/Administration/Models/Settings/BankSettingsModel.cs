using FluentValidation.Attributes;
using Ks.Admin.Validators.Settings;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Settings
{
    [Validator(typeof(BankSettingsModelValidator))]
    public class BankSettingsModel : BaseKsModel
    {
        public int IdBank1 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.BankSettings.IsActive")]
        public bool IsActive1 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.BankSettings.NameBank")]
        public string NameBank1 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.BankSettings.AccountNumber")]
        public string AccountNumber1 { get; set; }
        public int IdBank2 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.BankSettings.IsActive")]
        public bool IsActive2 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.BankSettings.NameBank")]
        public string NameBank2 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.BankSettings.AccountNumber")]
        public string AccountNumber2 { get; set; }
        public int IdBank3 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.BankSettings.IsActive")]
        public bool IsActive3 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.BankSettings.NameBank")]
        public string NameBank3 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.BankSettings.AccountNumber")]
        public string AccountNumber3 { get; set; }
        public int IdBank4 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.BankSettings.IsActive")]
        public bool IsActive4 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.BankSettings.NameBank")]
        public string NameBank4 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.BankSettings.AccountNumber")]
        public string AccountNumber4 { get; set; }
        public int IdBank5 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.BankSettings.IsActive")]
        public bool IsActive5 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.BankSettings.NameBank")]
        public string NameBank5 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.BankSettings.AccountNumber")]
        public string AccountNumber5 { get; set; }

    }
}