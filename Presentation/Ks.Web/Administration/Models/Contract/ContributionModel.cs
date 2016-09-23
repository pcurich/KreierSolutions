using System;
using FluentValidation.Attributes;

using Ks.Admin.Validators.Contract;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Contract
{
    [Validator(typeof(ContributionValidator))]
    public class ContributionModel : BaseKsEntityModel
    {
        public int CustomerId { get; set; }
        public string CustomerCompleteName { get; set; }
        public string CustomerDni { get; set; }
        public string CustomerAdmCode { get; set; }

        public int LetterNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public decimal AmountTotal { get; set; }
        public bool Active { get; set; }
 
    }
}