using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Ks.Admin.Extensions;
using Ks.Admin.Models.Contract;
using Ks.Core;
using Ks.Core.Domain.Common;
using Ks.Core.Domain.Contract;
using Ks.Core.Domain.Customers;
using Ks.Services.Common;
using Ks.Services.Contract;
using Ks.Services.Customers;
using Ks.Services.ExportImport;
using Ks.Services.Helpers;
using Ks.Services.Localization;
using Ks.Services.Logging;
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
        private readonly ILoanService _loanService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IExportManager _exportManager;
        private readonly IReturnPaymentService _returnPaymentService;
        private readonly IWorkContext _workContext;

        private readonly BankSettings _bankSettings;

        #endregion

        #region Constructors

        public LoansController(IPermissionService permissionService,
            ILoanService loanService, ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            IDateTimeHelper dateTimeHelper, ICustomerActivityService customerActivityService,
            ILocalizationService localizationService, IExportManager exportManager,
            BankSettings bankSettings, IWorkContext workContext,
            IReturnPaymentService returnPaymentService)
        {
            _permissionService = permissionService;
            _loanService = loanService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _dateTimeHelper = dateTimeHelper;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _exportManager = exportManager;
            _bankSettings = bankSettings;
            _workContext = workContext;
            _returnPaymentService = returnPaymentService;
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

            var contributionsModel = loans.Select(x =>
            {
                var toModel = x.ToModel();
                toModel.CustomerCompleteName = x.Customer.GetFullName();
                toModel.CustomerDni = x.Customer.GetAttribute<string>(SystemCustomerAttributeNames.Dni);
                toModel.CustomerAdmCode = x.Customer.GetAttribute<string>(SystemCustomerAttributeNames.AdmCode);
                toModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
                if (x.UpdatedOnUtc.HasValue)
                    toModel.UpdatedOn = _dateTimeHelper.ConvertToUserTime(x.UpdatedOnUtc.Value, DateTimeKind.Utc);
                return toModel;
            });

            var gridModel = new DataSourceResult
            {
                Data = contributionsModel,
                Total = loans.TotalCount
            };

            return Json(gridModel);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageLoans))
                return AccessDeniedView();

            var loan = _loanService.GetLoanById(id);
            var model = new LoanPaymentListModel
            {
                LoanId = id,
                CustomerId = loan.CustomerId,
                States = LoanState.EnProceso.ToSelectList(false).ToList(),
                Banks = _bankSettings.PrepareBanks(),
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
            var loanPayments = _loanService.GetAllPayments(loan.Id);
            foreach (var payment in loanPayments)
            {
                if (payment.StateId == (int)LoanState.Pendiente)
                {
                    payment.StateId = (int)LoanState.Cancelado;
                    payment.Description = "Apoyo economico cancelado por el usuario " +
                                          _workContext.CurrentCustomer.GetFullName();
                    payment.ProcessedDateOnUtc = DateTime.UtcNow;
                    _loanService.UpdateLoanPayment(payment);
                }
            }
            loan.Active = false;
            loan.UpdatedOnUtc = DateTime.UtcNow;
            _loanService.UpdateLoan(loan);
            SuccessNotification("El apoyo social ha sido cancelado correctamente");
            return RedirectToAction("List");
        }

        #endregion

        #region CreatePayment

        public ActionResult CreatePayment(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributions))
                return AccessDeniedView();

            var contributionPayment = _loanService.GetPaymentById(id);
            if (!contributionPayment.Loan.Active)
            {
                ErrorNotification(_localizationService.GetResource("Admin.Customers.Loans.ValidActive"));
                return RedirectToAction("Edit", new { id = contributionPayment.Loan.Id });
            }

            var model = PrepareLoanPayment(contributionPayment);
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

            var allPayment = _loanService.GetAllPayments(loanId: id, stateId: (int)LoanState.Pendiente);
            var amountToCancel = allPayment.Sum(x => x.MonthlyQuota);
            var loanPaymentModel = new LoanPaymentsModel
            {
                Banks = _bankSettings.PrepareBanks(),
                LoanId = id,
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
                    PaymentNumber = maxQuota + 1,
                    ReturnPaymentTypeId = (int)ReturnPaymentType.ApoyoEconomico,
                    CustomerId = model.CustomerId
                };

                _returnPaymentService.InsertReturnPayment(returnPayment);
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