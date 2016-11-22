using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ks.Web.Framework;
using System.Web.Mvc;

namespace Ks.Admin.Models.Contract
{
    public class LoanListModel
    {
        public LoanListModel()
        {
            States = new List<SelectListItem>();
        }

        [KsResourceDisplayName("Admin.Contract.Loan.Fields.SearchLoanNumber")]
        [UIHint("Int32Nullable")]
        public int? SearchLoanNumber { get; set; }
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.SearchDni")]
        public string SearchDni { get; set; }
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.SearchAdmCode")]
        public string SearchAdmCode { get; set; }
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.IsActive")]
        public bool IsActive { get; set; }
        [KsResourceDisplayName("Admin.Contract.Loan.Fields.State")]
        public int StateId { get; set; }
        public List<SelectListItem> States { get; set; } 
    }
}