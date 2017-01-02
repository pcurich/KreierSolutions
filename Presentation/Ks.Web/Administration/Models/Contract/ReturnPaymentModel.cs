using System;
using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Ks.Admin.Validators.Contract;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Contract
{
    [Validator(typeof(ReturnPaymentValidator))]
    public class ReturnPaymentModel : BaseKsEntityModel
    {
        public ReturnPaymentModel()
        {
            Banks= new List<SelectListItem>();
            States= new List<SelectListItem>();
        }

        [KsResourceDisplayName("Admin.Contract.ReturnPayment.SearchType")]
        public string ReturnPaymentTypeName { get; set; }
        public int ReturnPaymentTypeId { get; set; }

        [KsResourceDisplayName("Admin.Contract.ReturnPayment.PaymentNumber")]
        public int PaymentNumber { get; set; }
        [KsResourceDisplayName("Admin.Contract.ReturnPayment.AmountToPay")]
        public decimal AmountToPay { get; set; }

        [KsResourceDisplayName("Admin.Contract.ReturnPayment.CustomerName")]
        public string CustomerName { get; set; }
        [KsResourceDisplayName("Admin.Contract.ReturnPayment.SearchDni")]
        public string CustomerDni { get; set; }
        [KsResourceDisplayName("Admin.Contract.ReturnPayment.SearchAdmCode")]
        public string CustomerAdmCode { get; set; }
        public int CustomerId { get; set; }

        [KsResourceDisplayName("Admin.Contract.ReturnPayment.SearchState")]
        public string StateName { get; set; }
        public int StateId { get; set; }
        public List<SelectListItem> States { get; set; }

        [KsResourceDisplayName("Admin.Contract.ReturnPayment.BankName")]
        public string BankName { get; set; }
        public List<SelectListItem> Banks { get; set; }
        [KsResourceDisplayName("Admin.Contract.ReturnPayment.AccountNumber")]
        public string AccountNumber { get; set; }
        [KsResourceDisplayName("Admin.Contract.ReturnPayment.CheckNumber")]
        public string CheckNumber { get; set; }

        [KsResourceDisplayName("Admin.Contract.ReturnPayment.CreatedOn")]
        public DateTime CreatedOn { get; set; }
        [KsResourceDisplayName("Admin.Contract.ReturnPayment.UpdatedOn")]
        public DateTime UpdatedOn { get; set; }
        
    }
}