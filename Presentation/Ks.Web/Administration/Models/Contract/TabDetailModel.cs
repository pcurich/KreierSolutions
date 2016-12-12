using System;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Contract
{
    public class TabDetailModel : BaseKsEntityModel
    {
        public int TabId { get;set; }

        [KsResourceDisplayName("Admin.Configuration.TabDetails.Fields.YearInActivity")]
        public int YearInActivity { get; set; }
        [KsResourceDisplayName("Admin.Configuration.TabDetails.Fields.TabValue")]
        public double TabValue { get; set; }

        public string TabValueS { get; set; }
        [KsResourceDisplayName("Admin.Configuration.TabDetails.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }
        [KsResourceDisplayName("Admin.Configuration.TabDetails.Fields.UpdatedOn")]
        public DateTime UpdatedOn { get; set; }
    }
}