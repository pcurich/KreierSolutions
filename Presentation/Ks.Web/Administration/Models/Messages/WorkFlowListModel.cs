using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Messages
{
    public class WorkFlowListModel : BaseKsModel
    {
        public WorkFlowListModel()
        {
            Types = new List<SelectListItem>();
        }

        [KsResourceDisplayName("Admin.Home.WorkFlowList.List.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? SearchStartDate { get; set; }

        [KsResourceDisplayName("Admin.Home.WorkFlowList.List.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? SearchEndDate { get; set; }

        [KsResourceDisplayName("Admin.Home.WorkFlowList.List.Type")]
        public int TypeId { get; set; }
        public List<SelectListItem> Types { get; set; }

        [KsResourceDisplayName("Admin.Home.WorkFlowList.List.Entity")]
        public int EntityNumber { get; set; }

        public int EntityId { get; set; }

        [KsResourceDisplayName("Admin.Home.WorkFlowList.List.State")]
        public int StateId { get; set; }
        public List<SelectListItem> States { get; set; }
    }
}