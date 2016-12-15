using Ks.Web.Framework.Mvc;
using System;
using Ks.Web.Framework;


namespace Ks.Admin.Models.Contract
{
    public class ContributionBenefitBankModel : BaseKsEntityModel
    {
        public int ContributionBenefitId { get; set; }
        [KsResourceDisplayName("Admin.Configuration.ContributionBenefitBank.CompleteName")]
        public string CompleteName { get; set; }
        [KsResourceDisplayName("Admin.Configuration.ContributionBenefitBank.Dni")]
        public string Dni { get; set; }
        [KsResourceDisplayName("Admin.Configuration.ContributionBenefitBank.RelationShip")]
        public string RelationShip { get; set; }
        public int RelationShipId { get; set; }
        [KsResourceDisplayName("Admin.Configuration.ContributionBenefitBank.AccountNumber")]
        public string AccountNumber { get; set; }
        [KsResourceDisplayName("Admin.Configuration.ContributionBenefitBank.Bank")]
        public string Bank { get; set; }
        public int BankId { get; set; }
        [KsResourceDisplayName("Admin.Configuration.ContributionBenefitBank.CheckNumber")]
        public string CheckNumber { get; set; }
        [KsResourceDisplayName("Admin.Configuration.ContributionBenefitBank.Ratio")]
        public double Ratio { get; set; }
        [KsResourceDisplayName("Admin.Configuration.ContributionBenefitBank.AmountToPay")]
        public decimal AmountToPay { get; set; }
        [KsResourceDisplayName("Admin.Configuration.ContributionBenefitBank.CreatedOn")]
        public DateTime CreatedOn { get; set; }
    }
}