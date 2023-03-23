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
        }

        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.TotalCycle")]
        public int TotalCycle { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.DayOfPaymentContribution")]
        public int DayOfPaymentContribution { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.CycleOfDelay")]
        public int CycleOfDelay { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.AmountMeta")]
        public decimal AmountMeta { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.AmountChargeCaja")]
        public decimal ChargeCaja { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.MaximumChargeCaja")]
        public decimal MaximumChargeCaja { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.AmountChargeCopere")]
        public decimal ChargeCopere { get; set; }

        //ya no se va a utilizar este valor
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.MaximumChargeCopere")]
        public decimal MaximumChargeCopere { get; set; }


        #region 1
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.NameAmount1")]
        public string NameAmount1 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.IsActiveAmount1")]
        public bool IsActiveAmount1 { set; get; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.Is1OnReport")]
        public bool Is1OnReport { set; get; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.Amount1")]
        public decimal Amount1 { get; set; }
        #endregion

        #region 2
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.NameAmount2")]
        public string NameAmount2 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.IsActiveAmount2")]
        public bool IsActiveAmount2 { set; get; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.Is2OnReport")]
        public bool Is2OnReport { set; get; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.Amount2")]
        public decimal Amount2 { get; set; }
        #endregion

        #region 3
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.NameAmount3")]
        public string NameAmount3 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.IsActiveAmount3")]
        public bool IsActiveAmount3 { set; get; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.Is3OnReport")]
        public bool Is3OnReport { set; get; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.Amount3")]
        public decimal Amount3 { get; set; }
        #endregion

        #region 4
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.NameAmount4")]
        public string NameAmount4 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.IsActiveAmount4")]
        public bool IsActiveAmount4{ set; get; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.Is4OnReport")]
        public bool Is4OnReport { set; get; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.Amount4")]
        public decimal Amount4 { get; set; }
        #endregion

        #region 5
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.NameAmount5")]
        public string NameAmount5 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.IsActiveAmount5")]
        public bool IsActiveAmount5 { set; get; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.Is5OnReport")]
        public bool Is5OnReport { set; get; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.Amount5")]
        public decimal Amount5 { get; set; }
        #endregion

        #region 6
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.NameAmount6")]
        public string NameAmount6 { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.IsActiveAmount6")]
        public bool IsActiveAmount6 { set; get; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.Is6OnReport")]
        public bool Is6OnReport { set; get; }
        [KsResourceDisplayName("Admin.Configuration.Settings.ContributionSettings.Amount6")]
        public decimal Amount6 { get; set; }
        #endregion
         
        public List<CustumerToChange> CustumerToChange { get; set; }
    }

    public class CustumerToChange
    {
        public int Delay { get; set; }
        public int Size { get; set; }
    }
}