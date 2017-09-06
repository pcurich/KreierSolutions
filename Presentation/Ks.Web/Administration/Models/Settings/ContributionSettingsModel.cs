using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Ks.Admin.Validators.Settings;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Settings
{
    [Validator(typeof(PaymentSettingsModelValidator))]
    public class ContributionSettingsModel : BaseKsModel
    {
        public ContributionSettingsModel()
        {
            Amount1Sources = new List<SelectListItem>();
            Amount2Sources = new List<SelectListItem>();
            Amount3Sources = new List<SelectListItem>();
        }

        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.TotalCycle")]
        public int TotalCycle { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.DayOfPaymentContribution")]
        public int DayOfPaymentContribution { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.CycleOfDelay")]
        public int CycleOfDelay { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.AmountMeta")]
        public decimal AmountMeta { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.MaximumChargeCaja")]
        public decimal MaximumChargeCaja { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.MaximumChargeCopere")]
        public decimal MaximumChargeCopere { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.NameAmount1")]
        public string NameAmount1 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.IsActiveAmount1")]
        public bool IsActiveAmount1 { set; get; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.Amount1")]
        public decimal Amount1 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.NameAmount2")]
        public string NameAmount2 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.IsActiveAmount2")]
        public bool IsActiveAmount2 { set; get; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.Amount2")]
        public decimal Amount2 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.NameAmount3")]
        public string NameAmount3{ get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.IsActiveAmount3")]
        public bool IsActiveAmount3 { set; get; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.Amount3")]
        public decimal Amount3 { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.AmountSource")]
        public int Amount1Source { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.AmountSource")]
        public int Amount2Source { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.AmountSource")]
        public int Amount3Source { get; set; }

        public List<SelectListItem> Amount1Sources { get; set; }
        public List<SelectListItem> Amount2Sources { get; set; }
        public List<SelectListItem> Amount3Sources { get; set; }

        public List<CustumerToChange> CustumerToChange { get; set; }
    }

    public class CustumerToChange
    {
        public int Delay { get; set; }
        public int Size { get; set; }
    }
}