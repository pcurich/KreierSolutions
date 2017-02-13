using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ks.Admin.Extensions;
using Ks.Admin.Models.Contract;
using Ks.Core;
using Ks.Core.Domain.Common;
using Ks.Core.Domain.Contract;
using Ks.Core.Domain.Customers;
using Ks.Core.Domain.Directory;
using Ks.Core.Domain.Messages;
using Ks.Services.Common;
using Ks.Services.Configuration;
using Ks.Services.Contract;
using Ks.Services.Customers;
using Ks.Services.ExportImport;
using Ks.Services.Helpers;
using Ks.Services.KsSystems;
using Ks.Services.Localization;
using Ks.Services.Logging;
using Ks.Services.Messages;
using Ks.Services.Security;
using Ks.Web.Framework;
using Ks.Web.Framework.Controllers;
using Ks.Web.Framework.Kendoui;

namespace Ks.Admin.Controllers
{
    public partial class LoansController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IKsSystemService _ksSystemService;
        private readonly ISettingService _settingService;
        private readonly ILoanService _loanService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IExportManager _exportManager;
        private readonly IReturnPaymentService _returnPaymentService;
        private readonly IWorkContext _workContext;
        private readonly IWorkFlowService _workFlowService;
        private readonly BankSettings _bankSettings;

        #endregion

        #region Constructors

        public LoansController(IPermissionService permissionService, IKsSystemService ksSystemService, ISettingService settingService, ILoanService loanService, ICustomerService customerService, IGenericAttributeService genericAttributeService, IDateTimeHelper dateTimeHelper, ICustomerActivityService customerActivityService, ILocalizationService localizationService, IExportManager exportManager, IReturnPaymentService returnPaymentService, IWorkContext workContext, IWorkFlowService workFlowService, BankSettings bankSettings)
        {
            _permissionService = permissionService;
            _ksSystemService = ksSystemService;
            _settingService = settingService;
            _loanService = loanService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _dateTimeHelper = dateTimeHelper;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _exportManager = exportManager;
            _returnPaymentService = returnPaymentService;
            _workContext = workContext;
            _workFlowService = workFlowService;
            _bankSettings = bankSettings;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLoans))
                return AccessDeniedView();

            var model = new LoanListModel
            {
                SearchAdmCode = "",
                SearchDni = "",
                SearchLoanNumber = null,
                StateId = -1,
                States = new List<SelectListItem>
                {
                    new SelectListItem { Value = "-1", Text = "Todos" },
                    new SelectListItem { Value = "0", Text = "Inactivos" },
                    new SelectListItem { Value = "1", Text = "Activos" }
                }

            };
            return View(model);
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command, LoanListModel model)
        {
            GenericAttribute generic = null;

            //1) Find By Dni and AdmCode

            if (!string.IsNullOrWhiteSpace(model.SearchDni))
                generic = _genericAttributeService.GetAttributeForKeyValue("Dni", model.SearchDni.Trim());

            if (!string.IsNullOrWhiteSpace(model.SearchAdmCode) && generic == null)
                generic = _genericAttributeService.GetAttributeForKeyValue("AdmCode", model.SearchAdmCode.Trim());

            IPagedList<Loan> loans = null;
            if (generic != null && string.Compare(generic.KeyGroup, "Customer", StringComparison.CurrentCulture) == 0)
                loans = _loanService.SearchLoanByCustomerId(generic.EntityId, model.StateId);

            //2) Find by letter Number
            if (loans == null && model.SearchLoanNumber.HasValue)
                loans = _loanService.SearchLoanByLoanNumber(model.SearchLoanNumber.Value, model.StateId);

            if (loans == null)
                loans = new PagedList<Loan>(new List<Loan>(), 0, 10);

            var loanModel = loans.Select(x =>
            {
                var toModel = x.ToModel();
                toModel.CustomerCompleteName = x.Customer.GetFullName();
                toModel.CustomerDni = x.Customer.GetAttribute<string>(SystemCustomerAttributeNames.Dni);
                toModel.CustomerAdmCode = x.Customer.GetAttribute<string>(SystemCustomerAttributeNames.AdmCode);
                toModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
                if (x.UpdatedOnUtc.HasValue)
                    toModel.UpdatedOn = _dateTimeHelper.ConvertToUserTime(x.UpdatedOnUtc.Value, DateTimeKind.Utc);

                if (x.ApprovalOnUtc.HasValue)
                    toModel.ApprovalOn = _dateTimeHelper.ConvertToUserTime(x.ApprovalOnUtc.Value, DateTimeKind.Utc);

                return toModel;
            });

            var gridModel = new DataSourceResult
            {
                Data = loanModel,
                Total = loans.TotalCount
            };

            return Json(gridModel);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLoans))
                return AccessDeniedView();

            var loan = _loanService.GetLoanById(id);
            var customer = _customerService.GetCustomerById(loan.CustomerId);
            var model = new LoanPaymentListModel
            {
                Id = id,
                LoanId = id,
                IsAuthorized = loan.IsAuthorized,
                CustomerId = loan.CustomerId,
                CheckNumber = loan.CheckNumber,
                Active = loan.Active,
                States = LoanState.EnProceso.ToSelectList(false).ToList(),
                Banks = _bankSettings.PrepareBanks(),
                CustomerName = customer.GetFullName(),
                CustomerAdminCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.AdmCode),
                CustomerDni = customer.GetAttribute<string>(SystemCustomerAttributeNames.Dni),
                CustomerFrom = _dateTimeHelper.ConvertToUserTime(loan.CreatedOnUtc, DateTimeKind.Utc),
                Types = new List<SelectListItem>
                {
                    new SelectListItem { Value = "0", Text = "--------------", Selected = true},
                    new SelectListItem {Value = "2", Text = "Automatico"},
                    new SelectListItem {Value = "1", Text = "Manual"}
                }
            };

            model.States.Insert(0, new SelectListItem { Value = "0", Text = "--------------", Selected = true });

            return View(model);
        }

        [HttpPost]
        public ActionResult ListLoansPayments(DataSourceRequest command, LoanPaymentListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLoans))
                return AccessDeniedView();

            bool? type = null;
            if (model.Type == 0)
                type = null;
            if (model.Type == 2)
                type = true;
            if (model.Type == 1)
                type = false;

            var loanPayments = _loanService.GetAllPayments(
                loanId: model.LoanId, quota: model.Quota,
                stateId: model.StateId, accountNumber: model.BankName,
                type: type, pageIndex: command.Page - 1,
                pageSize: command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = loanPayments.Select(x =>
                {
                    var m = x.ToModel();
                    m.ScheduledDateOn = _dateTimeHelper.ConvertToUserTime(x.ScheduledDateOnUtc, DateTimeKind.Local);
                    m.ProcessedDateOn = x.ProcessedDateOnUtc.HasValue
                        ? _dateTimeHelper.ConvertToUserTime(x.ProcessedDateOnUtc.Value, DateTimeKind.Local)
                        : x.ProcessedDateOnUtc;
                    m.Type = x.IsAutomatic ? "Automatico" : "Manual";
                    m.State = x.LoanState.ToSelectList().FirstOrDefault(r => r.Value == x.StateId.ToString()).Text;
                    return m;
                }),
                Total = loanPayments.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var loan = _loanService.GetLoanById(id);
            var loanPayments = _loanService.GetAllPayments(loan.Id, stateId: (int)LoanState.Pendiente);
            foreach (var payment in loanPayments)
            {
                payment.StateId = (int)LoanState.Cancelado;
                payment.Description = "Apoyo economico cancelado por el usuario " +
                                      _workContext.CurrentCustomer.GetFullName();
                payment.ProcessedDateOnUtc = DateTime.UtcNow;
                _loanService.UpdateLoanPayment(payment);

            }
            loan.Active = false;
            loan.UpdatedOnUtc = DateTime.UtcNow;
            _loanService.UpdateLoan(loan);
            SuccessNotification("El apoyo social ha sido cancelado correctamente");
            return RedirectToAction("List");
        }

        #endregion

        #region Approval / AddCheck

        public ActionResult Approval(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ApprovalLoan))
                return AccessDeniedView();

            var loan = _loanService.GetLoanById(id);
            var model = loan.ToModel();
            model.StateName = loan.IsAuthorized ? "Aprobado" : "No Aprobado";
            model.States = new List<SelectListItem>
            {
                new SelectListItem{ Text = "-------------------", Value = "0"},
                new SelectListItem{ Text = "Aprobar", Value = "1"},
                new SelectListItem{ Text = "Desaprobar", Value = "2"},
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Approval(LoanModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ApprovalLoan))
                return AccessDeniedView();

            var loan = _loanService.GetLoanById(model.Id);
            if (model.StateId == 1)
            {
                loan.IsAuthorized = true;
                loan.ApprovalOnUtc = DateTime.UtcNow;
                _loanService.UpdateLoan(loan);
                _customerActivityService.InsertActivity(DefaultActivityLogType.ActivityLogApprobalLoan.SystemKeyword, "Se ha relizado la aprobacion del Apoyo Socia Económico");

                #region Flow - Approval required

                _workFlowService.InsertWorkFlow(new WorkFlow
                {
                    CustomerCreatedId = _workContext.CurrentCustomer.Id,
                    EntityId = loan.LoanNumber,
                    EntityName = WorkFlowType.Loan.ToString(),
                    RequireCustomer = false,
                    RequireSystemRole = true,
                    SystemRoleApproval = SystemCustomerRoleNames.Employee,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    Active = true,
                    Title = "Aprobado el Apoyo Social Económico",
                    Description = "Se ha realizado la aprobacion para la emision del Apoyo Social Económico N° " + loan.LoanNumber +
                    " para el asociado " + _customerService.GetCustomerById(loan.CustomerId).GetFullName(),
                    GoTo = "Admin/Loans/AddCheck/" + loan.Id
                });
                #endregion
            }

            if (model.StateId == 2)
            {
                loan.IsAuthorized = false;
                loan.ApprovalOnUtc = null;
                loan.UpdatedOnUtc = DateTime.UtcNow;
                loan.Active = false;
                var details = _loanService.GetAllPayments(model.Id, stateId: 1);
                foreach (var detail in details)
                {
                    detail.StateId = (int)LoanState.Cancelado;
                    detail.ProcessedDateOnUtc = DateTime.UtcNow;
                    _loanService.UpdateLoanPayment(detail);
                }
                _loanService.UpdateLoan(loan);
                _customerActivityService.InsertActivity(DefaultActivityLogType.ActivityLogNoApprobalLoan.SystemKeyword, "No se ha relizado la aprobacion del Apoyo Socia Económico");

                #region Flow - Approval required

                _workFlowService.InsertWorkFlow(new WorkFlow
                {
                    CustomerCreatedId = _workContext.CurrentCustomer.Id,
                    EntityId = loan.LoanNumber,
                    EntityName = WorkFlowType.Loan.ToString(),
                    RequireCustomer = false,
                    RequireSystemRole = true,
                    SystemRoleApproval = SystemCustomerRoleNames.Employee,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    Active = true,
                    Title = "No aprobado el Apoyo Social Económico",
                    Description = "Se ha realizado la no aprobacion para la emision del Apoyo Social Económico N° " + loan.LoanNumber +
                    " para el asociado " + _customerService.GetCustomerById(loan.CustomerId).GetFullName(),
                    GoTo = "Admin/Loans/Edit/" + loan.Id
                });
                #endregion
            }

            return RedirectToAction("Index", "Home");

        }

        public ActionResult AddCheck(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLoans))
                return AccessDeniedView();

            var loan = _loanService.GetLoanById(id);
            var model = loan.ToModel();
            model.Banks = _bankSettings.PrepareBanks();
            return View(model);
        }

        [HttpPost]
        public ActionResult AddCheck(LoanModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLoans))
                return AccessDeniedView();

            var loan = _loanService.GetLoanById(model.Id);
            loan.BankName = GetBank(model.BankName);
            loan.AccountNumber = model.AccountNumber;
            loan.CheckNumber = model.CheckNumber;

            _loanService.UpdateLoan(loan);

            _customerActivityService.InsertActivity("AddChekToLoan",
                "Se ha asignado el cheque al apoyo social economico  N° " + loan.LoanNumber);

            return RedirectToAction("Edit", new { id = loan.Id });
        }
        #endregion

        #region CreatePayment

        public ActionResult CreatePayment(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributions))
                return AccessDeniedView();


            var loanPayment = _loanService.GetPaymentById(id);
            var loan = _loanService.GetLoanById(loanPayment.LoanId);
            if (!loan.IsAuthorized)
            {
                ErrorNotification("No se puede realizar pagos debido a que el Apoyo Social Económico no se encuentra aprobado");
                return RedirectToAction("Edit", new { id = loan.Id });
            }

            if (!loan.Active)
            {
                ErrorNotification(_localizationService.GetResource("Admin.Customers.Loans.ValidActive"));
                return RedirectToAction("Edit", new { id = loan.Id });
            }

            var model = PrepareLoanPayment(loanPayment);
            model.MonthlyPayed = model.MonthlyQuota;

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult CreatePayment(LoanPaymentsModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLoans))
                return AccessDeniedView();

            var loanPayment = _loanService.GetPaymentById(model.Id);

            if (loanPayment.StateId != (int)LoanState.Pendiente)
            {
                ErrorNotification(_localizationService.GetResource("Admin.Customers.Loans.ValidPayment"));
                return RedirectToAction("CreatePayment", new { id = loanPayment.Id });
            }

            if (!ModelState.IsValid)
                return View(model);

            loanPayment.IsAutomatic = false;
            loanPayment.StateId = (int)LoanState.Pagado;
            loanPayment.LoanId = model.LoanId;
            loanPayment.BankName = GetBank(model.BankName);
            loanPayment.AccountNumber = model.AccountNumber;
            loanPayment.TransactionNumber = model.TransactionNumber;
            loanPayment.Reference = model.Reference;
            loanPayment.Description = model.Description;
            loanPayment.ProcessedDateOnUtc = DateTime.UtcNow;
            loanPayment.MonthlyPayed = loanPayment.MonthlyQuota;

            _loanService.UpdateLoanPayment(loanPayment);

            var loan = _loanService.GetLoanById(model.LoanId);
            loan.UpdatedOnUtc = DateTime.UtcNow;
            loan.TotalPayed += model.MonthlyPayed;
            loan.IsDelay = false;

            _loanService.UpdateLoan(loan);

            SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Loans.Updated"));

            if (continueEditing)
            {
                return RedirectToAction("CreatePayment", new { id = loanPayment.Id });
            }
            return RedirectToAction("Edit", new { id = loanPayment.LoanId });
        }

        #endregion

        #region Reports

        public ActionResult Download(int id)
        {
            var loan = _loanService.GetLoanById(id);
            var html = PrepareBody(loan);
            var stream = new MemoryStream((html.ConvertHtmlToPdf()));
            return new PdfDownloadResult(stream, "Archivo.pdf");
        }

        private string PrepareBody(Loan loan)
        {
            var sb = new StringBuilder();

            //Generando las cabeceras
            sb.Append("</br></br>");
            sb.Append("<table style='font-family:arial; font-size:18px; width:100%;' cellspacing='5' cellpadding='5'>");
            sb.Append("<tr>" +
                          "<td align='center' colspan='3'>" +
                            "<h2>N°Orden de Pago " + loan.LoanNumber + "</h2>" +
                          "</td>" +
                      "</tr>");

            var customer = _customerService.GetCustomerById(loan.CustomerId);
            //tabla de resumen
            sb.Append("<tr>" +
                      "<td align='left' style='width:70%;'>");
            sb.Append("<table style='font-family:arial; font-size:10px;'>" +
                         "<tr>" +
                             "<td rowspan='4' style='width:30%;'><b>Destinatario:</b></td>" +
                             "<td style='width:40%;'> " + customer.GetFullName() + " </td>" +
                             "<td rowspan='4' style='width:30%;'><b>Banco:</b></td>" +
                             "<td style='width:40%;'> " + loan.BankName + " </td>" +
                         "</tr>" +
                        "</table>" +
                         "<table style='font-family:arial; font-size:10px;'>" +
                         "<tr>" +
                             "<td rowspan='4' style='width:30%;'><b>N° Admin:</b></td>" +
                             "<td style='width:40%;'> " + customer.GetAttribute<string>(SystemCustomerAttributeNames.AdmCode) + " </td>" +
                             "<td rowspan='4' style='width:30%;'><b>N°Cheque:</b></td>" +
                             "<td style='width:40%;'> " + loan.CheckNumber + " </td>" +
                         "</tr>" +
                        "</table>" +
        "</td>" +
      "</tr>");
            sb.Append("</table>");

            sb.Append("</br></br>");
            sb.Append("<p style='font-family:arial; font-size:10px;'><b>Apoyo Económico S/ " + loan.LoanAmount + " Plazo: " + loan.Period + " meses</b></p>");
            sb.Append("<table style='border:1px solid black; font-size:10px;'>" +
                        "<tr>" +
                            "<td style='width:60%; border-bottom: 1px solid #ddd;'>Descripcion de la cuenta</td>" +
                            "<td style='width:15%; border-bottom: 1px solid #ddd;'>Cargo</td>" +
                            "<td style='width:15%; border-bottom: 1px solid #ddd;' align='right'>Abono</td>" +
                      "</tr>"+
                       "<tr>" +
                            "<td style='width:60%;'>MONTO TOTAL CON INTERESES</td>" +
                            "<td style='width:15%;'> S/"+loan.TotalAmount+"</td>" +
                            "<td style='width:15%;'> </td>" +
                      "</tr>" +
                      "<tr>" +
                            "<td style='width:60%;'>SEGURO DESGRAVAMEN "+loan.Safe+"%</td>" +
                            "<td style='width:15%;'></td>" +
                            "<td style='width:15%;' align='right'> S/" + loan.TotalSafe + "</td>" +
                      "</tr>" +
                       "<tr>" +
                            "<td style='width:60%;'>INTERESES TOTALES DIFERIDOS</td>" +
                            "<td style='width:15%;'></td>" +
                            "<td style='width:15%;' align='right'> S/" + loan.TotalFeed + "</td>" +
                      "</tr>" +
                       "<tr>" +
                            "<td style='width:60%;'>CTA CTE MN N° "+loan.AccountNumber+"</td>" +
                            "<td style='width:15%;'></td>" +
                            "<td style='width:15%;' align='right'> S/" + loan.TotalToPay + "</td>" +
                      "</tr>" +
                "</table>");
            sb.Append("</br>");

            sb.Append("<table style='border:1px solid black; font-size:10px;'>" +
                      "<tr>" +
                      "<td style='width:35%;'>Importe a descontarse en: " + loan.Period + "</td>" +
                      "<td style='width:35%;'>Cuotas Mensuales: S/" + loan.MonthlyQuota + " </td>" +
                      "</tr>" +
                      "</table>");

 
            sb.Append("</br>");
            sb.Append("</br>");
            sb.Append("</br>");

            var storeScope = this.GetActiveStoreScopeConfiguration(_ksSystemService, _workContext);
            var setting = _settingService.LoadSetting<SignatureSettings>(storeScope);

            sb.Append("<table style='border:0px solid black; font-size:10px;'>" +
                "<tr style='color:white;'><td>.</td><td>.</td></tr>" +
                "<tr style='color:white;' ><td>.</td><td>.</td></tr>" +
                "<tr style='color:white;' ><td>.</td><td>.</td></tr>" +
                "<tr style='color:white;' ><td>.</td><td>.</td></tr>" +
                "<tr style='color:white;' ><td>.</td><td>.</td></tr>" +
                "<tr style='color:white;' ><td>.</td><td>.</td></tr>" +
                "<tr style='color:white;' ><td>.</td><td>.</td></tr>" +
                      "<tr>" +
                      "<td style='width:45%;' align='center'>--------------------------------------------</td>" +
                      "<td style='width:45%;' align='center'>--------------------------------------------</td>" +
                      "</tr>" +
                      "<tr>" +
                      "<td style='width:45%;' align='center'>" + setting.BenefitCenterName + "</td>" +
                      "<td style='width:45%;' align='center'>" + setting.BenefitLeftName + "</td>" +
                      "</tr>" +
                      "<tr>" +
                      "<td style='width:45%;' align='center'>" + setting.BenefitCenterPosition + "</td>" +
                      "<td style='width:45%;' align='center'>" + setting.BenefitLeftPosition + "</td>" +
                      "</tr>" +
                      "<tr style='color:white;'><td>.</td><td>.</td></tr>" +
                "<tr style='color:white;' ><td>.</td><td>.</td></tr>" +
                "<tr style='color:white;' ><td>.</td><td>.</td></tr>" +
                "<tr style='color:white;' ><td>.</td><td>.</td></tr>" +
                "<tr style='color:white;' ><td>.</td><td>.</td></tr>" +
                "<tr style='color:white;' ><td>.</td><td>.</td></tr>" +
                "<tr style='color:white;' ><td>.</td><td>.</td></tr>" +
                 "<tr>" +
                      "<td style='width:45%;' align='center'>--------------------------------------------</td>" +
                      "<td style='width:45%;' align='center'>--------------------------------------------</td>" +
                      "</tr>" +
                      "<tr>" +
                      "<td style='width:45%;' align='center'>" + setting.LoanRightName + "</td>" +
                      "<td style='width:45%;' align='center'>" + setting.LoanLeftName + "</td>" +
                      "</tr>" +
                      "<tr>" +
                      "<td style='width:45%;' align='center'>" + setting.LoanRightPosition+ "</td>" +
                      "<td style='width:45%;' align='center'>" + setting.LoanLeftPosition + "</td>" +
                      "</tr>" +
                      "</table>");

            sb.Append("</br>");
            sb.Append("</br>");
            sb.Append("</br>");
            sb.Append("</br>");
            sb.Append("</br>");
            sb.Append("</br>");
            sb.Append("</br>");

            sb.Append("<table style='border:0px solid black; font-size:10px;'>" +
                       "<tr>"+
                      "<td style='width:90%;' align='center'>--------------------------------------------</td>" +
                      "</tr>" +
                      "<tr>" +
                      "<td style='width:90%;' align='center'>" + customer.GetFullName() + "</td>" +
                      "</tr>" +
                      "<tr>" +
                      "<td style='width:90%;' align='center'>DNI:" + customer.GetAttribute<string>(SystemCustomerAttributeNames.AdmCode) + "</td>" +
                      "</tr>" +
                      "</table>");

            return sb.ToString();
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("exportexcel")]
        public ActionResult ExportExcel(LoanPaymentListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLoans))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(model.CustomerId);
            var loan = _loanService.GetLoanById(model.LoanId);
            var reportContributionPayment = _loanService.GetReportLoanPayment(model.LoanId);
            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    _exportManager.ExportReportLoanPaymentToXlsx(stream, customer, loan, reportContributionPayment);
                    bytes = stream.ToArray();
                }
                //Response.ContentType = "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //Response.AddHeader("content-disposition", "attachment; filename=Aportaciones.xlsx");
                return File(bytes, "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Apoyo Economico.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("exportexcelKardex")]
        public ActionResult ExportExcelKardex(LoanPaymentListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLoans))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(model.CustomerId);
            var loan = _loanService.GetLoanById(model.LoanId);
            var reportLoanPayment = _loanService.GetReportLoanPaymentKardex(model.LoanId);
            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    _exportManager.ExportReportLoanPaymentKardexToXlsx(stream, customer, loan, reportLoanPayment);
                    bytes = stream.ToArray();
                }
                //Response.ContentType = "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //Response.AddHeader("content-disposition", "attachment; filename=Aportaciones.xlsx");
                return File(bytes, "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Apoyo Economico.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        #endregion

        #region CustomPayment

        public ActionResult CreateCustomPayment(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLoans))
                return AccessDeniedView();

            var loan = _loanService.GetLoanById(id);

            if (!loan.IsAuthorized)
            {
                ErrorNotification("No se puede realizar pagos debido a que el Apoyo Social Económico no se encuentra aprobado");
                return RedirectToAction("Edit", new { id = id });
            }

            var allPayment = _loanService.GetAllPayments(loanId: id, stateId: (int)LoanState.Pendiente);
            var amountToCancel = allPayment.Sum(x => x.MonthlyQuota);
            var loanPaymentModel = new LoanPaymentsModel
            {
                Banks = _bankSettings.PrepareBanks(),
                LoanId = id,
                CustomerId = loan.CustomerId,
                AmountToCancel = amountToCancel
            };

            return View(loanPaymentModel);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult CreateCustomPayment(LoanPaymentsModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLoans))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return View(model);

            var loan = _loanService.GetLoanById(model.LoanId);
            var allPayment = _loanService.GetAllPayments(model.LoanId, stateId: (int)LoanState.Pendiente);

            #region Payed more then One Quota

            var countQuotas = 0;
            var scheduledDateLast = allPayment.Max(x => x.ScheduledDateOnUtc);
            var minQuota = allPayment.Min(x => x.Quota);
            var maxQuota = allPayment.Max(x => x.Quota);
            foreach (var payment in allPayment)
            {
                if (payment.MonthlyQuota <= model.MonthlyPayed)
                {
                    countQuotas++;
                    payment.IsAutomatic = false;
                    payment.MonthlyPayed = payment.MonthlyQuota;
                    payment.StateId = (int)LoanState.PagoPersonal;
                    payment.BankName = GetBank(model.BankName);
                    payment.AccountNumber = model.AccountNumber;
                    payment.TransactionNumber = model.TransactionNumber;
                    payment.Reference = model.Reference;
                    payment.Description = "Couta liquidada por el adelanto realizado en la couta N° " + minQuota;
                    payment.ProcessedDateOnUtc = DateTime.UtcNow;

                    _loanService.UpdateLoanPayment(payment);

                    loan = _loanService.GetLoanById(model.LoanId);

                    loan.UpdatedOnUtc = DateTime.UtcNow;
                    loan.TotalPayed += loan.MonthlyQuota;
                    loan.IsDelay = false;
                    if (loan.TotalAmount == loan.TotalPayed)
                        loan.Active = false;

                    _loanService.UpdateLoan(loan);

                    model.MonthlyPayed -= payment.MonthlyQuota;
                }
                else
                {
                    if (model.MonthlyPayed > 0)
                    {
                        countQuotas++;

                        #region Se paga el pucho

                        payment.IsAutomatic = false;
                        payment.MonthlyPayed = model.MonthlyPayed;
                        payment.StateId = (int)LoanState.PagoPersonal;
                        payment.BankName = GetBank(model.BankName);
                        payment.AccountNumber = model.AccountNumber;
                        payment.TransactionNumber = model.TransactionNumber;
                        payment.Reference = model.Reference;
                        payment.Description = "Couta pagada parcialmente por el adelanto realizado en la couta N° " +
                                              minQuota;
                        payment.ProcessedDateOnUtc = DateTime.UtcNow;

                        _loanService.UpdateLoanPayment(payment);

                        loan = _loanService.GetLoanById(model.LoanId);

                        loan.UpdatedOnUtc = DateTime.UtcNow;
                        loan.TotalPayed += model.MonthlyPayed;
                        loan.IsDelay = false;

                        #endregion

                        #region Se ingresa una couta al final

                        var newQupta = loan.MonthlyQuota - model.MonthlyPayed;

                        var lastLoanPayment = new LoanPayment
                        {
                            IsAutomatic = true,
                            StateId = (int)LoanState.Pendiente,
                            LoanId = model.LoanId,
                            Quota = maxQuota + 1,
                            MonthlyCapital = Math.Round((newQupta / loan.MonthlyQuota) * payment.MonthlyCapital, 2),
                            MonthlyFee = Math.Round((newQupta / loan.MonthlyQuota) * payment.MonthlyFee, 2),
                            MonthlyPayed = 0,
                            MonthlyQuota = newQupta,
                            ScheduledDateOnUtc = scheduledDateLast.AddMonths(1 - countQuotas),
                            Description =
                                "Nueva Couta creada debido al pago personal realizado el : " +
                                DateTime.Now.ToString(CultureInfo.InvariantCulture)
                        };

                        _loanService.InsertLoanPayment(lastLoanPayment);
                        model.MonthlyQuota = 0;

                        #endregion

                        model.MonthlyPayed = 0;
                    }
                    else
                    {
                        payment.ScheduledDateOnUtc = payment.ScheduledDateOnUtc.AddMonths(countQuotas * -1);
                        payment.IsAutomatic = true;
                        payment.Description = "Couta adelantada debido al pago parcial realizado en el cuota N° " + minQuota;

                        _loanService.UpdateLoanPayment(payment);
                    }
                }
            }

            if ((model.MonthlyPayed) > 0)
            {
                var lastLoanPayment = new LoanPayment
                {
                    IsAutomatic = true,
                    StateId = (int)LoanState.Devolucion,
                    LoanId = model.LoanId,
                    Quota = maxQuota + 1,
                    MonthlyCapital = 0,
                    MonthlyFee = 0,
                    MonthlyPayed = 0,
                    MonthlyQuota = model.MonthlyPayed * -1,
                    ScheduledDateOnUtc = DateTime.UtcNow,
                    ProcessedDateOnUtc = DateTime.UtcNow,
                    Description = "Devolucion debido al pago personal realizado el: " + DateTime.Now,
                    BankName = "ACMR",
                    AccountNumber = "ACMR",
                    TransactionNumber = "ACMR",
                    Reference = "Reembolso del Apoyo económico cancelado con anticipación"
                };

                _loanService.InsertLoanPayment(lastLoanPayment);

                var returnPayment = new ReturnPayment
                {
                    AmountToPay = model.MonthlyPayed,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    PaymentNumber = loan.LoanNumber,
                    StateId = (int)ReturnPaymentState.Creado,
                    ReturnPaymentTypeId = (int)ReturnPaymentType.ApoyoEconomico,
                    CustomerId = loan.CustomerId
                };

                _returnPaymentService.InsertReturnPayment(returnPayment);

                #region Flow - Approval required

                _workFlowService.InsertWorkFlow(new WorkFlow
                {
                    CustomerCreatedId = _workContext.CurrentCustomer.Id,
                    EntityId = loan.LoanNumber,
                    EntityName = "ReturnPayment",
                    RequireCustomer = false,
                    RequireSystemRole = true,
                    SystemRoleApproval = SystemCustomerRoleNames.Employee,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    Active = true,
                    Title = "Nueva Devolución",
                    Description = "Se requiere revision para una devolucion realizada por el pago del apoyo social economico N° " + loan.LoanNumber,
                    GoTo = "ReturnPayment/List/"
                });
                #endregion
            }

            #endregion

            SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Loans.Updated"));

            if (continueEditing)
            {
                return RedirectToAction("CreateCustomPayment", new { id = model.LoanId });
            }
            return RedirectToAction("Edit", new { id = model.LoanId });

        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual LoanPaymentsModel PrepareLoanPayment(LoanPayment loanPayment)
        {
            var model = loanPayment.ToModel();
            model.Banks = _bankSettings.PrepareBanks();
            model.ScheduledDateOn = _dateTimeHelper.ConvertToUserTime(loanPayment.ScheduledDateOnUtc, DateTimeKind.Utc);
            if (loanPayment.ProcessedDateOnUtc.HasValue)
                model.ProcessedDateOn = _dateTimeHelper.ConvertToUserTime(loanPayment.ProcessedDateOnUtc.Value, DateTimeKind.Utc);
            var state = LoanState.Pendiente.ToSelectList()
                .FirstOrDefault(x => x.Value == loanPayment.StateId.ToString());
            if (state != null)
                model.State = state.Text;

            return model;
        }

        [NonAction]
        protected virtual void CloseLoanPayment(IList<LoanPayment> allPayment)
        {
            foreach (var payment in allPayment)
            {
                payment.IsAutomatic = false;
                payment.StateId = (int)LoanState.Anulado;
                payment.ProcessedDateOnUtc = DateTime.UtcNow;
                payment.Description = "El monto fue cancelado debido a un pago de personal";
                _loanService.UpdateLoanPayment(payment);
            }
        }


        [NonAction]
        protected virtual string GetBank(string account)
        {
            if (_bankSettings.AccountNumber1.Equals(account))
                return _bankSettings.NameBank1;
            if (_bankSettings.AccountNumber2.Equals(account))
                return _bankSettings.NameBank2;
            if (_bankSettings.AccountNumber3.Equals(account))
                return _bankSettings.NameBank3;
            if (_bankSettings.AccountNumber4.Equals(account))
                return _bankSettings.NameBank4;
            if (_bankSettings.AccountNumber5.Equals(account))
                return _bankSettings.NameBank5;

            return string.Empty;
        }

        #endregion
    }
}