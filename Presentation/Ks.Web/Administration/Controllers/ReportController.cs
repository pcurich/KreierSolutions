using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Ks.Admin.Extensions;
using Ks.Admin.Models.Report;
using Ks.Core.Domain.Contract;
using Ks.Core.Domain.Customers;
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
                    
                    Types = new List<SelectListItem>
                    {
                        new SelectListItem {Value = "0", Text = "-----------------"},
                        new SelectListItem {Value = "1", Text = "Todos"},
                        new SelectListItem {Value = "2", Text = "Copere"},
                        new SelectListItem {Value = "3", Text = "Caja Pensión Militar Policial"}
                    },
                },
                ReportContribution = new ReportContribution
                {
                    To = DateTime.Now.GetYearsList(_localizationService, -80, 81),
                    From = DateTime.Now.GetYearsList(_localizationService, -80, 81),
                    Types = new List<SelectListItem>
                    {
                        new SelectListItem {Value = "0", Text = "-----------------"},
                        new SelectListItem {Value = "1", Text = "Todos"},
                        new SelectListItem {Value = "2", Text = "Copere"},
                        new SelectListItem {Value = "3", Text = "Caja Pensión Militar Policial"}
                    },
                },
                ReportMilitarySituation = new ReportMilitarySituation
                {
                    MilitarySituations = Ks.Web.Framework.Extensions.GetDescriptions(typeof(CustomerMilitarySituation)),
                    ContributionStates = new List<SelectListItem>
                    {
                        new SelectListItem { Value = "0", Text = "-----------------" },
                        new SelectListItem { Value = "1", Text = "Todos" },
                        new SelectListItem { Value = "2", Text = "Activo" },
                        new SelectListItem { Value = "3", Text = "Inactivo" },
                    },
                    LoanStates = new List<SelectListItem>
                    {
                        new SelectListItem { Value = "0", Text = "-----------------" },
                        new SelectListItem { Value = "1", Text = "Todos" },
                        new SelectListItem { Value = "2", Text = "Activo" },
                        new SelectListItem { Value = "3", Text = "Inactivo" },
                    }
                },
                SumaryBankPayment = new SumaryBankPayment
                {
                    Types = new List<SelectListItem>
                    {
                        new SelectListItem {Value = "0", Text = "-----------------"},
                        new SelectListItem {Value = "1", Text = "Todos"},
                        new SelectListItem {Value = "2", Text = "Copere"},
                        new SelectListItem {Value = "3", Text = "Caja Pensión Militar Policial"}
                    },
                    Sources = new List<SelectListItem>
                    {
                        new SelectListItem {Value = "0", Text = "-----------------"},
                        new SelectListItem {Value = "1", Text = "Todos"},
                        new SelectListItem {Value = "2", Text = "Aportaciones"},
                        new SelectListItem {Value = "3", Text = "Apoyo Social Economico"}
                    },
                },
                ReportBenefit = new ReportBenefit(),
                ReportCheck = new ReportCheck
                {
                    Types = new List<SelectListItem>
                    {
                        new SelectListItem {Value = "0", Text = "-----------------"},
                        new SelectListItem {Value = "1", Text = "Todos"},
                        new SelectListItem {Value = "2", Text = "Apoyo Social Económico"},
                        new SelectListItem {Value = "3", Text = "Devoluciones"},
                        new SelectListItem {Value = "4", Text = "Beneficios"},
                    }
                }
            };

            model.ReportMilitarySituation.MilitarySituations.Insert(0, new SelectListItem { Value = "0", Text = "-------------" });
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
                model.ReportLoan.TypeId );
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
                    return File(bytes, "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte Apoyo Social Economico.xlsx");
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
        public ActionResult SummaryContributionReport(ReportListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReports))
                return AccessDeniedView();

            var hasError = false;
            var errorMessage = string.Empty;

            //if (model.ReportContribution.FromId == 0)
            //{
            //    errorMessage += _localizationService.GetResource("Admin.Catalog.ReportContribution.Fields.From.Required") + " - ";
            //    hasError = true;
            //}
            //if (model.ReportContribution.ToId == 0)
            //{
            //    errorMessage += _localizationService.GetResource("Admin.Catalog.ReportContribution.Fields.To.Required") + " - ";
            //    hasError = true;
            //}

            if (!model.ReportContribution.FromDate.HasValue)
            {
                errorMessage += _localizationService.GetResource("Admin.Catalog.ReportContribution.Fields.From.Required") + " - ";
                hasError = true;
            }
            if (!model.ReportContribution.ToDate.HasValue)
            {
                errorMessage += _localizationService.GetResource("Admin.Catalog.ReportContribution.Fields.To.Required") + " - ";
                hasError = true;
            }

            if (!hasError)
            {
                var summaryContribution = _reportService.GetSummaryContribution(
                    model.ReportContribution.FromDate.Value.Year,
                    model.ReportContribution.FromDate.Value.Month,
                    model.ReportContribution.ToDate.Value.Year,
                    model.ReportContribution.ToDate.Value.Month,
                model.ReportContribution.TypeId);
                try
                {
                    byte[] bytes;
                    using (var stream = new MemoryStream())
                    {
                        _exportManager.ExportSummaryContributionToXlsx(stream, model.ReportContribution.FromDate.Value, model.ReportContribution.ToDate.Value,
                            model.ReportContribution.TypeId, summaryContribution);
                        bytes = stream.ToArray();
                    }
                    //Response.ContentType = "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //Response.AddHeader("content-disposition", "attachment; filename=Aportaciones.xlsx");
                    return File(bytes, "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte Consolidado Aportaciones.xlsx");
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
        public ActionResult MilitarSituation(ReportListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReports))
                return AccessDeniedView();

            var hasError = false;
            var errorMessage = string.Empty;

            if (model.ReportMilitarySituation.MilitarySituationId == 0)
            {
                errorMessage += _localizationService.GetResource("Admin.Catalog.ReportMilitarySituation.Fields.MilitarySituation.Required") + " - ";
                hasError = true;
            }
            if (model.ReportMilitarySituation.ContributionStateId == 0)
            {
                errorMessage += _localizationService.GetResource("Admin.Catalog.ReportMilitarySituation.Fields.ContributionState.Required") + " - ";
                hasError = true;
            }
            if (model.ReportMilitarySituation.LoanStateId == 0)
            {
                errorMessage += _localizationService.GetResource("Admin.Catalog.ReportMilitarySituation.Fields.LoanState.Required");
                hasError = true;
            }

            if (!hasError)
            {
                int loanState = -1;
                if (model.ReportMilitarySituation.LoanStateId == 2)
                    loanState = 1;
                if (model.ReportMilitarySituation.LoanStateId == 3)
                    loanState = 0;

                int contributionState = -1;
                if (model.ReportMilitarySituation.ContributionStateId == 2)
                    contributionState = 1;
                if (model.ReportMilitarySituation.ContributionStateId == 3)
                    contributionState = 0;


                var militarSituations = _reportService.GetMilitarSituation(model.ReportMilitarySituation.MilitarySituationId, loanState,contributionState);
                try
                {
                    byte[] bytes;
                    using (var stream = new MemoryStream())
                    {
                        var name =
                            Ks.Web.Framework.Extensions.GetDescriptions(typeof(CustomerMilitarySituation))
                                .FirstOrDefault(x => x.Value == model.ReportMilitarySituation.MilitarySituationId.ToString()).Text;
                        _exportManager.ExportMilitarSituationToXlsx(stream, name, militarSituations);
                        bytes = stream.ToArray();
                    }
                    //Response.ContentType = "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //Response.AddHeader("content-disposition", "attachment; filename=Aportaciones.xlsx");
                    return File(bytes, "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte Situacion Militar.xlsx");
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
        public ActionResult BenefitReport(ReportListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReports))
                return AccessDeniedView();

            var hasError = false;
            var errorMessage = string.Empty;

            if (!model.ReportBenefit.From.HasValue)
            {
                errorMessage += _localizationService.GetResource("Admin.Catalog.ReportBenefit.Fields.From.Required") + " - ";
                hasError = true;
            }
            if (!model.ReportBenefit.To.HasValue)
            {
                errorMessage += _localizationService.GetResource("Admin.Catalog.ReportBenefit.Fields.To.Required") + " - ";
                hasError = true;
            }
            if (model.ReportBenefit.TypeId == 0)
            {
                errorMessage += _localizationService.GetResource("Admin.Catalog.ReportBenefit.Fields.Type.Required") + " - ";
                hasError = true;
            }
            if (model.ReportBenefit.SourceId == 0)
            {
                errorMessage += _localizationService.GetResource("Admin.Catalog.ReportBenefit.Fields.Source.Required") + " - ";
                hasError = true;
            }


            if (!hasError)
            {
                var benefit = _reportService.GetContributionBenefit(model.ReportBenefit.From.Value.Year, model.ReportBenefit.From.Value.Month,
                    model.ReportBenefit.To.Value.Year, model.ReportBenefit.To.Value.Month,
                    model.ReportBenefit.TypeId, model.ReportBenefit.SourceId);
                try
                {
                    byte[] bytes;
                    using (var stream = new MemoryStream())
                    {
                        _exportManager.ExportBenefitToXlsx(stream, _benefitService.GetBenefitById(model.ReportBenefit.TypeId), benefit);
                        bytes = stream.ToArray();
                    }
                    //Response.ContentType = "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //Response.AddHeader("content-disposition", "attachment; filename=Aportaciones.xlsx");
                    return File(bytes, "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte Beneficios.xlsx");
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
        public ActionResult BankPaymentReport(ReportListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReports))
                return AccessDeniedView();

            var hasError = false;
            var errorMessage = string.Empty;

            if (!model.SumaryBankPayment.From.HasValue)
            {
                errorMessage += _localizationService.GetResource("Admin.Catalog.SumaryBankPayment.Fields.From.Required") + " - ";
                hasError = true;
            }
            if (!model.SumaryBankPayment.To.HasValue)
            {
                errorMessage += _localizationService.GetResource("Admin.Catalog.SumaryBankPayment.Fields.To.Required") + " - ";
                hasError = true;
            }
            if (model.SumaryBankPayment.TypeId == 0)
            {
                errorMessage += _localizationService.GetResource("Admin.Catalog.SumaryBankPayment.Fields.Type.Required") + " - ";
                hasError = true;
            }
            if (model.SumaryBankPayment.SourceId == 0)
            {
                errorMessage += _localizationService.GetResource("Admin.Catalog.SumaryBankPayment.Fields.Source.Required") + " - ";
                hasError = true;
            }


            if (!hasError)
            {
                var summaryBankPayment = _reportService.GetBankPayment(
                    model.SumaryBankPayment.From.Value.Year, model.SumaryBankPayment.From.Value.Month, model.SumaryBankPayment.From.Value.Day,
                    model.SumaryBankPayment.To.Value.Year, model.SumaryBankPayment.To.Value.Month, model.SumaryBankPayment.To.Value.Day,
                    model.SumaryBankPayment.TypeId-1, model.SumaryBankPayment.SourceId-1);
                try
                {
                    byte[] bytes;
                    using (var stream = new MemoryStream())
                    {
                        _exportManager.ExportBankPaymentToXlsx(stream, model.SumaryBankPayment.From.Value,model.SumaryBankPayment.To.Value, summaryBankPayment);
                        bytes = stream.ToArray();
                    }
                    //Response.ContentType = "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //Response.AddHeader("content-disposition", "attachment; filename=Aportaciones.xlsx");
                    return File(bytes, "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte Depositos Bancarios.xlsx");
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
        public ActionResult Checks(ReportListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReports))
                return AccessDeniedView();

            var hasError = false;
            var errorMessage = string.Empty;

            if (!model.ReportCheck.From.HasValue)
            {
                errorMessage += "Seleccione una fecha de Inicio" + " - ";
                hasError = true;
            }
            if (!model.ReportCheck.To.HasValue)
            {
                errorMessage += "Seleccione una fecha de Fin" + " - ";
                hasError = true;
            }
            if (!hasError)
            {
                var checks = _reportService.GetChecks(
                model.ReportCheck.From.Value.Year, model.ReportCheck.From.Value.Month, model.ReportCheck.From.Value.Day,
                model.ReportCheck.To.Value.Year, model.ReportCheck.To.Value.Month, model.ReportCheck.To.Value.Day,
                model.ReportCheck.TypeId-1);
                try
                {
                    byte[] bytes;
                    using (var stream = new MemoryStream())
                    {
                        _exportManager.ExportChecksToXlsx(stream, model.ReportCheck.From.Value, model.ReportCheck.To.Value, checks);
                        bytes = stream.ToArray();
                    }
                    //Response.ContentType = "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //Response.AddHeader("content-disposition", "attachment; filename=Aportaciones.xlsx");
                    return File(bytes, "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte de Cheques.xlsx");
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