using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Batchs
{
    public class ScheduleBatchModel : BaseKsEntityModel
    {
        public ScheduleBatchModel()
        {
            AvailableFrecuencies= new List<SelectListItem>();
        }

        [KsResourceDisplayName("Admin.System.ScheduleBatchs.Name")]
        [AllowHtml]
        public string Name { get; set; }

        public string SystemName { get; set; }

        [KsResourceDisplayName("Admin.System.ScheduleBatchs.PathRead")]
        [AllowHtml]
        public string PathRead { get; set; }
        [KsResourceDisplayName("Admin.System.ScheduleBatchs.PathLog")]
        [AllowHtml]
        public string PathLog { get; set; }
        [KsResourceDisplayName("Admin.System.ScheduleBatchs.PathMoveToDone")]
        [AllowHtml]
        public string PathMoveToDone { get; set; }
        [KsResourceDisplayName("Admin.System.ScheduleBatchs.PathMoveToError")]
        [AllowHtml]
        public string PathMoveToError { get; set; }

        [KsResourceDisplayName("Admin.System.ScheduleBatchs.Frecuency")]
        [AllowHtml]
        public string FrecuencyName { get; set; }
        [KsResourceDisplayName("Admin.System.ScheduleBatchs.Frecuency")]
        [AllowHtml]
        public int FrecuencyId { get; set; }

        public List<SelectListItem> AvailableFrecuencies { get; set; }

        [KsResourceDisplayName("Admin.System.ScheduleBatchs.StartExecutionOn")]
        [AllowHtml]
        [UIHint("DateTimeNullable")]
        public DateTime? StartExecutionOn { get; set; }
        [KsResourceDisplayName("Admin.System.ScheduleBatchs.NextExecutionOn")]
        [AllowHtml]
        [UIHint("DateTimeNullable")]
        public DateTime? NextExecutionOn { get; set; }
        [KsResourceDisplayName("Admin.System.ScheduleBatchs.LastExecutionOn")]
        [AllowHtml]
        [UIHint("DateTimeNullable")]
        public DateTime? LastExecutionOn { get; set; }
        [KsResourceDisplayName("Admin.System.ScheduleBatchs.Enabled")]
        [AllowHtml]
        public bool Enabled { get; set; }
    }
}