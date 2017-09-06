using System;
using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Ks.Admin.Validators.Contract;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Contract
{
    [Validator(typeof(CheckValidator))]
    public class CheckModel : BaseKsEntityModel
    {
        public CheckModel()
        {
            Source = new List<SelectListItem>
            {
                new SelectListItem {Value = "0", Text = "------------------------------"},
                new SelectListItem {Value = "1", Text = "Apoyo Social Económico"},
                new SelectListItem {Value = "2", Text = "Devoluciones"},
                new SelectListItem {Value = "3", Text = "Beneficios"}
            };
            Banks = new List<SelectListItem>();
        }

        public int EntityId { get; set; }

        [KsResourceDisplayName("Admin.Contract.Check.Fields.EntityName")]
        public string EntityName { get; set; }

        public int EntityTypeId { get; set; }

        [KsResourceDisplayName("Admin.Contract.Check.Fields.AccountNumber")]
        public string AccountNumber { get; set; }
        [KsResourceDisplayName("Admin.Contract.Check.Fields.BankName")]
        public string BankName { get; set; }
        [KsResourceDisplayName("Admin.Contract.Check.Fields.CheckNumber")]
        public string CheckNumber { get; set; }
        [KsResourceDisplayName("Admin.Contract.Check.Fields.Amount")]
        public decimal Amount { get; set; }
        [KsResourceDisplayName("Admin.Contract.Check.Fields.CheckStateName")]
        public string CheckStateName { get; set; }
        public int CheckStateId { get; set; }
        [KsResourceDisplayName("Admin.Contract.Check.Fields.Reason")]
        public string Reason { get; set; }
        [KsResourceDisplayName("Admin.Contract.Check.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        public List<SelectListItem> Source { get; set; }
        public List<SelectListItem> Banks { get; set; }
    }
}