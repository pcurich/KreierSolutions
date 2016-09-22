using System;
using FluentValidation.Attributes;
using Ks.Admin.Models.Customers;
using Ks.Admin.Validators.Contract;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Contract
{
    [Validator(typeof (ContributionValidator))]
    public class ContributionModel : BaseKsEntityModel
    {
        public int CustomerId { get; set; }
        public int LetterNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public decimal AmountTotal { get; set; }
        public bool Active { get; set; }
        public virtual CustomerModel CustomerModel { get; set; } 
    }
}