using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Ks.Admin.Extensions;
using Ks.Admin.Models.Contract;
using Ks.Core;
using Ks.Services.Contract;
using Ks.Services.Customers;
using Ks.Services.Helpers;
using Ks.Services.Localization;
using Ks.Services.Logging;
using Ks.Services.Security;
using Ks.Web.Framework.Controllers;
using Ks.Web.Framework.Kendoui;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Controllers
{
    public partial class TabController : BaseAdminController
    {
        #region Fields

        private readonly ITabService _tabService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructor

        public TabController(ITabService tabService,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            ICustomerActivityService customerActivityService,
            IDateTimeHelper dateTimeHelper,
            IWorkContext workContext)
        {
            _tabService = tabService;
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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTabs))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTabs))
                return AccessDeniedView();

            var tabs = _tabService.GetAllTabs();
            var gridModel = new DataSourceResult
            {
                Data = tabs.Select(x =>
                {
                    var model = x.ToModel();
                    model.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, TimeZoneInfo.Utc);
                    model.UpdatedOn = _dateTimeHelper.ConvertToUserTime(x.UpdatedOnUtc, TimeZoneInfo.Utc);
                    return model;
                }),
                Total = tabs.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTabs))
                return AccessDeniedView();
            var model = new TabModel { IsActive = true };
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(TabModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTabs))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var newTab = model.ToEntity();
                newTab.CreatedOnUtc = DateTime.UtcNow;
                newTab.UpdatedOnUtc = DateTime.UtcNow;

                if (newTab.IsActive)
                {
                    var tabs = _tabService.GetAllTabs();
                    foreach (var tab in tabs)
                    {
                        tab.IsActive = false;
                        tab.UpdatedOnUtc = DateTime.UtcNow;
                        _tabService.UpdateTab(tab);
                    }
                }
                _tabService.InsertTab(newTab);

                //activity log
                _customerActivityService.InsertActivity("AddNewTab", _localizationService.GetResource("ActivityLog.AddNewTab"), newTab.Name, _workContext.CurrentCustomer.GetFullName());

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Tabs.Added"));

                return continueEditing ? RedirectToAction("Edit", new { id = newTab.Id }) : RedirectToAction("List");
            }

            ErrorNotification(_localizationService.GetResource("Admin.Configuration.Tabs.Error"));

            return View(model);

        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTabs))
                return AccessDeniedView();

            var tab = _tabService.GetTabById(id);
            if (tab == null)
                //No tab found with the specified id
                return RedirectToAction("List");

            var model = tab.ToModel();
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(tab.CreatedOnUtc, DateTimeKind.Utc);
            model.UpdatedOn = _dateTimeHelper.ConvertToUserTime(tab.UpdatedOnUtc, DateTimeKind.Utc);

            if (model.Id>0)
                SaveSelectedTabIndex(1);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(TabModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTabs))
                return AccessDeniedView();

            var entity = _tabService.GetTabById(model.Id);
            if (ModelState.IsValid)
            {
                entity = model.ToEntity(entity);
                entity.UpdatedOnUtc = DateTime.UtcNow;

                if (entity.IsActive)
                {
                    var tabs = _tabService.GetAllTabs();
                    foreach (var tab in tabs)
                    {
                        tab.IsActive = false;
                        tab.UpdatedOnUtc = DateTime.UtcNow;
                        _tabService.UpdateTab(tab);
                    }
                    entity.IsActive = true;
                    _tabService.UpdateTab(entity);
                }
                else
                {
                    _tabService.UpdateTab(entity);
                }

                //activity log
                _customerActivityService.InsertActivity("EditTab", _localizationService.GetResource("ActivityLog.EditTab"), entity.Name, _workContext.CurrentCustomer.GetFullName());
                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Tabs.Updated"));
            }

            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabIndex();

                return RedirectToAction("Edit", new { id = entity.Id });
            }
            return RedirectToAction("List");

        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTabs))
                return AccessDeniedView();

            var tab = _tabService.GetTabById(id);
            if (tab == null)
                //No category found with the specified id
                return RedirectToAction("List");

            if (tab.IsActive || _tabService.GetAllValues().Count() == 1)
            {
                ErrorNotification(_localizationService.GetResource("Admin.Configuration.Tabs.ErrorDelete"));
                return RedirectToAction("Edit", new { id = tab.Id });
            }
            else
            {
                _tabService.DeleteTab(tab);
            }
            //activity log
            _customerActivityService.InsertActivity("DeleteTab", _localizationService.GetResource("ActivityLog.DeleteTab"), tab.Name, _workContext.CurrentCustomer.GetFullName());

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Tabs.Deleted"));
            return RedirectToAction("List");
        }

        #region TabDetails

        [HttpPost]
        public ActionResult ListDetails(DataSourceRequest command, int tabId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var tabDetails = _tabService.GetAllValues(tabId);
            var gridModel = new DataSourceResult
            {
                Data = tabDetails.Select(x =>
                {
                    var details = x.ToModel();
                    details.UpdatedOn = _dateTimeHelper.ConvertToUserTime(x.UpdatedOnUtc, DateTimeKind.Utc);
                    details.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
                    details.TabValueS = x.TabValue.ToString(CultureInfo.InvariantCulture);
                    return details;
                }).ToList(),
                Total = tabDetails.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [HttpPost]
        public ActionResult TabDetailUpdate([Bind(Exclude = "CreatedOn,UpdatedOn")] TabDetailModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTabs))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            model.TabValue = Convert.ToDouble(model.TabValueS);
            var tabDetail = _tabService.GetTabDetailById(model.Id);
            model.CreatedOn = tabDetail.CreatedOnUtc;
            model.UpdatedOn = DateTime.UtcNow;
            tabDetail = model.ToEntity(tabDetail);
            _tabService.UpdateTabDetail(tabDetail);

            _customerActivityService.InsertActivity("EditTabDetail", _localizationService.GetResource("ActivityLog.EditTab"), tabDetail.TabValue, _workContext.CurrentCustomer.GetFullName());

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult TabDetailAdd([Bind(Exclude = "Id,CreatedOn,UpdatedOn")] TabDetailModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTabs))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            model.TabValue = Convert.ToDouble(model.TabValueS);
            var tabDetail = model.ToEntity();
            tabDetail.CreatedOnUtc = DateTime.UtcNow;
            tabDetail.UpdatedOnUtc = DateTime.UtcNow;
            _tabService.InsertTabDetail(tabDetail);

            _customerActivityService.InsertActivity("AddNewTabDetail", _localizationService.GetResource("ActivityLog.AddNewTab"), tabDetail.TabValue, _workContext.CurrentCustomer.GetFullName());

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult TabDetailDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTabs))
                return AccessDeniedView();

            var tabDetail = _tabService.GetTabDetailById(id);
            if (tabDetail == null)
                throw new ArgumentException("No setting found with the specified id");
            _tabService.DeleteTabDetail(tabDetail);

            //activity log
            _customerActivityService.InsertActivity("DeleteTabDetail", _localizationService.GetResource("ActivityLog.DeleteTab"), tabDetail.TabValue, _workContext.CurrentCustomer.GetFullName());

            return new NullJsonResult();
        }

        #endregion

        #endregion

    }
}