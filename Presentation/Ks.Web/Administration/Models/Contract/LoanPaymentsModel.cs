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
    [Validator(typeof(LoanPaymentValidator))]
    public partial class LoanPaymentsModel : BaseKsEntityModel
    {
        public LoanPaymentsModel()
        {
            Banks = new List<SelectListItem>();
        }

        public int LoanId { get; set; }
        public int CustomerId { get; set; }
        [KsResourceDisplayName("Admin.Contract.LoanPayments.Fields.Quota")]
        public int Quota { get; set; }
        [KsResourceDisplayName("Admin.Contract.LoanPayments.Fields.MonthlyQuota")]
        [UIHint("Decimal")]
        public decimal MonthlyQuota { get; set; }
        [KsResourceDisplayName("Admin.Contract.LoanPayments.Fields.MonthlyFee")]
        [UIHint("Decimal")]
        public decimal MonthlyFee { get; set; }
        [KsResourceDisplayName("Admin.Contract.LoanPayments.Fields.MonthlyCapital")]
        [UIHint("Decimal")]
        public decimal MonthlyCapital { get; set; }
        [KsResourceDisplayName("Admin.Contract.LoanPayments.Fields.MonthlyPayed")]
        [UIHint("Decimal")]
        public decimal MonthlyPayed { get; set; }
        [KsResourceDisplayName("Admin.Contract.LoanPayments.Fields.AmountToCancel")]
        [UIHint("Decimal")]
        public decimal AmountToCancel { get; set; }
        public int StateId { get; set; }
        public bool IsAutomatic { get; set; }
        [KsResourceDisplayName("Admin.Contract.LoanPayments.Fields.Type")]
        public string Type { get; set; }
        [KsResourceDisplayName("Admin.Contract.LoanPayments.Fields.BankName")]
        public string BankName { get; set; }
        public List<SelectListItem> Banks { get; set; }
        [KsResourceDisplayName("Admin.Contract.LoanPayments.Fields.AccountNumber")]
        public string AccountNumber { get; set; }
        [KsResourceDisplayName("Admin.Contract.LoanPayments.Fields.TransactionNumber")]
        public string TransactionNumber { get; set; }
        [KsResourceDisplayName("Admin.Contract.LoanPayments.Fields.Reference")]
        public string Reference { get; set; }
        [KsResourceDisplayName("Admin.Contract.LoanPayments.Fields.Description")]
        public string Description { get; set; }
        [KsResourceDisplayName("Admin.Contract.LoanPayments.Fields.ScheduledDateOn")]
        public DateTime ScheduledDateOn { get; set; }
        [KsResourceDisplayName("Admin.Contract.LoanPayments.Fields.ProcessedDateOn")]
        public DateTime? ProcessedDateOn { get; set; }
        [KsResourceDisplayName("Admin.Contract.LoanPayments.Fields.State")]
        public string State { get; set; }
    }
}