using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Ks.Admin.Extensions;
using Ks.Core;
using Ks.Services.Contract;
using Ks.Services.Localization;
using Ks.Services.Security;
using Ks.Web.Framework.Kendoui;

namespace Ks.Admin.Controllers
{
    public partial class TabController:BaseAdminController
    {
        #region Fields

        private readonly ITabService _tabService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Constructor

        public TabController(ITabService tabService, ILocalizationService localizationService, IPermissionService permissionService, IWebHelper webHelper)
        {
            _tabService = tabService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _webHelper = webHelper;
        }

        #endregion

        #region Methods
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var tabs = _tabService.GetAllTabs();
            var gridModel = new DataSourceResult
            {
                Data = tabs.Select(x => x.ToModel()),
                Total = tabs.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var tab = _tabService.GetTabById(id);
            if (tab == null)
                //No language found with the specified id
                return RedirectToAction("List");

            var model = tab.ToModel();
            return View(model);
        }


        [HttpPost]
        public ActionResult ListDetails(DataSourceRequest command, int tabId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLanguages))
                return AccessDeniedView();

            var tabDetails = _tabService.GetAllValues(tabId);
            var gridModel = new DataSourceResult
            {
                Data = tabDetails.Select(x => x.ToModel()),
                Total = tabDetails.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        #endregion

    }
}