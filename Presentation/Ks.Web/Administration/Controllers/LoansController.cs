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
    public class LoansController : BaseAdminController
    {
        #region Constructors

        public LoansController(IPermissionService permissionService, IKsSystemService ksSystemService,
            ISettingService settingService, ILoanService loanService, ICustomerService customerService,
            IGenericAttributeService genericAttributeService, IDateTimeHelper dateTimeHelper,
            ICustomerActivityService customerActivityService, ILocalizationService localizationService,
            IExportManager exportManager, IReturnPaymentService returnPaymentService, IWorkContext workContext,
            IWorkFlowService workFlowService, BankSettings bankSettings)
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
                    new SelectListItem {Value = "-1", Text = "Todos"},
                    new SelectListItem {Value = "0", Text = "Inactivos"},
                    new SelectListItem {Value = "1", Text = "Activos"}
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
            }).OrderByDescending(x => x.Id);

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
                LoanNumber = loan.LoanNumber,
                MonthlyQuota = loan.MonthlyQuota,
                TotalAmount = loan.TotalAmount,
                TotalPayed = loan.TotalPayed,
                LoanAmount = loan.LoanAmount,
                States = LoanState.EnProceso.ToSelectList(false).ToList(),
                Banks = _bankSettings.PrepareBanks(),
                CustomerName = customer.GetFullName(),
                CustomerAdminCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.AdmCode),
                CustomerDni = customer.GetAttribute<string>(SystemCustomerAttributeNames.Dni),
                CustomerFrom = _dateTimeHelper.ConvertToUserTime(loan.CreatedOnUtc, DateTimeKind.Utc),
                Types = new List<SelectListItem>
                {
                    new SelectListItem {Value = "0", Text = "--------------", Selected = true},
                    new SelectListItem {Value = "2", Text = "Automatico"},
                    new SelectListItem {Value = "1", Text = "Manual"}
                }
            };

            model.States.Insert(0, new SelectListItem {Value = "0", Text = "--------------", Selected = true});

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

            var loanPayments = _loanService.GetAllPayments(model.LoanId, model.Quota, model.StateId, model.BankName,
                type, pageIndex: command.Page - 1,
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

            if (!loan.IsAuthorized) {
                _loanService.DeleteLoan(loan,false);
                SuccessNotification("El apoyo social ha sido eliminado del sistema correctamente");
            }
            else
            {
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
            }
            
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
                new SelectListItem {Text = "-------------------", Value = "0"},
                new SelectListItem {Text = "Aprobar", Value = "1"},
                new SelectListItem {Text = "Desaprobar", Value = "2"}
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Approval(LoanModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ApprovalLoan))
                return AccessDeniedView();

            var loan = _loanService.GetLoanById(model.Id);

            //The loan was approval by manager
            if (model.StateId == 1)
            {
                loan.IsAuthorized = true;
                loan.ApprovalOnUtc = DateTime.UtcNow;
                _loanService.UpdateLoan(loan);
                _customerActivityService.InsertActivity("LoanApprobaled",
                    string.Format(
                        "Se ha relizado la aprobacion del Apoyo Socia Económico N° {0} por el monto de  {1} para el asociado {2} ",
                        loan.LoanNumber, loan.LoanAmount.ToString("C"),
                        _customerService.GetCustomerById(loan.CustomerId).GetFullName()));

                #region Flow - Approvaled

                //The loan is approvated and the next step is to write a number of check
                _workFlowService.InsertWorkFlow(new WorkFlow
                {
                    CustomerCreatedId = _workContext.CurrentCustomer.Id,
                    EntityId = loan.Id,
                    EntityName = CommonHelper.GetKsCustomTypeConverter(typeof(Loan)).ConvertToInvariantString(new Loan()),
                    RequireCustomer = false,
                    RequireSystemRole = true,
                    SystemRoleApproval = SystemCustomerRoleNames.Employee,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    Active = true,
                    Title = "Aprobado el Apoyo Social Económico",
                    Description =
                        "Se ha realizado la aprobacion para la emision del Apoyo Social Económico N° " + loan.LoanNumber +
                        " para el asociado " + _customerService.GetCustomerById(loan.CustomerId).GetFullName(),
                    GoTo = "Admin/Loans/AddCheck/" + loan.Id
                });

                #endregion
            }

            //The loan was no approval by manager
            if (model.StateId == 2)
            {
                loan.IsAuthorized = false;
                loan.ApprovalOnUtc = null;
                loan.UpdatedOnUtc = DateTime.UtcNow;
                loan.Active = false;
                var details = _loanService.GetAllPayments(model.Id, stateId: 1);
                foreach (var detail in details)
                {
                    detail.StateId = (int) LoanState.Cancelado;
                    detail.ProcessedDateOnUtc = DateTime.UtcNow;
                    _loanService.UpdateLoanPayment(detail);
                }
                _loanService.UpdateLoan(loan);
                _customerActivityService.InsertActivity("LoanNoApprobaled",
                    string.Format(
                        "No se ha relizado la aprobacion del Apoyo Socia Económico N° {0} por el monto de  {1} para el asociado {2} ",
                        loan.LoanNumber, loan.LoanAmount.ToString("C"),
                        _customerService.GetCustomerById(loan.CustomerId).GetFullName()));

                #region Flow - NO Approvaled

                _workFlowService.InsertWorkFlow(new WorkFlow
                {
                    CustomerCreatedId = _workContext.CurrentCustomer.Id,
                    EntityId = loan.Id,
                    EntityName = CommonHelper.GetKsCustomTypeConverter(typeof (Loan)).ConvertToInvariantString(new Loan ()),
                    RequireCustomer = false,
                    RequireSystemRole = true,
                    SystemRoleApproval = SystemCustomerRoleNames.Employee,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    Active = true,
                    Title = "No aprobado el Apoyo Social Económico",
                    Description =
                        "Se ha realizado la no aprobacion para la emision del Apoyo Social Económico N° " +
                        loan.LoanNumber +
                        " para el asociado " + _customerService.GetCustomerById(loan.CustomerId).GetFullName(),
                    GoTo = "Admin/Loans/Edit/" + loan.Id
                });

                #endregion
            }

            #region close preview Flow

            _workFlowService.CloseWorkFlow(loan.Id,
                CommonHelper.GetKsCustomTypeConverter(typeof (Loan)).
                    ConvertToInvariantString(new Loan()), SystemCustomerRoleNames.Manager);

            #endregion

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
            loan.Active = true;
            loan.AccountNumber = model.AccountNumber;
            loan.CheckNumber = model.CheckNumber;

            _loanService.UpdateLoan(loan);

            _customerActivityService.InsertActivity("AddChekToLoan",
                "Se ha asignado el cheque Nº " + model.CheckNumber + " al apoyo social economico  N° " + loan.LoanNumber);

            #region close preview Flow

            _workFlowService.CloseWorkFlow(loan.Id,
                CommonHelper.GetKsCustomTypeConverter(typeof (Loan)).
                    ConvertToInvariantString(new Loan()), SystemCustomerRoleNames.Employee);

            #endregion

            return RedirectToAction("Edit", new {id = loan.Id});
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
                ErrorNotification(
                    "No se puede realizar pagos debido a que el Apoyo Social Económico no se encuentra aprobado");
                return RedirectToAction("Edit", new {id = loan.Id});
            }

            if (!loan.Active)
            {
                ErrorNotification(_localizationService.GetResource("Admin.Customers.Loans.ValidActive"));
                return RedirectToAction("Edit", new {id = loan.Id});
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

            if (loanPayment.StateId != (int) LoanState.Pendiente)
            {
                ErrorNotification(_localizationService.GetResource("Admin.Customers.Loans.ValidPayment"));
                return RedirectToAction("CreatePayment", new {id = loanPayment.Id});
            }

            if (!ModelState.IsValid)
                return View(model);

            loanPayment.IsAutomatic = false;
            loanPayment.StateId = (int) LoanState.Pagado;
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
                return RedirectToAction("CreatePayment", new {id = loanPayment.Id});
            }
            return RedirectToAction("Edit", new {id = loanPayment.LoanId});
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
            const String IMAGE =
                "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";

            var sb = new StringBuilder();

            var img = "</br>";

            img += "<img style='display:block; width:250px;height:80px;' id='base64image'" +
                   "src='data:image/jpeg;base64, " + IMAGE + "' />" +
                   "</br>" +
                   "<table style='font-family:arial; font-size:10px;'>" +
                   "<tr>" +
                   "<td align='center'  rowspan='4' style='width:30%;'>AUXILIO COOPERATIVO MILITAR DE RETIROS</td>" +
                   "</tr>" +
                   "<tr>" +
                   "<td align='center'  style='width:30%;'>  Jirón Cervantes N°197 </td>" +
                   "</tr>" +
                   "<tr>" +
                   "<td align='center'  rowspan='4' style='width:30%;'>Cercado de Lima</td>" +
                   "</tr>" +
                   "</table>" +
                   "</br>";

            sb.Append(img);


            //Generando las cabeceras
            sb.Append("</br></br>");
            sb.Append("<table style='font-family:arial; font-size:10px; width:100%;' cellspacing='5' cellpadding='5'>");
            sb.Append("<tr>" +
                      "<td align='center' colspan='3'>" +
                      "<h2>N° Orden de Pago " + loan.LoanNumber + "</h2>" +
                      "</td>" +
                      "<td align='right'>" +
                      (loan.ApprovalOnUtc.HasValue ? loan.ApprovalOnUtc.Value.ToShortDateString() + "." : "") + "</td>" +
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
                      "<td style='width:40%;'> " + customer.GetAttribute<string>(SystemCustomerAttributeNames.AdmCode) +
                      " </td>" +
                      "<td rowspan='4' style='width:30%;'><b>N°Cheque:</b></td>" +
                      "<td style='width:40%;'> " + loan.CheckNumber + " </td>" +
                      "</tr>" +
                      "</table>" +
                      "</td>" +
                      "</tr>");
            sb.Append("</table>");

            sb.Append("</br></br>");
            sb.Append("<p style='font-family:arial; font-size:10px;'><b>Apoyo Social Económico " +
                      loan.LoanAmount.ToString("C") + " Plazo: " + loan.Period + " meses</b></p>");
            sb.Append("<table style='border:1px solid black; font-size:10px;'>" +
                      "<tr>" +
                      "<td style='width:60%; border-bottom: 1px solid #ddd;'>Descripcion de la cuenta</td>" +
                      "<td style='width:15%; border-bottom: 1px solid #ddd;'>Cargo</td>" +
                      "<td style='width:15%; border-bottom: 1px solid #ddd;' align='right'>Abono</td>" +
                      "</tr>" +
                      "<tr>" +
                      "<td style='width:60%;'>MONTO TOTAL CON INTERESES</td>" +
                      "<td style='width:15%;'> " + loan.TotalAmount.ToString("C") + "</td>" +
                      "<td style='width:15%;'> </td>" +
                      "</tr>" +
                      "<tr>" +
                      "<td style='width:60%;'>SEGURO DESGRAVAMEN " + loan.Safe + "%</td>" +
                      "<td style='width:15%;'></td>" +
                      "<td style='width:15%;' align='right'> " + loan.TotalSafe.ToString("C") + "</td>" +
                      "</tr>" +
                      "<tr>" +
                      "<td style='width:60%;'>INTERESES TOTALES DIFERIDOS</td>" +
                      "<td style='width:15%;'></td>" +
                      "<td style='width:15%;' align='right'>" + loan.TotalFeed.ToString("C") + "</td>" +
                      "</tr>" +
                      "<tr>" +
                      "<td style='width:60%;'>CTA CTE MN N° " + loan.AccountNumber + "</td>" +
                      "<td style='width:15%;'></td>" +
                      "<td style='width:15%;' align='right'>" + loan.TotalToPay.ToString("C") + "</td>" +
                      "</tr>" +
                      "<tr  '>" +
                      "<td style='width:60%; border-top: 1px solid ;' align='right'>Total Soles</td>" +
                      "<td style='width:15%; border-top: 1px solid black;'>" + loan.TotalAmount.ToString("C") + "</td>" +
                      "<td style='width:15%; border-top: 1px solid black;' align='right'>" +
                      loan.TotalAmount.ToString("C") + "</td>" +
                      "</tr>" +
                      "</table>");
            sb.Append("</br>");

            sb.Append("<table style='border:1px solid black; font-size:10px;'>" +
                      "<tr>" +
                      "<td style='width:60%;'>Importe a descontarse en: " + loan.Period + "</td>" +
                      "<td style='width:30%;'>Cuotas Mensuales:" + loan.MonthlyQuota.ToString("C") + " </td>" +
                      "</tr>" +
                      "</table>");


            sb.Append("</br>");
            sb.Append("</br>");
            sb.Append("</br>");

            var setting = _settingService.LoadSetting<SignatureSettings>();
            var str = "<table style='border:0px solid black; font-size:10px;'>" +
                      "<tr style='color:white;'><td>.</td><td>.</td></tr>" +
                      "<tr style='color:white;' ><td>.</td><td>.</td></tr>" +
                      "<tr style='color:white;' ><td>.</td><td>.</td></tr>" +
                      "<tr style='color:white;' ><td>.</td><td>.</td></tr>" +
                      "<tr style='color:white;' ><td>.</td><td>.</td></tr>" +
                      "<tr style='color:white;' ><td>.</td><td>.</td></tr>" +
                      "<tr style='color:white;' ><td>.</td><td>.</td></tr>" +
                      "<tr>" +
                      "<td style='width:45%;' align='center'>" +
                      (setting.ShowBenefitCenter ? setting.BenefitCenterName.GetLine() : "") + "</td>" +
                      "<td style='width:45%;' align='center'>" +
                      (setting.ShowBenefitLeft ? setting.BenefitLeftName.GetLine() : "") + "</td>" +
                      "</tr>" +
                      "<tr>" +
                      "<td style='width:45%;' align='center'>" +
                      (setting.ShowBenefitCenter ? setting.BenefitCenterName : "") + "</td>" +
                      "<td style='width:45%;' align='center'>" +
                      (setting.ShowBenefitLeft ? setting.BenefitLeftName : "") + "</td>" +
                      "</tr>" +
                      "<tr>" +
                      "<td style='width:45%;' align='center'>" +
                      (setting.ShowBenefitCenter ? setting.BenefitCenterPosition : "") + "</td>" +
                      "<td style='width:45%;' align='center'>" +
                      (setting.ShowBenefitLeft ? setting.BenefitLeftPosition : "") + "</td>" +
                      "</tr>" +
                      "<tr style='color:white;'><td>.</td><td>.</td></tr>" +
                      "<tr style='color:white;' ><td>.</td><td>.</td></tr>" +
                      "<tr style='color:white;' ><td>.</td><td>.</td></tr>" +
                      "<tr style='color:white;' ><td>.</td><td>.</td></tr>" +
                      "<tr style='color:white;' ><td>.</td><td>.</td></tr>" +
                      "<tr style='color:white;' ><td>.</td><td>.</td></tr>" +
                      "<tr style='color:white;' ><td>.</td><td>.</td></tr>" +
                      "<tr>" +
                      "<td style='width:45%;' align='center'>" +
                      (setting.ShowLoanRight ? setting.LoanRightName.GetLine() : "") + "</td>" +
                      "<td style='width:45%;' align='center'>" +
                      (setting.ShowLoanLeft ? setting.LoanLeftName.GetLine() : "") + "</td>" +
                      "</tr>" +
                      "<tr>" +
                      "<td style='width:45%;' align='center'>" + (setting.ShowLoanRight ? setting.LoanRightName : "") +
                      "</td>" +
                      "<td style='width:45%;' align='center'>" + (setting.ShowLoanLeft ? setting.LoanLeftName : "") +
                      "</td>" +
                      "</tr>" +
                      "<tr>" +
                      "<td style='width:45%;' align='center'>" +
                      (setting.ShowLoanRight ? setting.LoanRightPosition : "") + "</td>" +
                      "<td style='width:45%;' align='center'>" + (setting.ShowLoanLeft ? setting.LoanLeftPosition : "") +
                      "</td>" +
                      "</tr>" +
                      "</table>";

            sb.Append(str);

            sb.Append("</br>");
            sb.Append("</br>");
            sb.Append("</br>");
            sb.Append("</br>");
            sb.Append("</br>");
            sb.Append("</br>");
            sb.Append("</br>");

            var strCustomer = "<table style='border:0px solid black; font-size:10px;'>" +
                              "<tr>" +
                              "<td style='width:45%;' align='center'>" + customer.GetFullName().GetLine() + "</td>" +
                              "</tr>" +
                              "<tr>" +
                              "<td style='width:45%;' align='center'>" + customer.GetFullName() + "</td>" +
                              "</tr>" +
                              "<tr>" +
                              "<td style='width:45%;' align='center'>DNI:" +
                              customer.GetAttribute<string>(SystemCustomerAttributeNames.Dni) + "</td>" +
                              "</tr>" +
                              "<tr>" +
                              "<td style='width:45%;' font=color:white; align='center' >.</td>" +
                              "</tr>" +
                              "<tr>" +
                              "<td style='width:45%;' align='center'>Fecha: ......../......../................ </td>" +
                              "</tr>" +
                              "</table>";

            var strSignature = "<table style='width:30%; border:2px solid black; font-size:10px; font=color:white;'>" +
                               "<tr style='color:white;'><td>.</td><td>.</td><td>.</td><td>.</td><td>.</td><td>.</td><td>.</td></tr>" +
                               "<tr style='color:white;' ><td>.</td><td>.</td><td>.</td><td>.</td><td>.</td><td>.</td><td>.</td></tr>" +
                               "<tr style='color:white;' ><td>.</td><td>.</td><td>.</td><td>.</td><td>.</td><td>.</td><td>.</td></tr>" +
                               "<tr style='color:white;'><td>.</td><td>.</td><td>.</td><td>.</td><td>.</td><td>.</td><td>.</td></tr>" +
                               "<tr style='color:white;'><td>.</td><td>.</td><td>.</td><td>.</td><td>.</td><td>.</td><td>.</td></tr>" +
                               "<tr style='color:white;'><td>.</td><td>.</td><td>.</td><td>.</td><td>.</td><td>.</td><td>.</td></tr>" +
                               "<tr style='color:white;'><td>.</td><td>.</td><td>.</td><td>.</td><td>.</td><td>.</td><td>.</td></tr>" +
                               "</table>";

            sb.Append("<table style='border:0px solid black; font-size:10px;'>" +
                      "<tr>" +
                      "<td style='width:55%;' align='center' >" + strCustomer + "</td>" +
                      "<td style='width:45%;' align='center' >" + strSignature + "</td>" +
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
                return File(bytes, "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Apoyo Economico.xlsx");
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
                return File(bytes, "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Apoyo Economico.xlsx");
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
                ErrorNotification(
                    "No se puede realizar pagos debido a que el Apoyo Social Económico no se encuentra aprobado");
                return RedirectToAction("Edit", new {id});
            }

            var allPayment = _loanService.GetAllPayments(id, stateId: (int) LoanState.Pendiente);
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

            var loan = _loanService.GetLoanById(model.LoanId);
            var allPayment = _loanService.GetAllPayments(model.LoanId, stateId: (int)LoanState.Pendiente);

            if (!ModelState.IsValid)
            {
                var amountToCancel = allPayment.Sum(x => x.MonthlyQuota);
                model = new LoanPaymentsModel
                {
                    Banks = _bankSettings.PrepareBanks(),
                    LoanId = loan.Id,
                    CustomerId = loan.CustomerId,
                    AmountToCancel = amountToCancel
                };
                return View(model);
            } 

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
                    payment.StateId = (int) LoanState.PagoPersonal;
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
                    if (loan.TotalAmount <= loan.TotalPayed)
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
                        payment.StateId = (int) LoanState.PagoParcial;
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

                        #region Se ingresa una couta al final (Improbable)

                        var newQuota = loan.MonthlyQuota - model.MonthlyPayed;

                        var lastLoanPayment = new LoanPayment
                        {
                            IsAutomatic = true,
                            StateId = (int) LoanState.Pendiente,
                            LoanId = model.LoanId,
                            Quota = maxQuota + 1,
                            MonthlyCapital = Math.Round((newQuota/loan.MonthlyQuota)*payment.MonthlyCapital, 2),
                            MonthlyFee = Math.Round((newQuota/loan.MonthlyQuota)*payment.MonthlyFee, 2),
                            MonthlyPayed = 0,
                            MonthlyQuota = newQuota,
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
                    else if (model.MonthlyPayed == 0)
                    {
                        //ADELANTO DE FUTURAS COUTAS
                        payment.ScheduledDateOnUtc = payment.ScheduledDateOnUtc.AddMonths(countQuotas * -1);
                        payment.IsAutomatic = true;
                        payment.Description = "Error Couta adelantada debido al pago parcial realizado en el cuota N° " +
                                              minQuota;

                        _loanService.UpdateLoanPayment(payment);
                    }
                    else
                    {
                        //NEGATIVO, COSA QUE NUNCA DEBE PASAR 
                        break;
                    }
                }
            }

            //Return Payment
            if (model.MonthlyPayed > 0)
            {
                var lastLoanPayment = new LoanPayment
                {
                    IsAutomatic = true,
                    StateId = (int) LoanState.Devolucion,
                    LoanId = model.LoanId,
                    Quota = maxQuota + 1,
                    MonthlyCapital = 0,
                    MonthlyFee = 0,
                    MonthlyPayed = 0,
                    MonthlyQuota = model.MonthlyPayed*-1,
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
                    StateId = (int) ReturnPaymentState.Creado,
                    ReturnPaymentTypeId = (int) ReturnPaymentType.ApoyoEconomico,
                    CustomerId = loan.CustomerId
                };

                _returnPaymentService.InsertReturnPayment(returnPayment);

                #region Flow - Approval required

                var customer = _customerService.GetCustomerById(loan.CustomerId);

                _workFlowService.InsertWorkFlow(new WorkFlow
                {
                    CustomerCreatedId = _workContext.CurrentCustomer.Id,
                    EntityId = returnPayment.Id,
                    EntityName =CommonHelper.GetKsCustomTypeConverter(typeof (ReturnPayment)).ConvertToInvariantString(new ReturnPayment()),
                    RequireCustomer = false,
                    RequireSystemRole = true,
                    SystemRoleApproval = SystemCustomerRoleNames.Employee,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    Active = true,
                    Title = "Nueva Devolución",
                    Description ="Se requiere revision para una devolucion realizada por el pago del apoyo social economico N° " +
                        loan.LoanNumber +", para el asociado " + customer.GetFullName() + ", con DNI: " +
                        customer.GetAttribute<string>(SystemCustomerAttributeNames.Dni) +
                        " y con CIP: " + customer.GetAttribute<string>(SystemCustomerAttributeNames.AdmCode),
                    GoTo = "Admin/ReturnPayment/Edit/" + returnPayment.Id
                });

                #endregion
            }

            #endregion

            SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Loans.Updated"));

            if (continueEditing)
            {
                return RedirectToAction("CreateCustomPayment", new {id = model.LoanId});
            }
            return RedirectToAction("Edit", new {id = model.LoanId});
        }

        #endregion

        #region CreateEndPayment

        public ActionResult CreateEndPayment(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLoans))
                return AccessDeniedView();

            var loan = _loanService.GetLoanById(id);

            if (!loan.IsAuthorized)
            {
                ErrorNotification(
                    "No se puede realizar pagos debido a que el Apoyo Social Económico no se encuentra aprobado");
                return RedirectToAction("Edit", new { id });
            }

            var allPayment = _loanService.GetAllPayments(id, stateId: (int)LoanState.Pendiente);
            var amountToCancel = allPayment.Sum(x => x.MonthlyCapital);
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
        public ActionResult CreateEndPayment(LoanPaymentsModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLoans))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return View(model);

            var loan = _loanService.GetLoanById(model.LoanId);
            var allPaymentPendient = _loanService.GetAllPayments(model.LoanId, stateId: (int)LoanState.Pendiente);

            var amountToCancel = allPaymentPendient.Sum(x => x.MonthlyCapital);

            if (amountToCancel == model.MonthlyPayed)
            {
                foreach (var payment in allPaymentPendient)
                {
                    payment.MonthlyFee = 0;
                    payment.IsAutomatic = false;
                    payment.MonthlyPayed = payment.MonthlyCapital; 

                    payment.StateId = (int)LoanState.PagoPersonal;
                    payment.BankName = GetBank(model.BankName);
                    payment.AccountNumber = model.AccountNumber;
                    payment.TransactionNumber = model.TransactionNumber;
                    payment.Reference = model.Reference;
                    payment.Description = "Couta liquidada por Pago Anticipado";
                    payment.ProcessedDateOnUtc = DateTime.UtcNow;

                    _loanService.UpdateLoanPayment(payment);
                }

                var allPayments = loan.LoanPayments;
                loan.UpdatedOnUtc = DateTime.UtcNow;
                loan.TotalPayed = allPayments.Sum(x => x.MonthlyPayed);
                loan.TotalFeed = allPayments.Sum(x => x.MonthlyFee);
                loan.IsDelay = false;
                loan.Active = false;

                _loanService.UpdateLoan(loan);
            }
            else
            {
                ErrorNotification("El monto ingresado no corresponde al valor pendiente de pago");

                var id = model.Id;  
                model = new LoanPaymentsModel
                {
                    Banks = _bankSettings.PrepareBanks(),
                    LoanId = id,
                    CustomerId = loan.CustomerId,
                    AmountToCancel = amountToCancel
                };
                return View(model);
            } 

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
                model.ProcessedDateOn = _dateTimeHelper.ConvertToUserTime(loanPayment.ProcessedDateOnUtc.Value,
                    DateTimeKind.Utc);
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
                payment.StateId = (int) LoanState.Anulado;
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