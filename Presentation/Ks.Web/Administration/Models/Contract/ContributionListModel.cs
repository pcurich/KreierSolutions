using System;
using System.ComponentModel.DataAnnotations;
using Ks.Web.Framework;

namespace Ks.Admin.Models.Contract
{
    public class ContributionListModel
    {
        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.SearchLetter")]
        [UIHint("Int32Nullable")]
        public int? SearchLetter { get; set; }
        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.SearchDni")]
        public string SearchDni { get; set; }
        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.SearchAdmCode")]
        public string SearchAdmCode { get; set; }
        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.SearchDateCreatedFrom")]
        [UIHint("DateNullable")]
        public DateTime? SearchDateCreatedFrom { get; set; }
        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.SearchDateCreatedTo")]
        [UIHint("DateNullable")]
        public DateTime? SearchDateCreatedTo { get; set; }
        [KsResourceDisplayName("Admin.Contract.Contribution.Fields.IsActive")]
        public bool IsActive { get; set; }
    }
}