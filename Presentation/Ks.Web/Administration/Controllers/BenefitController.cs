using System;
using System.Linq;
using System.Web.Mvc;
using Ks.Admin.Extensions;
using Ks.Admin.Models.Contract;
using Ks.Core;
using Ks.Core.Domain.Contract;
using Ks.Services.Contract;
using Ks.Services.Customers;
using Ks.Services.Helpers;
using Ks.Services.Localization;
using Ks.Services.Logging;
using Ks.Services.Security;
using Ks.Web.Framework;
using Ks.Web.Framework.Controllers;
using Ks.Web.Framework.Kendoui;

namespace Ks.Admin.Controllers
{
    public class BenefitController : BaseAdminController
    {
        #region Fields

        private readonly IBenefitService _benefitService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructor

        public BenefitController(IBenefitService benefitService,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            ICustomerActivityService customerActivityService,
            IDateTimeHelper dateTimeHelper,
            IWorkContext workContext)
        {
            _benefitService = benefitService;
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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBenefits))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBenefits))
                return AccessDeniedView();

            var benefits = _benefitService.GetAllBenefits();
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

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBenefits))
                return AccessDeniedView();
            var model = new BenefitModel
            {
                IsActive = true,
                BenefitTypes = BenefitType.Auxilio.ToSelectList().ToList()
            };
            model.BenefitTypes.Insert(0, new SelectListItem { Text = "--------------------", Value = "0" });
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(BenefitModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBenefits))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var benefit = model.ToEntity();
                benefit.CreatedOnUtc = DateTime.UtcNow;
                benefit.UpdatedOnUtc = DateTime.UtcNow;

                _benefitService.InsertBenefit(benefit);

                //activity log
                _customerActivityService.InsertActivity("AddNewBenefit", _localizationService.GetResource("ActivityLog.AddNewTab"), benefit.Name, _workContext.CurrentCustomer.GetFullName());

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Tabs.Added"));

                return continueEditing ? RedirectToAction("Edit", new { id = benefit.Id }) : RedirectToAction("List");
            }

            ErrorNotification(_localizationService.GetResource("Admin.Configuration.Tabs.Error"));

            return View(model);

        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBenefits))
                return AccessDeniedView();

            var benefit = _benefitService.GetBenefitById(id);
            if (benefit == null)
                //No tab found with the specified id
                return RedirectToAction("List");

            var model = benefit.ToModel();
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(benefit.CreatedOnUtc, DateTimeKind.Utc);
            model.UpdatedOn = _dateTimeHelper.ConvertToUserTime(benefit.UpdatedOnUtc, DateTimeKind.Utc);
            model.BenefitTypes = BenefitType.Auxilio.ToSelectList().ToList();
            model.BenefitTypes.Insert(0, new SelectListItem { Text = "--------------------", Value = "0" });
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(BenefitModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBenefits))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var entity = _benefitService.GetBenefitById(model.Id);

                entity = model.ToEntity(entity);
                entity.UpdatedOnUtc = DateTime.UtcNow;

                _benefitService.UpdateBenefit(entity);

                //activity log
                _customerActivityService.InsertActivity("EditBenefit", _localizationService.GetResource("ActivityLog.EditBenefit"), entity.Name, _workContext.CurrentCustomer.GetFullName());
                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Benefits.Updated"));


                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabIndex();
                    return RedirectToAction("Edit", new { id = entity.Id });
                }
                return RedirectToAction("List");
            }

            return View(model);

        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTabs))
                return AccessDeniedView();

            var benefit = _benefitService.GetBenefitById(id);
            if (benefit == null)
                //No category found with the specified id
                return RedirectToAction("List");

            _benefitService.DeleteBenefit(benefit);

            //activity log
            _customerActivityService.InsertActivity("DeleteBenefit", _localizationService.GetResource("ActivityLog.DeleteBenefit"), benefit.Name, _workContext.CurrentCustomer.GetFullName());

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Benefits.Deleted"));
            return RedirectToAction("List");
        }

        #endregion
    }
}