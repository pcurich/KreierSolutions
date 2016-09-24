using System;
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

        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.LetterNumber")]
        public int LetterNumber { get; set; }

        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.UpdatedOn")]
        public DateTime UpdatedOn { get; set; }

        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.AmountTotal")]
        public decimal AmountTotal { get; set; }

        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.Active")]
        public bool Active { get; set; }

    }
}