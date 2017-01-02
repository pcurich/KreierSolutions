using System;
using Ks.Core.Domain.Customers;

namespace Ks.Core.Domain.Messages
{
    public class WorkFlow : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int CustomerCreatedId { get;set; }
        public int EntityId { get; set; }
        public string EntityName { get; set; }

        public bool RequireSystemRole { get; set; }
        public string SystemRoleApproval { get; set; }

        public bool RequireCustomer { get; set; }
        public int CustomerApprovalId { get; set; }

        public string GoTo { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}