using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Customers
{
    public class ContributionBenefitModel : BaseKsEntityModel
    {
 
        public ContributionBenefitModel()
        {
            BenefitModels = new List<SelectListItem>();
            Banks= new List<SelectListItem>();
            RelaTionShips = new List<SelectListItem>();
        }

        public List<SelectListItem> Banks { get; set; }
        public List<SelectListItem> RelaTionShips { get; set; }

        public int ContributionId { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.BenefitId")]
        public int BenefitId { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.BenefitId")]
        public string BenefitName { get; set; }
        public int CustomerId { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.AmountBaseOfBenefit")]
        [UIHint("Decimal")]
        public decimal AmountBaseOfBenefit { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.YearInActivity")]
        public int YearInActivity { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.TabValue")]
        public double TabValue { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.Discount")]
        public double Discount { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.TotalToPay")]
        public decimal TotalToPay { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.TotalContributionCaja")]
        public decimal TotalContributionCaja { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.TotalContributionCopere")]
        public decimal TotalContributionCopere { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.TotalPersonalPayment")]
        public decimal TotalPersonalPayment { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.ReserveFund")]
        public decimal ReserveFund { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.TotalReationShip")]
        public int TotalReationShip { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.CreatedOn")]
        public DateTime CreatedOn { get; set; }
        public List<SelectListItem> BenefitModels { get; set; }
 

        #region Customer

        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.CustomerCompleteName")]
        public string CustomerCompleteName { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.CustomerAdmCode")]
        public string CustomerAdmCode { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.CustomerDni")]
        public string CustomerDni { get; set; }

        #endregion

        #region Nested

        public class ContributionBenefitCheckModel : BaseKsEntityModel
        {
            public string CompleteName { get; set; }
            public string Dni { get; set; }
            public string RelationShip { get; set; }
            public double Ratio { get; set; }
            public decimal AmountToPay { get; set; }
            public string AccountNumber { get; set; }
            public string BankName { get; set; }
            public List<SelectListItem> Banks { get; set; }
            public string CheckNumber { get; set; }

            public DateTime CreatedOn { get; set; }
            public DateTime UpdateOn { get; set; }
        }

        #endregion
    }
}