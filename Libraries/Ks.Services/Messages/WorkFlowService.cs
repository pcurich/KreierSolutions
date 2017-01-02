using System;
using System.Collections.Generic;
using System.Linq;
using Ks.Core;
using Ks.Core.Data;
using Ks.Core.Domain.Messages;

namespace Ks.Services.Messages
{
    public class WorkFlowService : IWorkFlowService
    {
        private readonly IRepository<WorkFlow> _workFlowRepository;

        public WorkFlowService(IRepository<WorkFlow> workFlowRepository)
        {
            _workFlowRepository = workFlowRepository;
        }

        public virtual IPagedList<WorkFlow> GetWorkFlowByRole(int roleId, int pageIndex = 0, int pageSize = Int32.MaxValue)
        {
            if (roleId == 0)
                return new PagedList<WorkFlow>(new List<WorkFlow>(), pageIndex, pageIndex);

            var query = from wf in _workFlowRepository.Table
                        where wf.CustomerRoleId == roleId
                        select wf;

            return new PagedList<WorkFlow>(query.ToList(), pageIndex, pageIndex);
        }

        public virtual IPagedList<WorkFlow> GetWorkFlowByCustomer(int customerId, int pageIndex = 0, int pageSize = Int32.MaxValue)
        {
            if (customerId == 0)
                return new PagedList<WorkFlow>(new List<WorkFlow>(), pageIndex, pageIndex);

            var query = from wf in _workFlowRepository.Table
                        where wf.CustomerApprovalId == customerId
                        select wf;

            return new PagedList<WorkFlow>(query.ToList(), pageIndex, pageIndex);
        }

        public virtual WorkFlow GetWorkFlowById(int workFlowId)
        {
            if (workFlowId == 0)
                return null;

            var query = from wf in _workFlowRepository.Table
                        where wf.Id == workFlowId
                        select wf;

            return query.FirstOrDefault();
        }

        public virtual void InsertWorkFlow(WorkFlow workFlow)
        {
            if (workFlow == null)
                return;

            workFlow.Active = true;
            _workFlowRepository.Insert(workFlow);
        }

        public virtual void CloseWorkFlow(int entityId, string entityName, string systemRole)
        {
            if (entityId == 0 && string.IsNullOrEmpty(entityName) && string.IsNullOrEmpty(systemRole))
                return;

            var query = from wf in _workFlowRepository.Table
                        where wf.EntityId == entityId && wf.EntityName == entityName && wf.SystemRoleApproval == systemRole
                        select wf;

            var workFlow = query.FirstOrDefault();
            if (workFlow == null)
                return;

            workFlow.UpdatedOnUtc = DateTime.UtcNow;
            workFlow.Active = false;
            _workFlowRepository.Update(workFlow);
        }
    }
}