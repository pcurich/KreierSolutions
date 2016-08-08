using System.Web.Mvc;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Common
{
    public partial class UrlRecordModel : BaseKsEntityModel
    {
        [KsResourceDisplayName("Admin.System.SeNames.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [KsResourceDisplayName("Admin.System.SeNames.EntityId")]
        public int EntityId { get; set; }

        [KsResourceDisplayName("Admin.System.SeNames.EntityName")]
        public string EntityName { get; set; }

        [KsResourceDisplayName("Admin.System.SeNames.IsActive")]
        public bool IsActive { get; set; }

        [KsResourceDisplayName("Admin.System.SeNames.Language")]
        public string Language { get; set; }

        [KsResourceDisplayName("Admin.System.SeNames.Details")]
        public string DetailsUrl { get; set; }
    }
}