using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ks.Web.Framework;

namespace Ks.Admin.Models.Contract
{
    public class ContributionListModel
    {
        public ContributionListModel()
        {
            States= new List<SelectListItem>();
        }

        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.SearchLetter")]
        [UIHint("Int32Nullable")]
        public int? SearchLetter { get; set; }
        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.SearchDni")]
        public string SearchDni { get; set; }
        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.SearchAdmCode")]
        public string SearchAdmCode { get; set; }
        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.IsActive")]
        public bool IsActive { get; set; }
        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.State")]
        public int StateId { get; set; }
        public List<SelectListItem> States { get; set; }


    }
}