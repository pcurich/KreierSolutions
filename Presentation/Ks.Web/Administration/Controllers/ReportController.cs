using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using Ks.Admin.Extensions;
using Ks.Admin.Models.Report;
using Ks.Core.Domain.Contract;
using Ks.Services.Common;
using Ks.Services.Contract;
using Ks.Services.Customers;
using Ks.Services.ExportImport;
using Ks.Services.Helpers;
using Ks.Services.Localization;
using Ks.Services.Logging;
using Ks.Services.Reports;
using Ks.Services.Security;
using Ks.Web.Framework.Controllers;

namespace Ks.Admin.Controllers
{
    public class ReportController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IContributionService _contributionService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IBenefitService _benefitService;
        private readonly ITabService _tabService;
        private readonly IExportManager _exportManager;
        private readonly IReturnPaymentService _returnPaymentService;
        private readonly IReportService _reportService;

        private readonly ContributionSettings _contributionSettings;
        private readonly BankSettings _bankSettings;
        private readonly BenefitValueSetting _benefitValueSetting;
        private readonly SequenceIdsSettings _sequenceIdsSettings;


        #endregion

        #region Constructors

        public ReportController(
            IPermissionService permissionService,
            IContributionService contributionService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            IDateTimeHelper dateTimeHelper,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IExportManager exportManager,
            ContributionSettings contributionSettings,
            IReturnPaymentService returnPaymentService,
            IBenefitService benefitService,
            ITabService tabService,
            IReportService reportService,
            BankSettings bankSettings,
            SequenceIdsSettings sequenceIdsSettings,
            BenefitValueSetting benefitValueSetting)
        {
            _permissionService = permissionService;
            _contributionService = contributionService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _dateTimeHelper = dateTimeHelper;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _exportManager = exportManager;
            _contributionSettings = contributionSettings;
            _bankSettings = bankSettings;
            _returnPaymentService = returnPaymentService;
            _benefitService = benefitService;
            _tabService = tabService;
            _reportService = reportService;
            _benefitValueSetting = benefitValueSetting;
            _sequenceIdsSettings = sequenceIdsSettings;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReports))
                return AccessDeniedView();

            var model = new ReportListModel
            {
                ReportGlobal = new ReportGlobal
                {
                    Months = DateTime.UtcNow.GetMonthsList(_localizationService),
                    Years = DateTime.UtcNow.GetYearsList(_localizationService, -45, 80),
                    Sources = new List<SelectListItem>
                    {
                        new SelectListItem {Value = "0", Text = "-----------------"},
                        new SelectListItem {Value = "1", Text = "Todos"},
                        new SelectListItem {Value = "2", Text = "Caja Pensión Militar Policial"},
                        new SelectListItem {Value = "3", Text = "Copere"}
                    },
                    Types = new List<SelectListItem>
                    {
                        new SelectListItem {Value = "0", Text = "-----------------"},
                        new SelectListItem {Value = "1", Text = "Todos"},
                        new SelectListItem {Value = "2", Text = "Aportaciones"},
                        new SelectListItem {Value = "3", Text = "Apoyo Social Económico"}
                    }
                },
                ReportLoan = new ReportLoan
                {
                    States = new List<SelectListItem>
                    {
                        new SelectListItem {Value = "0", Text = "-----------------"},
                        new SelectListItem {Value = "1", Text = "Vigente"},
                        new SelectListItem {Value = "2", Text = "Cancelado"},
                    },
                    Types = new List<SelectListItem>
                    {
                        new SelectListItem {Value = "0", Text = "-----------------"},
                        new SelectListItem {Value = "1", Text = "Copere"},
                        new SelectListItem {Value = "2", Text = "Caja Pensión Militar Policial"}
                    },
                }
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult GlobalReport(ReportListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReports))
                return AccessDeniedView();

            var hasError = false;
            var errorMessage = string.Empty;
            if (model.ReportGlobal.TypeId == 0)
            {
                errorMessage += _localizationService.GetResource("Admin.Catalog.ReportGlobal.Fields.Type.Required") +
                                " - ";
                hasError = true;
            }
            if (model.ReportGlobal.SourceId == 0)
            {
                errorMessage += _localizationService.GetResource("Admin.Catalog.ReportGlobal.Fields.Source.Required") +
                                "-";
                hasError = true;
            }
            if (model.ReportGlobal.Month == 0)
            {
                errorMessage += _localizationService.GetResource("Admin.Catalog.ReportGlobal.Fields.Month.Required") +
                                " - ";
                hasError = true;
            }
            if (model.ReportGlobal.Year == 0)
            {
                errorMessage += _localizationService.GetResource("Admin.Catalog.ReportGlobal.Fields.Year.Required");
                hasError = true;
            }

            if (!hasError)
            {
                var globalReport = _reportService.GetGlobalReport(
                    model.ReportGlobal.Year, model.ReportGlobal.Month, model.ReportGlobal.TypeId,
                    model.ReportGlobal.SourceId == 3 ? "Copere" : "",
                    model.ReportGlobal.SourceId == 2 ? "Caja" : ""
                    );
                try
                {
                    byte[] bytes;
                    using (var stream = new MemoryStream())
                    {
                        _exportManager.ExportGlobalReportToXlsx(stream, model.ReportGlobal.Year, model.ReportGlobal.Month, globalReport);
                        bytes = stream.ToArray();
                    }
                    //Response.ContentType = "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //Response.AddHeader("content-disposition", "attachment; filename=Aportaciones.xlsx");
                    return File(bytes, "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte Global.xlsx");
                }
                catch (Exception exc)
                {
                    ErrorNotification(exc);
                    return RedirectToAction("List");
                }
            }
            ErrorNotification(errorMessage);
            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult LoanGeneralReport(ReportListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReports))
                return AccessDeniedView();

            var hasError = false;
            var errorMessage = string.Empty;

            if (!model.ReportLoan.From.HasValue)
            {
                errorMessage += _localizationService.GetResource("Admin.Catalog.ReportLoan.Fields.From.Required") + " - ";
                hasError = true;
            }
            if (!model.ReportLoan.To.HasValue)
            {
                errorMessage += _localizationService.GetResource("Admin.Catalog.ReportLoan.Fields.To.Required") + " - ";
                hasError = true;
            }
            if (!hasError)
            {
                var reportLoan = _reportService.GetDetailLoan(
                model.ReportLoan.From.Value.Year, model.ReportLoan.From.Value.Month, model.ReportLoan.From.Value.Day,
                model.ReportLoan.To.Value.Year, model.ReportLoan.To.Value.Month, model.ReportLoan.To.Value.Day,
                model.ReportLoan.TypeId,model.ReportLoan.StatesId);
                try
                {
                    byte[] bytes;
                    using (var stream = new MemoryStream())
                    {
                        _exportManager.ExportDetailLoanToXlsx(stream, model.ReportLoan.From.Value, model.ReportLoan.To.Value,  
                            model.ReportLoan.TypeId == 1 ? "COPERE" : "CAJA PENSION MILITAR POLICIAL", 
                            reportLoan);
                        bytes = stream.ToArray();
                    }
                    //Response.ContentType = "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //Response.AddHeader("content-disposition", "attachment; filename=Aportaciones.xlsx");
                    return File(bytes, "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte Global.xlsx");
                }
                catch (Exception exc)
                {
                    ErrorNotification(exc);
                    return RedirectToAction("List");
                }
            }
            ErrorNotification(errorMessage);
            return RedirectToAction("List");





        }


        #endregion
    }
}