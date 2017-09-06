using System;
using System.Collections.Generic;
using System.Linq;
using Ks.Core;
using Ks.Core.Data;
using Ks.Core.Domain.Customers;
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

        public virtual IPagedList<WorkFlow> GetWorkFlowByRoles(ICollection<CustomerRole> systemRole,
            DateTime? searchStartDate = null, DateTime? searchEndDate = null, int typeId = 0, int stateId = 0,
            int entityId = 0, int pageIndex = 0, int pageSize = Int32.MaxValue)
        {

            if (systemRole == null || systemRole.Count == 0)
                return new PagedList<WorkFlow>(new List<WorkFlow>(), pageIndex, pageIndex);

            var roles = systemRole.Select(x => x.SystemName);

            var query = from wf in _workFlowRepository.Table
                        where roles.Contains(wf.SystemRoleApproval)
                        select wf;

            if (searchStartDate.HasValue && searchEndDate.HasValue)
            {
                query = query.Where(x => searchStartDate <= x.CreatedOnUtc);
                query = query.Where(x => x.CreatedOnUtc <= searchEndDate);
            }
            if (typeId != 0)
            {
                if (typeId == 1)
                    query = query.Where(x => x.EntityName == WorkFlowType.Contribution.ToString());

                if (typeId == 2)
                    query = query.Where(x => x.EntityName == WorkFlowType.Loan.ToString());

                if (typeId == 3)
                    query = query.Where(x => x.EntityName == WorkFlowType.Benefit.ToString());

            }//WorkFlowType.Contribution.ToString()


            if (stateId == 1)
                query = query.Where(x => x.Active == false);
            if (stateId == 2 || stateId == 0)
                query = query.Where(x => x.Active);

            if (entityId != 0 )
                query = query.Where(x => x.EntityId == entityId);

            query = query.OrderByDescending(x => x.Id);
            return new PagedList<WorkFlow>(query.ToList(), pageIndex, pageSize);
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

        public virtual WorkFlow GetWorkFlowByEntityId(int entityId=0, string systemRoleApproval="")
        {
            if (entityId == 0)
                return null;

            var query = from wf in _workFlowRepository.Table
                        where wf.EntityId == entityId
                        select wf;

            if (!string.IsNullOrEmpty(systemRoleApproval))
            {
                query = from wf in query
                        where wf.SystemRoleApproval == systemRoleApproval
                        select wf;
            }

            return query.FirstOrDefault();
        }

        public virtual List<WorkFlow> GetWorkFlowsByEntityId(int entityId = 0, string systemRoleApproval = "")
        {
            if (entityId == 0)
                return null;

            var query = from wf in _workFlowRepository.Table
                        where wf.EntityId == entityId
                        select wf;

            if (!string.IsNullOrEmpty(systemRoleApproval))
            {
                query = from wf in query
                        where wf.SystemRoleApproval == systemRoleApproval
                        select wf;
            }

            return query.ToList();
        }

        public virtual void UpdateWorkFlow(WorkFlow workFlow)
        {
            if (workFlow == null)
                return;

            _workFlowRepository.Update(workFlow);
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