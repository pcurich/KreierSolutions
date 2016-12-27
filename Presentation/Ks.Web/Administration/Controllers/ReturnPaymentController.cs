using System;
using System.Linq;
using System.Web.Mvc;
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

            return View();
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturns))
                return AccessDeniedView();

            var benefits = _returnPaymentService.Search();
            var gridModel = new DataSourceResult
            {
                Data = benefits.Select(x =>
                {
                    var model = x.ToModel();
                    model.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, TimeZoneInfo.Utc);
                    model.UpdatedOn = _dateTimeHelper.ConvertToUserTime(x.UpdatedOnUtc, TimeZoneInfo.Utc);
                    model.BenefitTypes = BenefitType.Auxilio.ToSelectList().ToList();
                    model.BenefitTypes.Insert(0, new SelectListItem { Value = "0", Text = "" });
                    var firstOrDefault = BenefitType.Auxilio.ToSelectList()
                        .FirstOrDefault(r => r.Value == x.BenefitTypeId.ToString());
                    if (firstOrDefault != null)
                        model.BenefitTypeName =
                            firstOrDefault
                                .Text;

                    return model;
                }),
                Total = benefits.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        #endregion
    }
}