using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Contract
{
    public class LoanPaymentListModel : BaseKsEntityModel
    {
        public LoanPaymentListModel()
        {
            States = new List<SelectListItem>();
            Types = new List<SelectListItem>();
            Banks = new List<SelectListItem>();
        }

        [KsResourceDisplayName("Admin.Contract.LoanPayments.Fields.Quota")]
        public int Quota { get; set; }
        [KsResourceDisplayName("Admin.Contract.LoanPayments.Fields.State")]
        public int StateId { get; set; }
        [KsResourceDisplayName("Admin.Contract.LoanPayments.Fields.BankName")]
        public string BankName { get; set; }
        [KsResourceDisplayName("Admin.Contract.LoanPayments.Fields.Type")]
        public int Type { get; set; }
        [KsResourceDisplayName("Admin.Contract.LoanPayments.Fields.CustomerName")]
        public string CustomerName { get; set; }
        [KsResourceDisplayName("Admin.Contract.LoanPayments.Fields.CustomerAdminCode")]
        public string CustomerAdminCode { get; set; }
        [KsResourceDisplayName("Admin.Contract.LoanPayments.Fields.CustomerDni")]
        public string CustomerDni { get; set; }
        [KsResourceDisplayName("Admin.Contract.LoanPayments.Fields.CustomerFrom")]
        public DateTime? CustomerFrom { get; set; }
        public List<SelectListItem> States { get; set; }
        public List<SelectListItem> Types { get; set; }
        public List<SelectListItem> Banks { get; set; }

        public int LoanId { get; set; }
        public int CustomerId { get; set; }

        [KsResourceDisplayName("Admin.Contract.Loan.Fields.Active")]
        public bool Active { get; set; }

        [KsResourceDisplayName("Admin.Contract.Loan.Fields.IsAuthorized")]
        public bool IsAuthorized { get; set; }
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.TotalPayed")]
        public decimal TotalPayed { get; set; }
        [KsResourceDisplayName("Admin.Contract.ReturnPayment.CheckNumber")]
        public string CheckNumber { get; set; }
        [KsResourceDisplayName("Admin.Contract.ReturnPayment.LoanNumber")]
        public int LoanNumber { get; set; }
        [KsResourceDisplayName("Admin.Contract.ReturnPayment.LoanAmount")]
        public decimal LoanAmount { get; set; }
        [KsResourceDisplayName("Admin.Contract.ReturnPayment.MonthlyQuota")]
        public decimal MonthlyQuota { get; set; }
        [KsResourceDisplayName("Admin.Contract.ReturnPayment.TotalAmount")]
        public decimal TotalAmount { get; set; }

        


    }
}