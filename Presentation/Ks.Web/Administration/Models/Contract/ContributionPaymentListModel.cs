using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ks.Web.Framework;

namespace Ks.Admin.Models.Contract
{
    public class ContributionPaymentListModel
    {
        public ContributionPaymentListModel()
        {
            States = new List<SelectListItem>();
            Types = new List<SelectListItem>();
            Banks = new List<SelectListItem>();
        }

        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.Number")]
        public int Number { get; set; }
        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.State")]
        public int StateId { get; set; }
        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.BankName")]
        public string BankName { get; set; }
        
        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.Type")]
        public int Type { get; set; }

        public List<SelectListItem> States { get; set; }
        public List<SelectListItem> Types { get; set; }
        public List<SelectListItem> Banks { get; set; }


    }
}