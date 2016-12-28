using System;
using System.Linq;
using System.Web.Mvc;
using Ks.Admin.Extensions;
using Ks.Admin.Models.Contract;
using Ks.Core;
using Ks.Core.Domain.Contract;
using Ks.Services.Contract;
using Ks.Services.Helpers;
using Ks.Services.Localization;
using Ks.Services.Logging;
using Ks.Services.Security;
using Ks.Web.Framework;
using Ks.Web.Framework.Kendoui;

namespace Ks.Admin.Controllers
{
    public class ReturnPaymentController : BaseAdminController
    {
        #region Fields

        private readonly IReturnPaymentService _returnPaymentService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructor

        public ReturnPaymentController(IReturnPaymentService returnPaymentService, IPermissionService permissionService, ILocalizationService localizationService, ICustomerActivityService customerActivityService, IDateTimeHelper dateTimeHelper, IWorkContext workContext)
        {
            _returnPaymentService = returnPaymentService;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _customerActivityService = customerActivityService;
            _dateTimeHelper = dateTimeHelper;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturns))
                return AccessDeniedView();

            var model = new ReturnPaymentListModel
            {
                States = ReturnPaymentState.Aprobado.ToSelectList().ToList(),
                Types = ReturnPaymentType.Aportacion.ToSelectList().ToList()
            };

            model.Types.Insert(0, new SelectListItem { Text = "-------------------------", Value = "0" });
            model.States.Insert(0, new SelectListItem { Text = "-------------------------", Value = "0" });

            return View(model);
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command, ReturnPaymentListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturns))
                return AccessDeniedView();

            var returnPayment = _returnPaymentService.SearchReturnPayment(model.SearchDni, model.SearchAdmCode, model.SearchTypeId, model.PaymentNumber, command.Page, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = returnPayment.Select(x =>
                {
                    var toModel = x.ToModel();
                    toModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, TimeZoneInfo.Utc);
                    toModel.UpdatedOn = _dateTimeHelper.ConvertToUserTime(x.UpdatedOnUtc, TimeZoneInfo.Utc);
                    toModel.StateName = Enum.GetName(typeof(ReturnPaymentState), x.StateId);
                    toModel.ReturnPaymentTypeName = Enum.GetName(typeof(ReturnPaymentType), x.ReturnPaymentTypeId);
                    return toModel;
                }),
                Total = returnPayment.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        #endregion
    }
}