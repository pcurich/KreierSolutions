using System.Collections.Generic;
using System.Web.Mvc;
using Ks.Web.Framework;

namespace Ks.Admin.Models.Contract
{
    public class ReturnPaymentListModel
    {
        public ReturnPaymentListModel()
        {
            States = new List<SelectListItem>();
            Types = new List<SelectListItem>();
        }

        [KsResourceDisplayName("Admin.Contract.ReturnPayment.SearchDni")]
        public string SearchDni { get; set; }
        [KsResourceDisplayName("Admin.Contract.ReturnPayment.SearchAdmCode")]
        public string SearchAdmCode { get; set; }
        [KsResourceDisplayName("Admin.Contract.ReturnPayment.SearchType")]
        public int SearchTypeId { get; set; }
        public List<SelectListItem> Types { get; set; }
        [KsResourceDisplayName("Admin.Contract.ReturnPayment.PaymentNumber")]
        public int PaymentNumber { get; set; }
        [KsResourceDisplayName("Admin.Contract.ReturnPayment.SearchState")]
        public int SearchStateId { get; set; }
        public List<SelectListItem> States { get; set; }

    }
}