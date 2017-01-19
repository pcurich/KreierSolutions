using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ks.Admin.Extensions;
using Ks.Admin.Models.Logging;
using Ks.Admin.Models.Messages;
using Ks.Core;
using Ks.Services.Customers;
using Ks.Services.Helpers;
using Ks.Services.Messages;
using Ks.Services.Security;
using Ks.Web.Framework.Kendoui;
using Ks.Core.Domain.Messages;

namespace Ks.Admin.Controllers
{

    public partial class HomeController : BaseAdminController
    {
        #region Fields

        private readonly IWorkFlowService _workFlowService;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion

        #region Constructor

        public HomeController(IWorkFlowService workFlowService, ICustomerService customerService, IWorkContext workContext, IDateTimeHelper dateTimeHelper)
        {
            _workFlowService = workFlowService;
            _customerService = customerService;
            _workContext = workContext;
            _dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Methods
 
        public ActionResult Index()
        {
            var model = new WorkFlowListModel
            {
                Types = Web.Framework.Extensions.GetDescriptions(typeof(WorkFlowType)),
                States = new List<SelectListItem>
                {
                    new SelectListItem{Value = "0", Text = "---------------------"},
                    new SelectListItem{Value = "1", Text = "Atendido"},
                    new SelectListItem{Value = "2", Text = "Por Atender"},
                }
            };
            model.Types.Insert(0, new SelectListItem { Value = "0", Text = "---------------------" });
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(DataSourceRequest command, WorkFlowListModel model)
        {
            var workFlows = _workFlowService.GetWorkFlowByRoles(_workContext.CurrentCustomer.CustomerRoles, 
                model.SearchStartDate,model.SearchEndDate,model.TypeId,model.StateId,model.EntityId,
                  command.Page - 1, command.PageSize);


            var workFlowModel = workFlows.Select(x =>
            {
                var toModel = x.ToModel();
                toModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
                toModel.UpdatedOn = _dateTimeHelper.ConvertToUserTime(x.UpdatedOnUtc, DateTimeKind.Utc);
                toModel.CustomerCreatedName = _customerService.GetCustomerById(x.CustomerCreatedId).GetFullName();
                return toModel;

            });
            var gridModel = new DataSourceResult
            {
                Data = workFlowModel,
                Total = workFlows.TotalCount
            };

            return Json(gridModel);
        }

        public ActionResult Attend(int id)
        {
            var workFlow=_workFlowService.GetWorkFlowById(id);
            workFlow.Active = false;
            _workFlowService.UpdateWorkFlow(workFlow);
            return RedirectToAction("Index");
        }

        #endregion
    }
}