using System;
using System.Collections.Generic;
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
    public class LoanModel : BaseKsEntityModel
    {
        public LoanModel()
        {
            Periods = new List<SelectListItem>();
            Months = new List<SelectListItem>();
            Years = new List<SelectListItem>();
            States=new List<SelectListItem>();
            Banks= new List<SelectListItem>();
        }

        #region Customer
        
        public int CustomerId { get; set; }

        [KsResourceDisplayName("Admin.Contract.Loan.Fields.CustomerCompleteName")]
        [AllowHtml]
        public string CustomerCompleteName { get; set; }

        [KsResourceDisplayName("Admin.Contract.Loan.Fields.CustomerDni")]
        [AllowHtml]
        public string CustomerDni { get; set; }

        [KsResourceDisplayName("Admin.Contract.Loan.Fields.CustomerAdmCode")]
        [AllowHtml]
        public string CustomerAdmCode { get; set; }

        #endregion

        [KsResourceDisplayName("Admin.Contract.Loan.Fields.TotalOfCycle")]
        public int TotalOfCycle { get; set; }
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.LoanAmount")]
        [UIHint("Decimal")]
        public decimal LoanAmount { get; set; }
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.CashFlow")]
        [UIHint("Decimal")]
        public decimal CashFlow { get; set; }
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.Period")]
        public int Period { get; set; }
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.Active")]
        public bool Active { get; set; }
        public bool IsPostBack { get; set; }
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.LoanNumber")]
        public int LoanNumber { get; set; }
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.Tea")]
        public double Tea { get; set; }
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.Safe")]
        public double Safe { get; set; }
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.TotalFeed")]
        public decimal TotalFeed { get; set; } // Amount*Period*Tea/12    
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.TotalSafe")]
        public decimal TotalSafe { get; set; } // Amount*Safe     
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.MonthlyQuota")]
        public decimal MonthlyQuota { get; set; } // (Amount +TotalFeed)/Period
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.TotalAmount")]
        public decimal TotalAmount { get; set; } // Amount +  TotalFeed   
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.TotalToPay")]
        [UIHint("Decimal")]
        public decimal TotalToPay { get; set; } // Amount-TotalSafe
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.TotalPayed")]
        [UIHint("Decimal")]
        public decimal TotalPayed { get; set; }
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.IsAuthorized")]
        public bool IsAuthorized { get; set; }
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.AccountNumber")]
        public string AccountNumber { get; set; }
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.BankName")]
        public string BankName { get; set; }

        public List<SelectListItem> Banks { get; set; }

        [KsResourceDisplayName("Admin.Contract.Loan.Fields.CheckNumber")]
        public string CheckNumber { get; set; }
        public StateActivityModel StateActivityModels { get; set; } //SyngleSignature Warranty
        public CashFlowModel CashFlowModels { get; set; }
        public PreCashFlowModel PreCashFlow { get; set; }
        public List<SelectListItem> Periods { get; set; }

        [KsResourceDisplayName("Admin.Contract.Loan.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.ApprovalOn")]
        public DateTime? ApprovalOn  { get; set; }
        public string StateName { get; set; }

        [KsResourceDisplayName("Admin.Contract.Loan.Fields.UpdatedOn")]
        public DateTime? UpdatedOn { get; set; }

        public int StateId { get; set; }
        public List<SelectListItem> States { get; set; }

        [KsResourceDisplayName("Admin.Contract.Loan.Fields.Day")]
        public int Day { get; set; }
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.Month")]
        public int MonthId { get; set; }
        public List<SelectListItem> Months { get; set; }
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.Year")]
        public int YearId { get; set; }
        public List<SelectListItem> Years { get; set; }

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
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.LoanAmount")]
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
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.IsAuthorized")]
        public bool IsAuthorized { get; set; }
        public string StateName { get; set; }
        }
}