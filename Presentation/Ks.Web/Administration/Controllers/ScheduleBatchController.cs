using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ks.Admin.Extensions;
using Ks.Admin.Models.Batchs;
using Ks.Core.Domain.Batchs;
using Ks.Services.Batchs;
using Ks.Services.ExportImport;
using Ks.Services.Helpers;
using Ks.Services.Localization;
using Ks.Services.Logging;
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
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ScheduleBatchsSetting _scheduleBatchsSetting;
        private readonly IExportManager _exportManager;

        #endregion

        #region Constructors

        public ScheduleBatchController(IScheduleBatchService scheduleBatchService,
            IPermissionService permissionService,
            IDateTimeHelper dateTimeHelper, ILocalizationService localizationService,
            ICustomerActivityService customerActivityService,
            ScheduleBatchsSetting scheduleBatchsSetting, IExportManager exportManager)
        {
            this._scheduleBatchService = scheduleBatchService;
            this._permissionService = permissionService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._customerActivityService = customerActivityService;
            this._scheduleBatchsSetting = scheduleBatchsSetting;
            this._exportManager = exportManager;
        }

        #endregion

        #region Utility

        [NonAction]
        protected virtual ScheduleBatchModel PrepareScheduleBatchModel(ScheduleBatch batch)
        {
            var model = batch.ToModel();
            model.FrecuencyName = ((ScheduleBatchFrecuency)batch.FrecuencyId).ToString();
            if (batch.StartExecutionOnUtc != null)
                model.StartExecutionOn = _dateTimeHelper.ConvertToUserTime(batch.StartExecutionOnUtc.Value, DateTimeKind.Utc);
            if (batch.NextExecutionOnUtc != null)
                model.NextExecutionOn = _dateTimeHelper.ConvertToUserTime(batch.NextExecutionOnUtc.Value, DateTimeKind.Utc);
            if (batch.LastExecutionOnUtc.HasValue)
                model.LastExecutionOn = _dateTimeHelper.ConvertToUserTime(batch.LastExecutionOnUtc.Value, DateTimeKind.Utc);

            model.AvailableMonths = DateTime.Now.GetMonthsList(_localizationService);
            model.AvailableYears = DateTime.Now.GetYearsList(_localizationService);
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
            model.AvailableMonths = DateTime.Now.GetMonthsList(_localizationService);
            model.AvailableYears = DateTime.Now.GetYearsList(_localizationService);

            if (!batch.StartExecutionOnUtc.HasValue)
            {
                //only for the First Time
                var startExecution = DateTime.Now;
                if (batch.SystemName == _scheduleBatchsSetting.ServiceName1)
                {
                    startExecution = new DateTime(DateTime.Now.Year, DateTime.Now.Month, _scheduleBatchsSetting.DayOfProcess1);
                    if (DateTime.Now.Day > _scheduleBatchsSetting.DayOfProcess1)
                        startExecution = startExecution.AddMonths(1);
                }

                if (batch.SystemName == _scheduleBatchsSetting.ServiceName2)
                {
                    startExecution = new DateTime(DateTime.Now.Year, DateTime.Now.Month, _scheduleBatchsSetting.DayOfProcess2);
                    if (DateTime.Now.Day > _scheduleBatchsSetting.DayOfProcess1)
                        startExecution = startExecution.AddMonths(1);
                }

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

                var startExecution = new DateTime(year, month, day, hour, minute, second);

                model.StartExecutionOn = _dateTimeHelper.ConvertToUserTime(startExecution, TimeZoneInfo.Utc);
                if (batch.NextExecutionOnUtc != null)
                    model.NextExecutionOn = _dateTimeHelper.ConvertToUserTime(batch.NextExecutionOnUtc.Value, TimeZoneInfo.Utc);
                if (batch.LastExecutionOnUtc.HasValue)
                    model.LastExecutionOn = _dateTimeHelper.ConvertToUserTime(batch.LastExecutionOnUtc.Value, TimeZoneInfo.Utc);
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
                if (batch.Enabled)
                {
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

                    var startExecution = DateTime.Now;
                    if (batch.SystemName == _scheduleBatchsSetting.ServiceName1)
                    {
                        startExecution = new DateTime(DateTime.Now.Year, DateTime.Now.Month, _scheduleBatchsSetting.DayOfProcess1, hour, minute, second);
                        if (DateTime.Now.Day > _scheduleBatchsSetting.DayOfProcess1)
                            startExecution = startExecution.AddMonths(1);
                    }

                    if (batch.SystemName == _scheduleBatchsSetting.ServiceName2)
                    {
                        startExecution = new DateTime(DateTime.Now.Year, DateTime.Now.Month, _scheduleBatchsSetting.DayOfProcess2, hour, minute, second);
                        if (DateTime.Now.Day > _scheduleBatchsSetting.DayOfProcess1)
                            startExecution = startExecution.AddMonths(1);
                    }


                    batch.StartExecutionOnUtc = _dateTimeHelper.ConvertToUtcTime(startExecution);
                    batch.NextExecutionOnUtc = _dateTimeHelper.ConvertToUtcTime(startExecution);
                    batch.LastExecutionOnUtc = null;
                }

                _scheduleBatchService.UpdateBatch(batch);
                //_customerActivityService.InsertActivity()
                SuccessNotification(_localizationService.GetResource("Admin.System.ScheduleBatchs.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = batch.Id }) : RedirectToAction("List");
            }

            model.FrecuencyName = ((ScheduleBatchFrecuency)batch.FrecuencyId).ToString();
            model.AvailableFrecuencies = ScheduleBatchFrecuency.Diario.ToSelectList().ToList();
            model.AvailableFrecuencies.Insert(0, new SelectListItem { Value = "0", Text = "-----------" });
            model.AvailableMonths = DateTime.Now.GetMonthsList(_localizationService);
            model.AvailableYears = DateTime.Now.GetYearsList(_localizationService);

            return View(model);
        }


        public ActionResult ExportTxtPre(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleBatchs))
                return AccessDeniedView();

            var schedule = _scheduleBatchService.GetBatchById(id);
            if (schedule == null)
                //No  scheduleBatch  found with the specified id
                return RedirectToAction("List");
            try
            {
                schedule.Enabled = true;
                schedule.UpdateData = false;
                _scheduleBatchService.UpdateBatch(schedule);

                this.Server.ScriptTimeout=60*60;

                var txt = _exportManager.ExportScheduleTxt(schedule);
                string name = string.Empty;
                if (schedule.SystemName.Trim().ToUpper() == ("KS.BATCH.CAJA.OUT"))
                    name = string.Format("6008_{0}00", schedule.PeriodYear.ToString("0000") + schedule.PeriodMonth.ToString("00"));
                else
                    name = string.Format("8001_{0}00", schedule.PeriodYear.ToString("0000") + schedule.PeriodMonth.ToString("00"));
                name += "-previo.txt";
                return new TxtDownloadResult(txt, name);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }

        }

        public ActionResult ExportTxt(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleBatchs))
                return AccessDeniedView();

            var schedule = _scheduleBatchService.GetBatchById(id);
            if (schedule == null)
                //No  scheduleBatch  found with the specified id
                return RedirectToAction("List");
            try
            {
                schedule.Enabled = true;
                schedule.UpdateData = true;
                _scheduleBatchService.UpdateBatch(schedule);

                this.Server.ScriptTimeout = 60 * 60;

                var txt = _exportManager.ExportScheduleTxt(schedule);
                string name = string.Empty;
                if (schedule.SystemName == ("Ks.Batch.Caja.Out"))
                    name=string.Format("6008_{0}00", schedule.PeriodYear.ToString("0000") + schedule.PeriodMonth.ToString("00"));
                else
                    name = string.Format("8001_{0}00", schedule.PeriodYear.ToString("0000") + schedule.PeriodMonth.ToString("00"));
                name += ".txt";
                return new TxtDownloadResult(txt, name);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }

        }

        public ActionResult Download(int name)
        {
            return null;
        }

        public ActionResult CreateMerge(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleBatchs))
                return AccessDeniedView();

            var schedule = _scheduleBatchService.GetBatchById(id);
            if (schedule == null)
                //No  scheduleBatch  found with the specified id
                return RedirectToAction("List");

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

            try
            {
                if (schedule.SystemName == ("Ks.Batch.Caja.Out"))
                {
                    var path = @"C:\inetpub\wwwroot\Acmr\App_Data\Service\Ks.Batch.Merge\Read";
                    path = System.IO.Path.Combine(path, "CajaWakeUp.txt");
                    using (var myFile = System.IO.File.Create(path))
                    {
                        TextWriter tw = new StreamWriter(myFile);
                        tw.WriteLine("CajaWakeUp");
                        tw.Close();
                    }
                }
                else
                {
                    var path = @"C:\inetpub\wwwroot\Acmr\App_Data\Service\Ks.Batch.Merge\Read";
                    path = System.IO.Path.Combine(path, "CopereWakeUp.txt");
                    using (var myFile = System.IO.File.Create(path))
                    {
                        TextWriter tw = new StreamWriter(myFile);
                        tw.WriteLine("CopereWakeUp");
                        tw.Close();
                    }
                }
 

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.ScheduleBatch.Imported"));
                return RedirectToAction("Edit", new { id = schedule.Id });
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = schedule.Id });
            }

        }

        [HttpPost]
        public ActionResult ImportTxt(int id, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleBatchs))
                return AccessDeniedView();

            var schedule = _scheduleBatchService.GetBatchById(id);
            if (schedule == null)
                //No  scheduleBatch  found with the specified id
                return RedirectToAction("List");

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

            try
            {
                var file = Request.Files["importtxtfile"];
                if (file != null && file.ContentLength > 0)
                {
                    using (var sr = new StreamReader(file.InputStream, Encoding.UTF8))
                    {
                        string content = sr.ReadToEnd();
                        var path = Path.Combine(schedule.PathBase, schedule.FolderRead);
                        string destPath = Path.Combine(path, file.FileName);

                        if (!System.IO.Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        System.IO.File.WriteAllText(destPath, content);
                    }

                }
                else
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Common.UploadFile"));
                    return RedirectToAction("Edit", new { id = schedule.Id });
                }

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.ScheduleBatch.Imported"));
                return RedirectToAction("Edit", new { id = schedule.Id });
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = schedule.Id });
            }
        }

        [HttpPost]
        public ActionResult ListFiles(int id, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleBatchs))
                return AccessDeniedView();

            var schedule = _scheduleBatchService.GetBatchById(id);

            var models = Directory.GetFiles(System.IO.Path.Combine(schedule.PathBase, schedule.FolderMoveToDone)).Select(
                x =>
                {
                    var model = new ScheduleBatchFiles
                    {
                        Name = x
                    };
                    return model;
                }).ToList();

            var gridModel = new DataSourceResult
            {
                Data = models,
                Total = models.Count
            };

            return Json(gridModel);
        }

        #endregion
    }
}