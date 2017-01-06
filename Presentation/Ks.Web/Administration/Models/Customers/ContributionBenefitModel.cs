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
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.NumberOfLiquidation")]
        public int NumberOfLiquidation { get;set; }

        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.AmountBaseOfBenefit")]
        [UIHint("Decimal")]
        public decimal AmountBaseOfBenefit { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.YearInActivity")]
        public int YearInActivity { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.TabValue")]
        public double TabValue { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.Discount")]
        public double Discount { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.SubTotalToPay")]
        public decimal SubTotalToPay { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.TotalContributionCaja")]
        public decimal TotalContributionCaja { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.TotalContributionCopere")]
        public decimal TotalContributionCopere { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.TotalContributionPersonalPayment")]
        public decimal TotalContributionPersonalPayment { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.ReserveFund")]
        public decimal ReserveFund { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.TotalLoan")]
        public int TotalLoan { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.TotalLoanToPay")]
        public decimal TotalLoanToPay { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.TotalToPay")]
        public decimal TotalToPay { get; set; }
        public string CustomField1 { get; set; }
        public string CustomValue1 { get; set; }
        public string CustomField2 { get; set; }
        public string CustomValue2 { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.TotalReationShip")]
        public int TotalReationShip { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.CreatedOn")]
        public DateTime CreatedOn { get; set; }
        public bool Active { get; set;}
        public List<SelectListItem> BenefitModels { get; set; }
 

        #region Customer

        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.CustomerCompleteName")]
        public string CustomerCompleteName { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.CustomerAdmCode")]
        public string CustomerAdmCode { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.CustomerDni")]
        public string CustomerDni { get; set; }
        [KsResourceDisplayName("Admin.Customers.Customers.Benefit.ContributionStart")]
        public DateTime ContributionStart { get; set; }

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