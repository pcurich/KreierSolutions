using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Ks.Admin.Extensions;
using Ks.Admin.Models.Batchs;
using Ks.Admin.Models.Tasks;
using Ks.Core.Domain.Batchs;
using Ks.Core.Domain.Contract;
using Ks.Services.Batchs;
using Ks.Services.Helpers;
using Ks.Services.Localization;
using Ks.Services.Security;
using Ks.Web.Framework;
using Ks.Web.Framework.Controllers;
using Ks.Web.Framework.Kendoui;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Controllers
{
    public class ScheduleBatchController : BaseAdminController
    {
        #region Fields

        private readonly IScheduleBatchService _scheduleBatchService;
        private readonly IPermissionService _permissionService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;

        private readonly ContributionSettings _contributionSettings;

        #endregion

        #region Constructors

        public ScheduleBatchController(IScheduleBatchService scheduleBatchService,
            IPermissionService permissionService,
            IDateTimeHelper dateTimeHelper, ILocalizationService localizationService,
            ContributionSettings contributionSettings)
        {
            this._scheduleBatchService = scheduleBatchService;
            this._permissionService = permissionService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._contributionSettings = contributionSettings;
        }

        #endregion

        #region Utility

        [NonAction]
        protected virtual ScheduleBatchModel PrepareScheduleBatchModel(ScheduleBatch batch)
        {
            var model = batch.ToModel();
            model.FrecuencyName = ((ScheduleBatchFrecuency)batch.FrecuencyId).ToString();
            if (batch.StartExecutionOnUtc != null)
                model.StartExecutionOn = _dateTimeHelper.ConvertToUserTime(batch.StartExecutionOnUtc.Value, DateTimeKind.Local);
            if (batch.NextExecutionOnUtc != null)
                model.NextExecutionOn = _dateTimeHelper.ConvertToUserTime(batch.NextExecutionOnUtc.Value, DateTimeKind.Local);
            if (batch.LastExecutionOnUtc.HasValue)
                model.LastExecutionOn = _dateTimeHelper.ConvertToUserTime(batch.LastExecutionOnUtc.Value, DateTimeKind.Local);

            return model;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleBatchs))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleBatchs))
                return AccessDeniedView();

            var models = _scheduleBatchService.GetAllBatchs(true)
                .Select(PrepareScheduleBatchModel)
                .ToList();
            var gridModel = new DataSourceResult
            {
                Data = models,
                Total = models.Count
            };

            return Json(gridModel);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleBatchs))
                return AccessDeniedView();

            var batch = _scheduleBatchService.GetBatchById(id);
            if (batch == null)
                //No batch found with the specified id
                return RedirectToAction("List");

            var model = batch.ToModel();
            model.FrecuencyName = ((ScheduleBatchFrecuency)batch.FrecuencyId).ToString();
            model.AvailableFrecuencies = ScheduleBatchFrecuency.Diario.ToSelectList().ToList();
            model.AvailableFrecuencies.Insert(0, new SelectListItem { Value = "0", Text = "-----------" });

            if (!batch.StartExecutionOnUtc.HasValue)
            {
                //only for the First Time
                var startExecution = new DateTime(DateTime.Now.Year, DateTime.Now.Month, _contributionSettings.DayOfPayment);
                if (DateTime.Now.Day > _contributionSettings.DayOfPayment)
                    startExecution = startExecution.AddMonths(1);

                model.StartExecutionOn = startExecution;
                model.NextExecutionOn = startExecution.AddDays(batch.FrecuencyId);
                model.LastExecutionOn = startExecution;
            }
            else
            {
                var year = batch.StartExecutionOnUtc.Value.Year;
                var month = batch.StartExecutionOnUtc.Value.Month;
                var day = batch.StartExecutionOnUtc.Value.Day;
                var hour = batch.StartExecutionOnUtc.Value.Hour;
                var minute = batch.StartExecutionOnUtc.Value.Minute;
                var second = batch.StartExecutionOnUtc.Value.Second;

                var startExecution = new DateTime(year,month,day, hour, minute, second);

                model.StartExecutionOn = _dateTimeHelper.ConvertToUserTime(startExecution, DateTimeKind.Utc);
                if (batch.NextExecutionOnUtc != null)
                    model.NextExecutionOn = _dateTimeHelper.ConvertToUserTime(batch.NextExecutionOnUtc.Value, DateTimeKind.Utc);
                if (batch.LastExecutionOnUtc.HasValue)
                    model.LastExecutionOn = _dateTimeHelper.ConvertToUserTime(batch.LastExecutionOnUtc.Value, DateTimeKind.Utc);
            }

            return View(model);
        }

        [HttpPost]
        [ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult Edit(ScheduleBatchModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleBatchs))
                return AccessDeniedView();

            var batch = _scheduleBatchService.GetBatchById(model.Id);
            if (batch == null)
                //No batch found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                if (batch.Enabled && model.Enabled)
                {
                    model = batch.ToModel();
                    model.FrecuencyName = ((ScheduleBatchFrecuency)batch.FrecuencyId).ToString();
                    model.AvailableFrecuencies = ScheduleBatchFrecuency.Diario.ToSelectList().ToList();
                    model.AvailableFrecuencies.Insert(0, new SelectListItem { Value = "0", Text = "---------" });
                    if (!batch.StartExecutionOnUtc.HasValue)
                    {
                        //only for the First Time
                        var startExecution = new DateTime(DateTime.Now.Year, DateTime.Now.Month, _contributionSettings.DayOfPayment);
                        if (DateTime.Now.Day > _contributionSettings.DayOfPayment)
                            startExecution = startExecution.AddMonths(1);

                        model.StartExecutionOn = startExecution;
                        model.NextExecutionOn = startExecution.AddDays(batch.FrecuencyId);
                        model.LastExecutionOn = startExecution;
                    }
                    ErrorNotification(_localizationService.GetResource("Admin.System.ScheduleBatchs.Error"));
                    return RedirectToAction("Edit", new { id = batch.Id });
                }
                
                //Is not Enabled
                batch = model.ToEntity(batch);
                if (model.StartExecutionOn.HasValue)
                {
                    var hour = model.StartExecutionOn.Value.Hour;
                    var minute = model.StartExecutionOn.Value.Minute;
                    var second = model.StartExecutionOn.Value.Second;

                    var startExecution = new DateTime(
                        DateTime.Now.Year, 
                        DateTime.Now.Month, 
                        _contributionSettings.DayOfPayment,hour,minute,second);

                    if (DateTime.Now.Day > _contributionSettings.DayOfPayment)
                        startExecution = startExecution.AddMonths(1);

                    batch.StartExecutionOnUtc = _dateTimeHelper.ConvertToUtcTime(startExecution);
                    batch.NextExecutionOnUtc = batch.StartExecutionOnUtc.Value.AddDays(batch.FrecuencyId);
                    batch.LastExecutionOnUtc = batch.StartExecutionOnUtc;
                }
                    
                _scheduleBatchService.UpdateBatch(batch);

                SuccessNotification(_localizationService.GetResource("Admin.System.ScheduleBatchs.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = batch.Id }) : RedirectToAction("List");
            }
            return View(model);
        }
        #endregion
    }
}