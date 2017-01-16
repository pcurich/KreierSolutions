using System;
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
            return View( );
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command )
        { 
            var workFlows = _workFlowService.GetWorkFlowByRoles(_workContext.CurrentCustomer.CustomerRoles, command.Page - 1, command.PageSize);
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

        #endregion
    }
}