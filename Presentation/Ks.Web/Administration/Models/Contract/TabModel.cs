using System;
using System.Collections.Generic;
using Ks.Core.Domain.Contract;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Contract
{
    public class TabModel : BaseKsEntityModel
    {
        [KsResourceDisplayName("Admin.Configuration.Tabs.Fields.Name")]
        public string Name { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Tabs.Fields.AmountBase")]
        public decimal AmountBase { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Tabs.Fields.IsActive")]
        public bool IsActive { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Tabs.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Tabs.Fields.UpdatedOn")]
        public DateTime UpdatedOn { get; set; }

       public List<TabDetailModel> TabDetailModels { get;set; }
    }
}