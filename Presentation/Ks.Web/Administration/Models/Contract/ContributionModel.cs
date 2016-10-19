using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Ks.Admin.Validators.Contract;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Contract
{
    [Validator(typeof(ContributionValidator))]
    public class ContributionModel : BaseKsEntityModel
    {
        public ContributionModel()
        {
            MonthsList = new List<SelectListItem>();
            YearsList = new List<SelectListItem>();
            Banks = new List<SelectListItem>();
        }

        public int CustomerId { get; set; }

        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.CustomerCompleteName")]
        [AllowHtml]
        public string CustomerCompleteName { get; set; }

        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.CustomerDni")]
        [AllowHtml]
        public string CustomerDni { get; set; }

        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.CustomerAdmCode")]
        [AllowHtml]
        public string CustomerAdmCode { get; set; }

        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.AuthorizeDiscount")]
        public int AuthorizeDiscount { get; set; }

        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.DayOfPayment")]
        public int DayOfPayment { get; set; }

        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.CreatedOn")]
        public DateTime? CreatedOn { get; set; }

        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.UpdatedOn")]
        public DateTime? UpdatedOn { get; set; }

        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.AmountTotal")]
        [UIHint("Decimal")]
        public decimal AmountTotal { get; set; }

        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.TotalCycle")]
        public int TotalCycle { get; set; }

        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.Active")]
        public bool Active { get; set; }

        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.MonthId")]
        public int MonthId { get; set; }
        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.YearId")]
        public int YearId { get; set; }
        public List<SelectListItem> MonthsList { get; set; }
        public List<SelectListItem> YearsList { get; set; }

        
        public string NameAmount1 { get; set; }
        public bool IsActiveAmount1 { set; get; }
        public string NameAmount2 { get; set; }
        public bool IsActiveAmount2 { set; get; }
        public string NameAmount3 { get; set; }
        public bool IsActiveAmount3 { set; get; }

        public List<SelectListItem> Banks { get; set; }

    }
}