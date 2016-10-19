﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Ks.Admin.Models.Settings;
using Ks.Admin.Validators.Contract;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Contract
{
    [Validator(typeof(LoanValidator))]
    public class LoanModel : BaseKsModel
    {
        public LoanModel()
        {
            Periods = new List<SelectListItem>();
        }
        public int CustomerId { get; set; }

        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.CustomerAdmCode")]
        public string AdminCode { get; set; }

        [KsResourceDisplayName("Admin.Contract.Loan.Fields.TotalOfCycle")]
        public int TotalOfCycle { get; set; }
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.Amount")]
        [UIHint("Decimal")]
        public decimal Amount { get; set; }
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.CashFlow")]
        [UIHint("Decimal")]
        public decimal CashFlow { get; set; }
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.Period")]
        public int Period { get; set; }
        public List<SelectListItem> Periods { get; set; }

        public bool IsPostBack { get; set; }
        public StateActivityModel StateActivityModels { get; set; } //SyngleSignature Warranty
        public CashFlowModel CashFlowModels { get; set; }

        public PreCashFlowModel PreCashFlow { get; set; }

    }

    public class CustomerWarranty
    {
        public int CustomerId { get; set; }
        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.CustomerCompleteName")]
        public string CompleteName { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.StateActivity.CashFlow.FindCustomer")]
        public string AdminCode { get; set; }
        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.IsContributor")]
        public bool IsContributor { get; set; }
        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.IsActive")]
        public bool IsActive { get; set; }

    }

    public class StateActivityModel
    {
        #region Clase

        [KsResourceDisplayName("Admin.Configuration.Settings.StateActivitySettings.IsEnable")]
        public bool IsEnable { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.StateActivitySettings.StateName")]
        public string StateName { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.StateActivitySettings.MinClycle")]
        public int MinClycle { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.StateActivitySettings.MaxClycle")]
        public int MaxClycle { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.StateActivitySettings.HasOnlySignature")]
        public bool HasOnlySignature { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.StateActivitySettings.MinAmountWithSignature")]
        public decimal MinAmountWithSignature { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.StateActivitySettings.MaxAmountWithSignature")]
        public decimal MaxAmountWithSignature { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.StateActivitySettings.HasWarranty")]
        public bool HasWarranty { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.StateActivitySettings.MinAmountWithWarranty")]
        public decimal MinAmountWithWarranty { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.StateActivitySettings.MaxAmountWithWarranty")]
        public decimal MaxAmountWithWarranty { get; set; }
        public CustomerWarranty CustomerWarranty { get; set; }
        
        #endregion
    }

    public class PreCashFlowModel
    {
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.Period")]
        public int Period { get; set; }
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.Amount")]
        [UIHint("Decimal")]
        public decimal Amount { get; set; }
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.Tea")]
        public double Tea { get; set; }
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.Safe")]
        public double Safe { get; set; }
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.TotalFeed")]
        [UIHint("Decimal")]
        public decimal TotalFeed { get; set; } // Amount*Period*Tea/12
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.TotalSafe")]
        [UIHint("Decimal")]
        public decimal TotalSafe { get; set; } // Amount*Safe
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.MonthlyFee")]
        [UIHint("Decimal")]
        public decimal MonthlyQuota { get; set; } // (Amount +TotalFeed)/Period
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.TotalAmount")]
        [UIHint("Decimal")]
        public decimal TotalAmount { get; set; } // Amount +  TotalFeed
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.TotalToPay")]
        [UIHint("Decimal")]
        public decimal TotalToPay { get; set; } // Amount-TotalSafe
    }
}