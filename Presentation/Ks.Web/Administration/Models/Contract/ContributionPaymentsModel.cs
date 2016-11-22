using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Ks.Admin.Validators.Customers;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Contract
{
    [Validator(typeof(ContributionPaymentsValidator))]
    public partial class ContributionPaymentsModel : BaseKsEntityModel
    {
        public ContributionPaymentsModel()
        {
            Banks= new List<SelectListItem>();
        }
        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.Number")]
        public int Number { get; set; }
        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.NumberOld")]
        public int NumberOld { get; set; }
        public int ContributionId { get; set; }
        public int CustomerId { get; set; }
        [UIHint("Decimal")]
        public decimal Amount1 { get; set; }
        [UIHint("Decimal")]
        public decimal Amount2 { get; set; }
        [UIHint("Decimal")]
        public decimal Amount3 { get; set; }
        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.AmountOld")]
        [UIHint("Decimal")]
        public decimal AmountOld { get; set; }
        [UIHint("Decimal")]
        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.AmountTotal")]
        public decimal AmountTotal { get; set; }

        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.AmountPayed")]
        public decimal AmountPayed { get; set; }

        public bool ShowAmountTotal { get;set; }
        public string NameAmount1 { get; set; }
        public bool IsActiveAmount1 { set; get; }
        public string NameAmount2 { get; set; }
        public bool IsActiveAmount2 { set; get; }
        public string NameAmount3 { get; set; }
        public bool IsActiveAmount3 { set; get; }


        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.ScheduledDateOn")]
        public DateTime ScheduledDateOn { get; set; }
        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.ProcessedDateOn")]
        public DateTime? ProcessedDateOn { get; set; }
        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.StateId")]
        public string State { get; set; }
        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.Type")]
        public string Type { get; set; }
        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.BankName")]
        public string BankName { get; set; }
        public List<SelectListItem> Banks { get; set; }
        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.AccountNumber")]
        public string AccountNumber { get; set; }
        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.TransactionNumber")]
        public string TransactionNumber { get; set; }
        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.Reference")]
        public string Reference { get; set; }
        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.Description")]
        public string Description { get; set; }
    }
}