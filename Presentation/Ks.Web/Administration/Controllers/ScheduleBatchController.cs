using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using Ks.Admin.Extensions;
using Ks.Admin.Models.Batchs;
using Ks.Core.Domain.Batchs;
using Ks.Core.Domain.Customers;
using Ks.Core.Domain.Reports;
using Ks.Services.Batchs;
using Ks.Services.ExportImport;
using Ks.Services.Helpers;
using Ks.Services.Localization;
using Ks.Services.Logging;
using Ks.Services.Reports;
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
        private readonly IReportService _reportService;

        private const string PATHBASE = @"C:\inetpub\wwwroot\Acmr\App_Data\Service\";

        #endregion

        #region Constructors

        public ScheduleBatchController(IScheduleBatchService scheduleBatchService,
            IPermissionService permissionService,
            IDateTimeHelper dateTimeHelper, ILocalizationService localizationService,
            ICustomerActivityService customerActivityService, IReportService reportService,
            ScheduleBatchsSetting scheduleBatchsSetting, IExportManager exportManager)
        {
            this._scheduleBatchService = scheduleBatchService;
            this._permissionService = permissionService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._customerActivityService = customerActivityService;
            this._scheduleBatchsSetting = scheduleBatchsSetting;
            this._exportManager = exportManager;
            this._reportService = reportService;
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
            var model = new ScheduleBatchModel
            {
                ReportInfo = new ReportInfo
                {
                    AvailableMonths = DateTime.Now.GetMonthsList(_localizationService),
                    AvailableYears = DateTime.Now.GetYearsList(_localizationService, -5, 15)
                }
            };


            model.ReportInfo.Types.Add(new SelectListItem { Text = "---------------", Value = "0" });
            model.ReportInfo.Types.Add(new SelectListItem { Text = "Copere", Value = "1" });
            model.ReportInfo.Types.Add(new SelectListItem { Text = "Caja", Value = "2" });

            model.ReportInfo.SubTypes.Add(new SelectListItem { Text = "---------------", Value = "0" });
            model.ReportInfo.SubTypes.Add(new SelectListItem { Text = "Envio", Value = "1" });
            model.ReportInfo.SubTypes.Add(new SelectListItem { Text = "Recepcion", Value = "2" });
            model.ReportInfo.SubTypes.Add(new SelectListItem { Text = "Resultado", Value = "3" });

            model.ReportInterface.AvailableMonths = DateTime.Now.GetMonthsList(_localizationService);
            model.ReportInterface.AvailableYears = DateTime.Now.GetYearsList(_localizationService,-20,22);

            model.ReportInterface.States.Add(new SelectListItem { Text = "Activo", Value = "2" });
            model.ReportInterface.States.Add(new SelectListItem { Text = "Inactivo", Value = "1" });

            
            model.ReportInterface.Types=Ks.Web.Framework.Extensions.GetDescriptions(typeof(CustomerMilitarySituation));
            model.ReportInterface.Types.Insert(0, new SelectListItem { Text = "---------------", Value = "0" });

            return View(model);
        }
         
        [HttpPost]
        public ActionResult ListBatchs(DataSourceRequest command)
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

                model.ReportInterface.AvailableMonths = DateTime.Now.GetMonthsList(_localizationService);
                model.ReportInterface.AvailableYears = DateTime.Now.GetYearsList(_localizationService);
                model.ReportInterface.Types = new List<SelectListItem>() {
                    new SelectListItem { Value="0", Text= _localizationService.GetResource("Admin.Common.Select") },
                    new SelectListItem { Value="1", Text= "Copere" },
                    new SelectListItem { Value="2", Text= "Caja" }
                };
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

            //if (ModelState.IsValid)
            //{
            //    if (batch.Enabled)
            //    {
            //        ErrorNotification(_localizationService.GetResource("Admin.System.ScheduleBatchs.Error"));
            //        return RedirectToAction("Edit", new { id = batch.Id });
            //    }

            //    //Is not Enabled
            //    batch = model.ToEntity(batch);
            //    if (model.StartExecutionOn.HasValue)
            //    {
            //        var hour = model.StartExecutionOn.Value.Hour;
            //        var minute = model.StartExecutionOn.Value.Minute;
            //        var second = model.StartExecutionOn.Value.Second;

            //        var startExecution = DateTime.Now;
            //        if (batch.SystemName == _scheduleBatchsSetting.ServiceName1)
            //        {
            //            startExecution = new DateTime(DateTime.Now.Year, DateTime.Now.Month, _scheduleBatchsSetting.DayOfProcess1, hour, minute, second);
            //            if (DateTime.Now.Day > _scheduleBatchsSetting.DayOfProcess1)
            //                startExecution = startExecution.AddMonths(1);
            //        }

            //        if (batch.SystemName == _scheduleBatchsSetting.ServiceName2)
            //        {
            //            startExecution = new DateTime(DateTime.Now.Year, DateTime.Now.Month, _scheduleBatchsSetting.DayOfProcess2, hour, minute, second);
            //            if (DateTime.Now.Day > _scheduleBatchsSetting.DayOfProcess1)
            //                startExecution = startExecution.AddMonths(1);
            //        }


            //        batch.StartExecutionOnUtc = _dateTimeHelper.ConvertToUtcTime(startExecution);
            //        batch.NextExecutionOnUtc = _dateTimeHelper.ConvertToUtcTime(startExecution);
            //        batch.LastExecutionOnUtc = null;
            //    }

            //    _scheduleBatchService.UpdateBatch(batch);
            //    //_customerActivityService.InsertActivity()
            //    SuccessNotification(_localizationService.GetResource("Admin.System.ScheduleBatchs.Updated"));

            //    model.ReportSummaryMerges = _reportService.ExportReportSummaryMergeFromDataBase(9, 2019, 0);
            //    return continueEditing ? RedirectToAction("Edit", new { id = batch.Id }) : RedirectToAction("List");
            //}

            model.ReportSummaryMerges = _reportService.ExportReportSummaryMergeFromDataBase(model.PeriodMonth, model.PeriodYear, model.ReportInterface.TypeId);

            model.FrecuencyName = ((ScheduleBatchFrecuency)batch.FrecuencyId).ToString();
            model.AvailableFrecuencies = ScheduleBatchFrecuency.Diario.ToSelectList().ToList();
            model.AvailableFrecuencies.Insert(0, new SelectListItem { Value = "0", Text = "-----------" });
            model.AvailableMonths = DateTime.Now.GetMonthsList(_localizationService);
            model.AvailableYears = DateTime.Now.GetYearsList(_localizationService);
            model.ReportInterface.AvailableMonths = DateTime.Now.GetMonthsList(_localizationService);
            model.ReportInterface.AvailableYears = DateTime.Now.GetYearsList(_localizationService);
            model.ReportInterface.Types = new List<SelectListItem>() {
                new SelectListItem { Value="0", Text= _localizationService.GetResource("Admin.Common.Select") },
                new SelectListItem { Value="1", Text= "Copere" },
                new SelectListItem { Value="2", Text= "Caja" }};

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

                string name = string.Empty;
                if (schedule.SystemName.Trim().ToUpper() == ("KS.BATCH.CAJA.OUT"))
                    name = string.Format("6008_{0}00", schedule.PeriodYear.ToString("0000") + schedule.PeriodMonth.ToString("00"));
                else
                    name = string.Format("8001_{0}00", schedule.PeriodYear.ToString("0000") + schedule.PeriodMonth.ToString("00"));
                name += "-previo.zip";

                int max = 4;
                while (!System.IO.File.Exists(Path.Combine(Path.Combine(schedule.PathBase,schedule.FolderMoveToDone), name))){
                    if (max == 0)
                        break;
                    max--;
                    Thread.Sleep(30 * 1000);
                } 
                return File(Path.Combine(Path.Combine(schedule.PathBase, schedule.FolderMoveToDone), name), "application/zip", name);
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

                string name = string.Empty;
                if (schedule.SystemName == ("Ks.Batch.Caja.Out"))
                    name = string.Format("6008_{0}00", schedule.PeriodYear.ToString("0000") + schedule.PeriodMonth.ToString("00"));
                else
                    name = string.Format("8001_{0}00", schedule.PeriodYear.ToString("0000") + schedule.PeriodMonth.ToString("00"));
                name += ".zip";

                int max = 4;
                while (!System.IO.File.Exists(Path.Combine(schedule.PathBase, name))){
                    if (max == 0)
                        break;
                    max--;
                    Thread.Sleep(30 * 1000);
                }

                return File(Path.Combine(Path.Combine(schedule.PathBase, schedule.FolderMoveToDone), name), "application/zip", name);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }

        }


        [HttpPost]
        public ActionResult DownloadData(ScheduleBatchModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleBatchs))
                return AccessDeniedView();

            var source = "";
            if (model.ReportInfo.TypeId == 1 && model.ReportInfo.SubTypeId == 1) //Copere Envio
                source = "Ks.Batch.Copere.Out";
            if (model.ReportInfo.TypeId == 1 && model.ReportInfo.SubTypeId == 2) //Copere Recepcion
                source = "Ks.Batch.Copere.In";
            if (model.ReportInfo.TypeId == 1 && model.ReportInfo.SubTypeId == 3) //Copere Resultado
                source = "Ks.Batch.Copere.In,Ks.Batch.Copere.Out";

            if (model.ReportInfo.TypeId == 2 && model.ReportInfo.SubTypeId == 1) //Caja Envio
                source = "Ks.Batch.Caja.Out";
            if (model.ReportInfo.TypeId == 2 && model.ReportInfo.SubTypeId == 2) //Caja Recepcion
                source = "Ks.Batch.Caja.In";
            if (model.ReportInfo.TypeId == 2 && model.ReportInfo.SubTypeId == 3) //Caja Resultado
                source = "Ks.Batch.Caja.Out,Ks.Batch.Caja.In";

            if (source == "")
                return RedirectToAction("List");
            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {

                    if (source.Split(',').Count() == 2)
                    {
                        var envio = _reportService.GetInfo(source.Split(',')[0],
                            model.ReportInfo.YearId.ToString("0000") + model.ReportInfo.MonthId.ToString("00"));
                        var recepcion = _reportService.GetInfo(source.Split(',')[1],
                            model.ReportInfo.YearId.ToString("0000") + model.ReportInfo.MonthId.ToString("00"));

                        if (envio != null && recepcion != null)
                            _exportManager.ExportReportInfoMergeToXlsx(stream, source,  recepcion, envio);
                    }
                    else
                    {
                        var info = _reportService.GetInfo(source, model.ReportInfo.YearId.ToString("0000") + model.ReportInfo.MonthId.ToString("00"));
                        _exportManager.ExportReportInfoToXlsx(stream, source, info);
                    }

                    bytes = stream.ToArray();
                }
                //Response.ContentType = "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //Response.AddHeader("content-disposition", "attachment; filename=Aportaciones.xlsx");
                return File(bytes, "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte de Interfaz.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public ActionResult ListReport(ScheduleBatchModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleBatchs))
                return AccessDeniedView();


            var interfaceLoan = _reportService.GetInterfaceLoan(
                model.ReportInterface.YearId, model.ReportInterface.MonthId, model.ReportInterface.TypeId, model.ReportInterface.StateId - 1);
            var interfaceContribution = _reportService.GetInterfaceContribution(
                model.ReportInterface.YearId, model.ReportInterface.MonthId, model.ReportInterface.TypeId,model.ReportInterface.StateId-1);
            try
                {
                    byte[] bytes;
                    using (var stream = new MemoryStream())
                    {
                        _exportManager.ExportinterfaceToXlsx(stream, model.ReportInterface.YearId, model.ReportInterface.MonthId, interfaceLoan,interfaceContribution);
                        bytes = stream.ToArray();
                    }
                    //Response.ContentType = "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //Response.AddHeader("content-disposition", "attachment; filename=Aportaciones.xlsx");
                    return File(bytes, "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte De Inerfaces.xlsx");
                }
                catch (Exception exc)
                {
                    ErrorNotification(exc);
                }
            
            return RedirectToAction("List");
        }

        public ActionResult PreMerge(string type)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleBatchs))
                return AccessDeniedView();
             
            try {

                if (type == "Caja")
                {
                    var path = PATHBASE + @"\Ks.Batch.Merge\Read";
                    path = System.IO.Path.Combine(path, "PreCajaWakeUp.txt");
                    using (var myFile = System.IO.File.Create(path))
                    {
                        TextWriter tw = new StreamWriter(myFile);
                        tw.WriteLine("PreCajaWakeUp");
                        tw.Close();
                    }
                }
                else
                {
                    var path = PATHBASE + @"Ks.Batch.Merge\Read";
                    path = System.IO.Path.Combine(path, "PreCopereWakeUp.txt");
                    using (var myFile = System.IO.File.Create(path))
                    {
                        TextWriter tw = new StreamWriter(myFile);
                        tw.WriteLine("PreCopereWakeUp");
                        tw.Close();
                    }
                }


                SuccessNotification(_localizationService.GetResource("Admin.Configuration.ScheduleBatch.Imported"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        public ActionResult CreateMerge(string type)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleBatchs))
                return AccessDeniedView();

            try
            {
                if (type == ("Caja"))
                {
                    var path = PATHBASE + @"\Ks.Batch.Merge\Read";
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
                    var path = PATHBASE + @"\Ks.Batch.Merge\Read";
                    path = System.IO.Path.Combine(path, "CopereWakeUp.txt");
                    using (var myFile = System.IO.File.Create(path))
                    {
                        TextWriter tw = new StreamWriter(myFile);
                        tw.WriteLine("CopereWakeUp");
                        tw.Close();
                    }
                }


                SuccessNotification(_localizationService.GetResource("Admin.Configuration.ScheduleBatch.Imported"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }

        }

        public ActionResult Revert(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleBatchs))
                return AccessDeniedView();

            var schedule = _scheduleBatchService.GetBatchById(id);
            if (schedule == null)
                //No  scheduleBatch  found with the specified id
                return RedirectToAction("List");

            //Siempre esta adelantado uno 
            if(schedule.PeriodMonth == 1)
            {
                schedule.PeriodMonth = 12;
                schedule.PeriodYear--;
            }
            else
            {
                schedule.PeriodMonth--;
            }

            var period = schedule.PeriodYear.ToString("D4") + schedule.PeriodMonth.ToString("D2");
            var source = schedule.SystemName.Replace(".In", "").Replace(".Out", "");

            //out esta en Procesado == 2
            //in esta en Espera ==1            
            var result = _reportService.CanRevertBatch(period, source + ".Out", (int)ReportState.InProcess, source + ".In", (int)ReportState.Waiting);

            if (result)
            {
                try
                {
                    var path = PATHBASE + @"\Ks.Batch.Reverse";
                    path = System.IO.Path.Combine(path, schedule.SystemName);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                    using (var myFile = System.IO.File.Create(path))
                    {
                        TextWriter tw = new StreamWriter(myFile);
                        tw.WriteLine("revert - " + id + "-" + schedule.SystemName);
                        tw.Close();
                    }

                    SuccessNotification(_localizationService.GetResource("Admin.Configuration.ScheduleBatch.Reverted"));
                    return RedirectToAction("List");
                }
                catch (Exception exc)
                {
                    ErrorNotification(exc);
                    return RedirectToAction("List");
                }
            }
            else
            {
               ErrorNotification("Por el momento esta combinacion no esta disponible");
                return RedirectToAction("List");
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