using System;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Messages
{
    public class WorkFlowModel : BaseKsEntityModel
    {
        [KsResourceDisplayName("Admin.Home.WorkFlow.Field.Title")]
        public string Title { get; set; }

        [KsResourceDisplayName("Admin.Home.WorkFlow.Field.Description")]
        public string Description { get; set; }

        public int CustomerCreatedId { get; set; }

        [KsResourceDisplayName("Admin.Home.WorkFlow.Field.CustomerCreatedName")]
        public string CustomerCreatedName { get; set; }

        [KsResourceDisplayName("Admin.Home.WorkFlow.Field.EntityId")]
        public int EntityId { get; set; }

        [KsResourceDisplayName("Admin.Home.WorkFlow.Field.EntityName")]
        public string EntityName { get; set; }

        [KsResourceDisplayName("Admin.Home.WorkFlow.Field.RequireSystemRole")]
        public bool RequireSystemRole { get; set; }

        [KsResourceDisplayName("Admin.Home.WorkFlow.Field.SystemRoleApproval")]
        public string SystemRoleApproval { get; set; }

        [KsResourceDisplayName("Admin.Home.WorkFlow.Field.RequireCustomer")]
        public bool RequireCustomer { get; set; }

        [KsResourceDisplayName("Admin.Home.WorkFlow.Field.CustomerApprovalId")]
        public int CustomerApprovalId { get; set; }

        [KsResourceDisplayName("Admin.Home.WorkFlow.Field.GoTo")]
        public string GoTo { get; set; }

        [KsResourceDisplayName("Admin.Home.WorkFlow.Field.Active")]
        public bool Active { get; set; }

        [KsResourceDisplayName("Admin.Home.WorkFlow.Field.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [KsResourceDisplayName("Admin.Home.WorkFlow.Field.UpdatedOn")]
        public DateTime UpdatedOn { get; set; }
    }
}