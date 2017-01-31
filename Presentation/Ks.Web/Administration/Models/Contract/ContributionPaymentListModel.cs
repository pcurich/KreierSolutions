using System;
using System.Collections.Generic;
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
        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.CustomerName")]
        public string CustomerName { get; set; }
        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.CustomerAdminCode")]
        public string CustomerAdminCode { get; set; }
        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.CustomerDni")]
        public string CustomerDni { get; set; }
        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.CustomerFrom")]
        public DateTime? CustomerFrom { get; set; }

        public List<SelectListItem> States { get; set; }
        public List<SelectListItem> Types { get; set; }
        public List<SelectListItem> Banks { get; set; }

        public int ContributionId { get; set; }
        public int CustomerId { get; set; }

        public string NameAmount1 { get; set; }
        public bool IsActiveAmount1 { set; get; }
        
        public string NameAmount2 { get; set; }
        public bool IsActiveAmount2 { set; get; }

        public string NameAmount3 { get; set; }
        public bool IsActiveAmount3 { set; get; }
        

    }
}