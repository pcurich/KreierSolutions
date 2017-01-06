﻿using System.Collections.Generic;
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

        public List<SelectListItem> States { get; set; }
        public List<SelectListItem> Types { get; set; }
        public List<SelectListItem> Banks { get; set; }

        public int LoanId { get; set; }
        public int CustomerId { get; set; }
    }
}