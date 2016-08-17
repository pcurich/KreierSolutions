using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Logging
{
    public partial class LogListModel : BaseKsModel
    {
        public LogListModel()
        {
            AvailableLogLevels = new List<SelectListItem>();
        }

        [KsResourceDisplayName("Admin.System.Log.List.CreatedOnFrom")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnFrom { get; set; }

        [KsResourceDisplayName("Admin.System.Log.List.CreatedOnTo")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnTo { get; set; }

        [KsResourceDisplayName("Admin.System.Log.List.Message")]
        [AllowHtml]
        public string Message { get; set; }

        [KsResourceDisplayName("Admin.System.Log.List.LogLevel")]
        public int LogLevelId { get; set; }


        public IList<SelectListItem> AvailableLogLevels { get; set; }
    }
}