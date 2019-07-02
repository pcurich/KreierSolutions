using System;
using System.Linq;
using System.Web.Mvc;
using Ks.Admin.Extensions;
using Ks.Admin.Models.Systems;
using Ks.Core.Domain.System;
using Ks.Services.Configuration;
using Ks.Services.KsSystems;
using Ks.Services.Localization;
using Ks.Services.Security;
using Ks.Web.Framework.Controllers;
using Ks.Web.Framework.Kendoui;

namespace Ks.Admin.Controllers
{
    public class KsSystemController : BaseAdminController
    {
        #region Constructors

        public KsSystemController(IKsSystemService ksSystemService,
            ISettingService settingService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IPermissionService permissionService)
        {
            _ksSystemService = ksSystemService;
            _settingService = settingService;
            _languageService = languageService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _permissionService = permissionService;
        }

        #endregion

        #region Fields

        private readonly IKsSystemService _ksSystemService;
        private readonly ISettingService _settingService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Utilities

        [NonAction]
        protected virtual void PrepareLanguagesModel(KsSystemModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            //templates
            model.AvailableLanguages.Add(new SelectListItem
            {
                Text = "---",
                Value = "0"
            });
            var languages = _languageService.GetAllLanguages(true);
            foreach (var language in languages)
            {
                model.AvailableLanguages.Add(new SelectListItem
                {
                    Text = language.Name,
                    Value = language.Id.ToString()
                });
            }
        }

        #endregion

        #region Methods

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageKsSystem))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageKsSystem))
                return AccessDeniedView();

            var systemModels = _ksSystemService.GetAllKsSystems()
                .Select(x => x.ToModel())
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = systemModels,
                Total = systemModels.Count()
            };

            return Json(gridModel);
        }

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageKsSystem))
                return AccessDeniedView();

            var model = new KsSystemModel();
            //languages
            PrepareLanguagesModel(model);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(KsSystemModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageKsSystem))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var store = model.ToEntity();
                //ensure we have "/" at the end
                if (!store.Url.EndsWith("/"))
                    store.Url += "/";
                _ksSystemService.InsertKsSystem(store);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.KsSystems.Added"));
                return continueEditing ? RedirectToAction("Edit", new {id = store.Id}) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form

            //languages
            PrepareLanguagesModel(model);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageKsSystem))
                return AccessDeniedView();

            var system = _ksSystemService.GetKsSystemById(id);
            if (system == null)
                //No store found with the specified id
                return RedirectToAction("List");

            var model = system.ToModel();
            //languages
            PrepareLanguagesModel(model);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult Edit(KsSystemModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageKsSystem))
                return AccessDeniedView();

            var system = _ksSystemService.GetKsSystemById(model.Id);
            if (system == null)
                //No store found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                system = model.ToEntity(system);
                //ensure we have "/" at the end
                if (!system.Url.EndsWith("/"))
                    system.Url += "/";
                _ksSystemService.UpdateKsSystem(system);
               
                SuccessNotification(_localizationService.GetResource("Admin.Configuration.KsSystems.Updated"));
                return continueEditing ? RedirectToAction("Edit", new {id = system.Id}) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form

            //languages
            PrepareLanguagesModel(model);
            return View(model);
        }

        //[HttpPost]
        //public ActionResult Delete(int id)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageKsSystem))
        //        return AccessDeniedView();

        //    var system = _ksSystemService.GetKsSystemById(id);
        //    if (system == null)
        //        //No store found with the specified id
        //        return RedirectToAction("List");

        //    try
        //    {
        //        _ksSystemService.DeleteKsSystem(system);

        //        //when we delete a store we should also ensure that all "per store" settings will also be deleted
        //        var settingsToDelete = _settingService
        //            .GetAllSettings()
        //            .Where(s => s.Id == id)
        //            .ToList();
        //        foreach (var setting in settingsToDelete)
        //            _settingService.DeleteSetting(setting);
        //        //when we had two KsSystems and now have only one store, we also should delete all "per store" settings
        //        var allKsSystems = _ksSystemService.GetAllKsSystems();
        //        if (allKsSystems.Count == 1)
        //        {
        //            settingsToDelete = _settingService
        //                .GetAllSettings()
        //                .Where(s => s.KsSystemId == allKsSystems[0].Id)
        //                .ToList();
        //            foreach (var setting in settingsToDelete)
        //                _settingService.DeleteSetting(setting);
        //        }


        //        SuccessNotification(_localizationService.GetResource("Admin.Configuration.KsSystems.Deleted"));
        //        return RedirectToAction("List");
        //    }
        //    catch (Exception exc)
        //    {
        //        ErrorNotification(exc);
        //        return RedirectToAction("Edit", new {id = system.Id});
        //    }
        //}

        #endregion
    }
}