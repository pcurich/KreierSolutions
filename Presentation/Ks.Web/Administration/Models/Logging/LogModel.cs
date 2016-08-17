using System;
using System.Web.Mvc;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Logging
{
    public partial class LogModel : BaseKsEntityModel
    {
        [KsResourceDisplayName("Admin.System.Log.Fields.LogLevel")]
        public string LogLevel { get; set; }

        [KsResourceDisplayName("Admin.System.Log.Fields.ShortMessage")]
        [AllowHtml]
        public string ShortMessage { get; set; }

        [KsResourceDisplayName("Admin.System.Log.Fields.FullMessage")]
        [AllowHtml]
        public string FullMessage { get; set; }

        [KsResourceDisplayName("Admin.System.Log.Fields.IPAddress")]
        [AllowHtml]
        public string IpAddress { get; set; }

        [KsResourceDisplayName("Admin.System.Log.Fields.Customer")]
        public int? CustomerId { get; set; }
        [KsResourceDisplayName("Admin.System.Log.Fields.Customer")]
        public string CustomerEmail { get; set; }

        [KsResourceDisplayName("Admin.System.Log.Fields.PageURL")]
        [AllowHtml]
        public string PageUrl { get; set; }

        [KsResourceDisplayName("Admin.System.Log.Fields.ReferrerURL")]
        [AllowHtml]
        public string ReferrerUrl { get; set; }

        [KsResourceDisplayName("Admin.System.Log.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }
    }
}