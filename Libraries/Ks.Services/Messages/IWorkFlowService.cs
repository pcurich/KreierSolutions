using System;
using System.Collections.Generic;
using Ks.Core;
using Ks.Core.Domain.Customers;
using Ks.Core.Domain.Messages;

namespace Ks.Services.Messages
{
    public interface IWorkFlowService
    {
        IPagedList<WorkFlow> GetWorkFlowByRoles(ICollection<CustomerRole> systemRole, int pageIndex = 0, int pageSize = Int32.MaxValue);
        IPagedList<WorkFlow> GetWorkFlowByCustomer(int customerId, int pageIndex = 0, int pageSize = Int32.MaxValue);

        WorkFlow GetWorkFlowById(int workFlowId);

        void InsertWorkFlow(WorkFlow workFlow);
        void CloseWorkFlow(int entityId,string entityName, string systemRole );

    }
}