using System;
using System.Collections.Generic;
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
using Ks.Services.Messages;
using Ks.Services.Security;
using Ks.Web.Framework;
using Ks.Web.Framework.Controllers;
using Ks.Web.Framework.Kendoui;
using Ks.Core.Domain.Messages;
using System.Text;

namespace Ks.Admin.Controllers
{
    public partial class ContributionsController : BaseAdminController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly IContributionService _contributionService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IBenefitService _benefitService;
        private readonly IExportManager _exportManager;
        private readonly IReturnPaymentService _returnPaymentService;
        private readonly IWorkFlowService _workFlowService;

        private readonly ContributionSettings _contributionSettings;
        private readonly BankSettings _bankSettings;
        private readonly BenefitValueSetting _benefitValueSetting;
        private readonly SequenceIdsSettings _sequenceIdsSettings;

        #endregion

        #region Constructors

        public ContributionsController(
            IWorkContext workContext,
            IPermissionService permissionService,
            IContributionService contributionService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            IDateTimeHelper dateTimeHelper,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IWorkFlowService workFlowService,
            IExportManager exportManager,
            ContributionSettings contributionSettings,
            IReturnPaymentService returnPaymentService,
            IBenefitService benefitService,
            BankSettings bankSettings,
            SequenceIdsSettings sequenceIdsSettings,
            BenefitValueSetting benefitValueSetting)
        {
            _workContext = workContext;
            _permissionService = permissionService;
            _contributionService = contributionService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _dateTimeHelper = dateTimeHelper;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _exportManager = exportManager;
            _workFlowService = workFlowService;
            _contributionSettings = contributionSettings;
            _bankSettings = bankSettings;
            _returnPaymentService = returnPaymentService;
            _benefitService = benefitService;
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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributions))
                return AccessDeniedView();

            var model = new ContributionListModel
            {
                SearchAdmCode = "",
                SearchDni = "",
                SearchAuthorizeDiscount = null,
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
        public ActionResult List(DataSourceRequest command, ContributionListModel model)
        {
            GenericAttribute generic = null;

            //1) Find By Dni and AdmCode

            if (!string.IsNullOrWhiteSpace(model.SearchDni))
                generic = _genericAttributeService.GetAttributeForKeyValue("Dni", model.SearchDni.Trim());

            if (!string.IsNullOrWhiteSpace(model.SearchAdmCode) && generic == null)
                generic = _genericAttributeService.GetAttributeForKeyValue("AdmCode", model.SearchAdmCode.Trim());

            IPagedList<Contribution> contributions = null;
            if (generic != null && string.Compare(generic.KeyGroup, "Customer", StringComparison.CurrentCulture) == 0)
                contributions = _contributionService.SearchContributionByCustomerId(generic.EntityId, model.StateId);

            //2) Find by letter Number
            if (contributions == null && model.SearchAuthorizeDiscount.HasValue)
                contributions = _contributionService.SearchContributionByAuthorizeDiscount(model.SearchAuthorizeDiscount.Value, model.StateId);

            if (contributions == null)
                contributions = new PagedList<Contribution>(new List<Contribution>(), 0, 10);

            var contributionsModel = contributions.Select(x =>
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
                Total = contributions.TotalCount
            };

            return Json(gridModel);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributions))
                return AccessDeniedView();

            var contribution = _contributionService.GetContributionById(id);
            var customer = _customerService.GetCustomerById(contribution.CustomerId);
            var military = customer.GetAttribute<int>(SystemCustomerAttributeNames.MilitarySituationId);
            var model = new ContributionPaymentListModel
            {
                ContributionId = id,
                CustomerId = contribution.CustomerId,
                States = ContributionState.EnProceso.ToSelectList(false).ToList(),
                Banks = _bankSettings.PrepareBanks(),
                CustomerName = customer.GetFullName(),
                CustomerAdminCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.AdmCode),
                CustomerDni = customer.GetAttribute<string>(SystemCustomerAttributeNames.Dni),
                CustomerFrom = contribution.CreatedOnUtc,
                AuthorizeDiscount = contribution.AuthorizeDiscount,
                MilitarySituationId = military == 1 ? "COPERE" : military == 2 ? "CPMP" : "OTROS",
                Types = new List<SelectListItem>
                {
                    new SelectListItem { Value = "0", Text = "--------------", Selected = true},
                    new SelectListItem {Value = "2", Text = "Automatico"},
                    new SelectListItem {Value = "1", Text = "Manual"}
                }
            };

            model.States.Insert(0, new SelectListItem { Value = "0", Text = "--------------", Selected = true });

            model.IsActiveAmount1 = _contributionSettings.IsActiveAmount1;
            model.NameAmount1 = _contributionSettings.NameAmount1;
            model.IsActiveAmount2 = _contributionSettings.IsActiveAmount2;
            model.NameAmount2 = _contributionSettings.NameAmount2;
            model.IsActiveAmount3 = _contributionSettings.IsActiveAmount3;
            model.NameAmount3 = _contributionSettings.NameAmount3;

            return View(model);
        }

        [HttpPost]
        public ActionResult ListContributionsPayments(DataSourceRequest command, ContributionPaymentListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributions))
                return AccessDeniedView();

            bool? type = null;
            if (model.Type == 0)
                type = null;
            if (model.Type == 2)
                type = true;
            if (model.Type == 1)
                type = false;

            var contributionPayments = _contributionService.GetAllPayments(
                contributionId: model.ContributionId, number: model.Number,
                stateId: model.StateId, accountNumber: model.BankName,
                type: type, pageIndex: command.Page - 1,
                pageSize: command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = contributionPayments.Select(x =>
                {
                    var m = x.ToModel();
                    m.ScheduledDateOn = _dateTimeHelper.ConvertToUserTime(x.ScheduledDateOnUtc, DateTimeKind.Local);
                    m.ProcessedDateOn = x.ProcessedDateOnUtc.HasValue
                        ? _dateTimeHelper.ConvertToUserTime(x.ProcessedDateOnUtc.Value, DateTimeKind.Local)
                        : x.ProcessedDateOnUtc;
                    m.Type = x.IsAutomatic ? "Automatico" : "Manual";
                    m.State = x.ContributionState.ToSelectList().FirstOrDefault(r => r.Value == x.StateId.ToString()).Text;
                    return m;
                }),
                Total = contributionPayments.TotalCount
            };

            return Json(gridModel);
        }

        public ActionResult CreatePayment(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributions))
                return AccessDeniedView();

            var contributionPayment = _contributionService.GetPaymentById(id);
            if (!contributionPayment.Contribution.Active)
            {
                ErrorNotification(_localizationService.GetResource("Admin.Customers.Contributions.ValidActive"));
                return RedirectToAction("Edit", new { id = contributionPayment.Contribution.Id });
            }

            var model = PrepareContributionPayment(contributionPayment);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult CreatePayment(ContributionPaymentsModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributions))
                return AccessDeniedView();

            var contributionPayment = _contributionService.GetPaymentById(model.Id);

            if (contributionPayment.StateId != (int)ContributionState.Pendiente)
            {
                ErrorNotification(_localizationService.GetResource("Admin.Customers.Contributions.ValidPayment"));
                return RedirectToAction("CreatePayment", new { id = contributionPayment.Id });
            }

            if (model.AmountPayed != contributionPayment.AmountTotal)
            {
                ErrorNotification(_localizationService.GetResource("Admin.Customers.Contributions.CancelPayment"));
                return RedirectToAction("CreatePayment", new { id = contributionPayment.Id });
            }

            if (!ModelState.IsValid)
                return View(model);

            contributionPayment.IsAutomatic = false;
            contributionPayment.StateId = (int)ContributionState.PagoPersonal;
            contributionPayment.ContributionId = model.ContributionId;
            contributionPayment.BankName = GetBank(model.BankName);
            contributionPayment.AccountNumber = model.AccountNumber;
            contributionPayment.TransactionNumber = model.TransactionNumber;
            contributionPayment.Reference = model.Reference;
            contributionPayment.Description = model.Description;
            contributionPayment.ProcessedDateOnUtc = DateTime.UtcNow;
            contributionPayment.AmountPayed = contributionPayment.AmountTotal;

            _contributionService.UpdateContributionPayment(contributionPayment);

            var contribution = _contributionService.GetContributionById(model.ContributionId);
            contribution.UpdatedOnUtc = DateTime.UtcNow;
            contribution.AmountPayed += model.AmountPayed;

            contribution.DelayCycles = 0;
            contribution.IsDelay = false;

            if (contributionPayment.Number == _contributionSettings.TotalCycle)
            {
                contribution.Active = false;
                _contributionService.UpdateContribution(contribution);

                var benefit = _benefitService.GetAllBenefits().Where(x => x.CloseContributions).FirstOrDefault();
                var contributionBenefit = contribution.CreateBenefit(benefit, _benefitValueSetting.AmountBaseOfBenefit,
                    _sequenceIdsSettings.NumberOfLiquidation, _contributionSettings.TotalCycle / 12);

                _benefitService.InsertContributionBenefit(contributionBenefit);
            }

            _contributionService.UpdateContribution(contribution);

            SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Contributions.Updated"));

            if (continueEditing)
            {
                return RedirectToAction("CreatePayment", new { id = contributionPayment.Id });
            }
            return RedirectToAction("Edit", new { id = contributionPayment.ContributionId });
        }

        #endregion

        #region Reports

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("exportexcel-all")]
        public ActionResult ExportExcelAll(ContributionPaymentListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributions))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(model.CustomerId);
            var contribution = _contributionService.GetContributionById(model.ContributionId);
            var reportContributionPayment = _contributionService.GetReportContributionPayment(model.ContributionId);
            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    _exportManager.ExportReportContributionPaymentToXlsx(stream, customer, contribution, reportContributionPayment);
                    bytes = stream.ToArray();
                }
                //Response.ContentType = "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //Response.AddHeader("content-disposition", "attachment; filename=Aportaciones.xlsx");
                return File(bytes, "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Aportaciones.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("exportexcel")]
        public ActionResult ExportExcel(ContributionPaymentListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributions))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(model.CustomerId);
            var contribution = _contributionService.GetContributionById(model.ContributionId);
            var reportContributionPayment = _contributionService.GetReportContributionPaymentFuture(model.ContributionId);
            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    _exportManager.ExportReportContributionPaymentToXlsx(stream, customer, contribution, reportContributionPayment);
                    bytes = stream.ToArray();
                }
                //Response.ContentType = "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //Response.AddHeader("content-disposition", "attachment; filename=Aportaciones.xlsx");
                return File(bytes, "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Aportaciones.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("exportpdf")]
        public ActionResult ExportPdf(ContributionPaymentListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributions))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(model.CustomerId);
            var contribution = _contributionService.GetContributionById(model.ContributionId);
            var reportContributionPayment = _contributionService.GetReportContributionPayment(model.ContributionId);
            try
            {
                var html = _exportManager.ExportReportContributionPaymentToPdf(customer, contribution, reportContributionPayment);
                var stream = new MemoryStream((html.ConvertHtmlToPdf()));
                return new PdfDownloadResult(stream, "Aportaciones.pdf");
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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributions))
                return AccessDeniedView();

            var allPayment = _contributionService.GetAllPayments(contributionId: id, stateId: (int)ContributionState.Pendiente);
            var amountToCancel = allPayment.Sum(x => x.AmountTotal);
            var loanPaymentModel = new ContributionPaymentsModel
            {
                Banks = _bankSettings.PrepareBanks(),
                ContributionId = id,
                AmountToCancel = amountToCancel
            };

            return View(loanPaymentModel);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult CreateCustomPayment(ContributionPaymentsModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributions))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return View(model);

            var contribution = _contributionService.GetContributionById(model.ContributionId);
            var customer = _customerService.GetCustomerById(contribution.CustomerId);
            var militarySituation = customer.GetAttribute<int>(SystemCustomerAttributeNames.MilitarySituationId);
            var allPayment = _contributionService.GetAllPayments(contributionId: model.ContributionId, stateId: (int)ContributionState.Pendiente);

            #region Payed more then One Quota

            var minNumber = allPayment.Min(x => x.Number);
            var maxNumber = allPayment.Max(x => x.Number);
            var isNextQuota = false;
            var valueNextQuotaAdd = 0.0M;
            var valueNextNumber = 0;
            var maximumCharge = 0.0M;
            if (militarySituation == 1)
                maximumCharge = _contributionSettings.MaximumChargeCopere;
            if (militarySituation == 2)
                maximumCharge = _contributionSettings.MaximumChargeCaja;
            if (militarySituation == 2)
                maximumCharge = _contributionSettings.MaximumChargeCaja;



            foreach (var payment in allPayment)
            {
                if (payment.AmountTotal <= model.AmountPayed)
                {
                    payment.IsAutomatic = false;
                    payment.AmountPayed = payment.AmountTotal;
                    payment.StateId = (int)ContributionState.PagoPersonal;
                    payment.BankName = GetBank(model.BankName);
                    payment.AccountNumber = model.AccountNumber;
                    payment.TransactionNumber = model.TransactionNumber;
                    payment.Reference = model.Reference;
                    payment.Description = "Couta liquidada por el adelanto realizado en la couta N° " + minNumber;
                    payment.ProcessedDateOnUtc = DateTime.UtcNow;

                    _contributionService.UpdateContributionPayment(payment);

                    contribution = _contributionService.GetContributionById(model.ContributionId);

                    contribution.UpdatedOnUtc = DateTime.UtcNow;
                    contribution.AmountPayed += payment.AmountPayed;
                    contribution.IsDelay = false;

                    //if (contribution.AmountPayed == payment.AmountPayed)
                    //    contribution.Active = false;
                    //Si lo cancelo ya no podre ver los beneficios

                    _contributionService.UpdateContribution(contribution);

                    model.AmountPayed -= payment.AmountPayed;
                }
                else
                {
                    //If is Zero => break
                    if (model.AmountPayed == 0)
                        break;

                    valueNextQuotaAdd = payment.AmountTotal - model.AmountPayed;
                    valueNextNumber = payment.Number + 1;
                    isNextQuota = true;

                    //just one time, 
                    payment.IsAutomatic = false;
                    payment.AmountPayed = model.AmountPayed;
                    payment.StateId = (int)ContributionState.PagoParcial;
                    payment.BankName = GetBank(model.BankName);
                    payment.AccountNumber = model.AccountNumber;
                    payment.TransactionNumber = model.TransactionNumber;
                    payment.Reference = model.Reference;
                    payment.Description = "Couta pagada parcialmente por el adelanto realizado en la couta N° " + minNumber;
                    payment.ProcessedDateOnUtc = DateTime.UtcNow;

                    _contributionService.UpdateContributionPayment(payment);

                    contribution = _contributionService.GetContributionById(model.ContributionId);

                    contribution.UpdatedOnUtc = DateTime.UtcNow;
                    contribution.AmountPayed += model.AmountPayed;
                    contribution.IsDelay = false;

                    _contributionService.UpdateContribution(contribution);

                    break;
                }
            }

            #region No sale del bucle y hay aumento de coutas

            while (isNextQuota)
            {
                var nextPayment = _contributionService.GetPaymentByContributionId(model.ContributionId).FirstOrDefault(x => x.Number == valueNextNumber);
                //Si entra aca es porque no salio del bucle y puede arreglar un caso limite cuando la couta a aumentar es la 420
                if (nextPayment != null)
                {
                    //es una couta menor a la 420
                    nextPayment.AmountTotal += valueNextQuotaAdd;
                    nextPayment.NumberOld = valueNextNumber - 1;
                    nextPayment.AmountOld = valueNextQuotaAdd;
                    nextPayment.Description =
                        "El valor de la couta a aumentado debido a un prorrateo de una couta anterior N° " +
                        (valueNextNumber - 1);

                    //@todo no entiendo hay q revisar
                    if (nextPayment.AmountTotal > maximumCharge)
                    {
                        var temp = nextPayment.AmountTotal - maximumCharge;
                        nextPayment.AmountTotal = maximumCharge;
                        valueNextQuotaAdd = temp;
                        valueNextNumber++;
                    }
                    else
                    {
                        isNextQuota = false;
                        model.AmountPayed = 0;
                    }

                    _contributionService.UpdateContributionPayment(nextPayment);

                }
            }

            #endregion

            #region Es el caso en que sale del bucle y defrente hay couta negativa

            if (model.AmountPayed > 0)
            {
                #region ReturnPayment
                //couta negativa
                var contributionPayment = new ContributionPayment
                {
                    IsAutomatic = true,
                    StateId = (int)ContributionState.Devolucion,
                    AmountOld = 0,
                    ContributionId = model.ContributionId,
                    Number = maxNumber + 1,
                    AmountTotal = 0,
                    AmountPayed = model.AmountPayed * -1,
                    ScheduledDateOnUtc = DateTime.UtcNow,
                    ProcessedDateOnUtc = DateTime.UtcNow,
                    Description =
                        "Devolucion debido al pago personal realizado al numero de couta N°: " + (valueNextNumber - 1),
                    BankName = "ACMR",
                    AccountNumber = "ACMR",
                    TransactionNumber = "ACMR",
                    Reference = "Reembolso de aportacion cancelado con anticipación"
                };

                _contributionService.InsertContributionPayment(contributionPayment);

                //todo se debe cerrar el contribution y crear el beneficio normal 

                var returnPayment = new ReturnPayment
                {
                    AmountToPay = model.AmountPayed,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    PaymentNumber = maxNumber + 1,
                    StateId = (int)ReturnPaymentState.Creado,
                    ReturnPaymentTypeId = (int)ReturnPaymentType.Aportacion,
                    CustomerId = customer.Id
                };
                _returnPaymentService.InsertReturnPayment(returnPayment);
                #endregion

                var benefit = _benefitService.GetAllBenefits().Where(x => x.CloseContributions).FirstOrDefault();
                var contributionBenefit = contribution.CreateBenefit(benefit, _benefitValueSetting.AmountBaseOfBenefit,
                    _sequenceIdsSettings.NumberOfLiquidation, _contributionSettings.TotalCycle / 12);
                _benefitService.InsertContributionBenefit(contributionBenefit);

                #region Flow - Return - Benefit

                _workFlowService.InsertWorkFlow(new WorkFlow
                {
                    CustomerCreatedId = _workContext.CurrentCustomer.Id,
                    EntityId = customer.Id,
                    EntityName = CommonHelper.GetKsCustomTypeConverter(typeof(ReturnPayment)).ConvertToInvariantString(new Benefit()),
                    RequireCustomer = false,
                    RequireSystemRole = true,
                    SystemRoleApproval = SystemCustomerRoleNames.Employee,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    Active = true,
                    Title = "Nueva Beneficio Asignado",
                    Description = "Se ha asigado de manera automatica el beneficio " + benefit.Name +
                        ", para el asociado " + customer.GetFullName() + ", con DNI: " +
                        customer.GetAttribute<string>(SystemCustomerAttributeNames.Dni) +
                        " y con CIP: " + customer.GetAttribute<string>(SystemCustomerAttributeNames.AdmCode),
                    GoTo = "Admin/Customer/Edit/" + customer.Id
                });

                _workFlowService.InsertWorkFlow(new WorkFlow
                {
                    CustomerCreatedId = _workContext.CurrentCustomer.Id,
                    EntityId = returnPayment.Id,
                    EntityName = CommonHelper.GetKsCustomTypeConverter(typeof(ReturnPayment)).ConvertToInvariantString(new ReturnPayment()),
                    RequireCustomer = false,
                    RequireSystemRole = true,
                    SystemRoleApproval = SystemCustomerRoleNames.Employee,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    Active = true,
                    Title = "Nueva Devolución",
                    Description = "Se requiere revision para una devolucion realizada por el pago de la aportación con N° de autorización de descuento " +
                        contribution.AuthorizeDiscount + ", para el asociado " + customer.GetFullName() + ", con DNI: " +
                        customer.GetAttribute<string>(SystemCustomerAttributeNames.Dni) +
                        " y con CIP: " + customer.GetAttribute<string>(SystemCustomerAttributeNames.AdmCode),
                    GoTo = "Admin/ReturnPayment/Edit/" + returnPayment.Id
                });
                #endregion
            }

            #endregion

            #endregion

            SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Contribution.Updated"));

            if (continueEditing)
            {
                return RedirectToAction("CreateCustomPayment", new { id = model.ContributionId });
            }
            return RedirectToAction("Edit", new { id = model.ContributionId });

        }

        public ActionResult MovePayments(int id)
        {
            var contribution = _contributionService.GetContributionById(id);
            var customer = _customerService.GetCustomerById(contribution.CustomerId);
            var military = customer.GetAttribute<int>(SystemCustomerAttributeNames.MilitarySituationId);
            var dateOfAdmission = customer.GetAttribute<DateTime>(SystemCustomerAttributeNames.DateOfAdmission);
            var contributionPayment = _contributionService.GetPaymentByContributionId(id);
            var firstPaymentDate = contributionPayment.Min(x => x.ScheduledDateOnUtc);
            var lastPaymentDate = contributionPayment.Max(x => x.ScheduledDateOnUtc);
            var newPaymentDate = new DateTime(dateOfAdmission.Year, dateOfAdmission.Month, _contributionSettings.DayOfPaymentContribution);

            var model = new ContributionPaymentListRefactorModel
            {
                ContributionId = id,
                CustomerId = contribution.CustomerId,
                CustomerName = customer.GetFullName(),
                CustomerAdminCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.AdmCode),
                CustomerDni = customer.GetAttribute<string>(SystemCustomerAttributeNames.Dni),
                CustomerFrom = contribution.CreatedOnUtc,
                FirstPaymentDate = firstPaymentDate,
                LastPaymentDate = lastPaymentDate,
                NewPaymentDate = newPaymentDate,
                AuthorizeDiscount = contribution.AuthorizeDiscount,
                MilitarySituationId = military == 1 ? "COPERE" : military == 2 ? "CPMP" : "OTROS",
            };

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult MovePayments(ContributionPaymentListRefactorModel model, bool continueEditing)
        {
            var contribution = _contributionService.GetContributionById(model.ContributionId);
            var customer = _customerService.GetCustomerById(contribution.CustomerId);
            var contributionPayment = _contributionService.GetPaymentByContributionId(model.ContributionId);
            var firstPaymentDate = contributionPayment.Min(x => x.ScheduledDateOnUtc);
            var monthsDifference = GetAbsoluteMonthsDifference(firstPaymentDate, model.NewPaymentDate.Value);
            foreach (var payment in contributionPayment)
            {
                payment.ScheduledDateOnUtc = payment.ScheduledDateOnUtc.AddMonths(monthsDifference);
                _contributionService.UpdateContributionPayment(payment);
            }
            contributionPayment = _contributionService.GetPaymentByContributionId(model.ContributionId);

            SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Contribution.Updated"));
            
            if (continueEditing)
            {
                return RedirectToAction("MovePayments", new { id = model.ContributionId });
            }

            return RedirectToAction("Edit", new { id = model.ContributionId });
        }

        public ActionResult DownloadTemplate(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributions))
                return AccessDeniedView();

            var contributionPayment = _contributionService.GetPaymentByContributionId(id);

            var sb = new System.Text.StringBuilder();

            // Cabecera
            sb.AppendLine("Id|Numero|NumeroAnterior|ContribucionId|Monto1|Monto2|Monto3|MontoAnterior|MontoTotal|MontoPagado|Estado|EsAutomatico|Banco|NumeroCuenta|NumeroTransaccion|Referencia|Descripcion|FechaProgramada|FechaProcesada");

            // Datos
            foreach (var payment in contributionPayment)
            {
                sb.AppendLine(string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}|{17}|{18}",
                    payment.Id,
                    payment.Number,
                    payment.NumberOld,
                    payment.ContributionId,
                    payment.Amount1,
                    payment.Amount2,
                    payment.Amount3,
                    payment.AmountOld,
                    payment.AmountTotal,
                    payment.AmountPayed,
                    payment.ContributionState,
                    payment.IsAutomatic,
                    payment.BankName,
                    payment.AccountNumber,
                    payment.TransactionNumber,
                    payment.Reference,
                    payment.Description,
                    payment.ScheduledDateOnUtc.ToString("dd/MM/yyyy"),
                    payment.ProcessedDateOnUtc.HasValue ? payment.ProcessedDateOnUtc.Value.ToString("dd/MM/yyyy") : string.Empty
                ));
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", id + ".csv");
        }

        [HttpPost]
        public ActionResult UploadTemplate(int id, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributions))
                return AccessDeniedView();

            this.Server.ScriptTimeout = 300;

            var file = Request.Files["importxmlfile"];
            if (file == null || file.ContentLength == 0)
            {
                ErrorNotification("No se seleccionó ningún archivo.");
                return RedirectToAction("Edit", new { id = id });
            }

            List<string> validationErrors;
            List<ContributionPayment> paymentsToUpdate;

            // Leer el stream en memoria para poder usarlo dos veces
            var fileBytes = new byte[file.ContentLength];
            file.InputStream.Read(fileBytes, 0, file.ContentLength);

            // 1) Validar primero sin persistir
            using (var ms = new MemoryStream(fileBytes))
            {
                paymentsToUpdate = ValidateCsvPayments(id, ms, out validationErrors);
            }

            if (validationErrors.Any())
            {
                foreach (var error in validationErrors)
                    ErrorNotification(error);

                return RedirectToAction("Edit", new { id = id });
            }

            // 2) Sin errores: persistir
            foreach (var contributionPayment in paymentsToUpdate)
                _contributionService.UpdateContributionPayment(contributionPayment);

            SuccessNotification(string.Format("Se actualizaron {0} pagos correctamente.", paymentsToUpdate.Count));
            return RedirectToAction("Edit", new { id = id });
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual ContributionPaymentsModel PrepareContributionPayment(ContributionPayment contributionPayment)
        {
            var model = contributionPayment.ToModel();
            model.Banks = _bankSettings.PrepareBanks();
            model.ScheduledDateOn = _dateTimeHelper.ConvertToUserTime(contributionPayment.ScheduledDateOnUtc, DateTimeKind.Utc);
            if (contributionPayment.ProcessedDateOnUtc.HasValue)
                model.ProcessedDateOn = _dateTimeHelper.ConvertToUserTime(contributionPayment.ProcessedDateOnUtc.Value, DateTimeKind.Utc);
            var state = ContributionState.Pendiente.ToSelectList()
                .FirstOrDefault(x => x.Value == contributionPayment.StateId.ToString());
            if (state != null)
                model.State = state.Text;

            return model;
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

        [NonAction]
        protected virtual int GetAbsoluteMonthsDifference(DateTime date1, DateTime date2)
        {
            int meses = Math.Abs(((date2.Year - date1.Year) * 12) + date2.Month - date1.Month);
            return meses;
        }

        [NonAction]
        protected virtual List<ContributionPayment> ValidateCsvPayments(int id, Stream stream, out List<string> errors)
        {
            errors = new List<string>();
            var result = new List<ContributionPayment>();

            var stateMap = new Dictionary<string, int>
            {
                { "EnProceso",    (int)ContributionState.EnProceso    },
                { "Pendiente",     (int)ContributionState.Pendiente    },
                { "PagoParcial",  (int)ContributionState.PagoParcial  },
                { "Pagado",        (int)ContributionState.Pagado       },
                { "SinLiquidez",  (int)ContributionState.SinLiquidez  },
                { "Devolucion",    (int)ContributionState.Devolucion   },
                { "PagoPersonal", (int)ContributionState.PagoPersonal }
            };

            using (var sr = new StreamReader(stream, Encoding.UTF8))
            {
                // Saltar cabecera
                // Saltar cabecera y detectar separador
                var header = sr.ReadLine();
                var separator = (header != null && header.Contains('|')) ? '|' : ';';

                var lineNumber = 1;
                var dateFormats = new[] { "yyyy-MM-dd", "dd/MM/yyyy" };

                while (!sr.EndOfStream)
                {
                    lineNumber++;
                    var line = sr.ReadLine();

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var cols = line.Split(separator);

                    // Validar número de columnas
                    if (cols.Length != 19)
                    {
                        errors.Add(string.Format("Línea {0}: se esperaban 19 columnas, se encontraron {1}.", lineNumber, cols.Length));
                        break;
                    }

                    // Validar Id
                    int paymentId;
                    if (!int.TryParse(cols[0].Trim(), out paymentId))
                    {
                        errors.Add(string.Format("Línea {0}: el Id '{1}' no es válido.", lineNumber, cols[0].Trim()));
                        continue;
                    }

                    // Validar que el pago exista y pertenezca a la contribución
                    var contributionPayment = _contributionService.GetPaymentById(paymentId);
                    if (contributionPayment == null)
                    {
                        errors.Add(string.Format("Línea {0}: no se encontró el pago con Id {1}.", lineNumber, paymentId));
                        continue;
                    }
                    if (contributionPayment.ContributionId != id)
                    {
                        errors.Add(string.Format("Línea {0}: el pago Id {1} no pertenece a la contribución {2}.", lineNumber, paymentId, id));
                        continue;
                    }

                    // Validar Estado
                    int stateId;
                    if (!stateMap.TryGetValue(cols[10].Trim(), out stateId))
                    {
                        errors.Add(string.Format("Línea {0}: el estado '{1}' no es reconocido.", lineNumber, cols[10].Trim()));
                        continue;
                    }

                    // Validar decimales
                    decimal amount1, amount2, amount3, amountOld, amountTotal, amountPayed;
                    if (!decimal.TryParse(cols[4].Trim(), out amount1)) { errors.Add(string.Format("Línea {0}: Monto1 '{1}' no es válido.", lineNumber, cols[4].Trim())); continue; }
                    if (!decimal.TryParse(cols[5].Trim(), out amount2)) { errors.Add(string.Format("Línea {0}: Monto2 '{1}' no es válido.", lineNumber, cols[5].Trim())); continue; }
                    if (!decimal.TryParse(cols[6].Trim(), out amount3)) { errors.Add(string.Format("Línea {0}: Monto3 '{1}' no es válido.", lineNumber, cols[6].Trim())); continue; }
                    if (!decimal.TryParse(cols[7].Trim(), out amountOld)) { errors.Add(string.Format("Línea {0}: MontoAnterior '{1}' no válido.", lineNumber, cols[7].Trim())); continue; }
                    if (!decimal.TryParse(cols[8].Trim(), out amountTotal)) { errors.Add(string.Format("Línea {0}: MontoTotal '{1}' no válido.", lineNumber, cols[8].Trim())); continue; }
                    if (!decimal.TryParse(cols[9].Trim(), out amountPayed)) { errors.Add(string.Format("Línea {0}: MontoPagado '{1}' no válido.", lineNumber, cols[9].Trim())); continue; }

                    // Validar booleano
                    bool isAutomatic;
                    if (!bool.TryParse(cols[11].Trim(), out isAutomatic))
                    {
                        errors.Add(string.Format("Línea {0}: EsAutomatico '{1}' no es válido (use True o False).", lineNumber, cols[11].Trim()));
                        continue;
                    }

                    // Validar FechaProgramada
                    DateTime scheduledDate;
                    if (!TryParseDate(cols[17].Trim(), dateFormats, out scheduledDate))
                    {
                        errors.Add(string.Format("Línea {0}: FechaProgramada '{1}' no tiene el formato dd/MM/yyyy.", lineNumber, cols[17].Trim()));
                        continue;
                    }

                    // Validar FechaProcesada (opcional)
                    DateTime? processedDate = null;
                    if (!string.IsNullOrWhiteSpace(cols[18].Trim()))
                    {
                        DateTime tempDate;
                        if (!TryParseDate(cols[18].Trim(), dateFormats, out tempDate)) 
                        {
                            errors.Add(string.Format("Línea {0}: FechaProcesada '{1}' no tiene el formato dd/MM/yyyy.", lineNumber, cols[18].Trim()));
                            continue;
                        }
                        processedDate = tempDate;
                    }

                    // Fila válida: mapear entidad
                    contributionPayment.StateId = stateId;
                    contributionPayment.Amount1 = amount1;
                    contributionPayment.Amount2 = amount2;
                    contributionPayment.Amount3 = amount3;
                    contributionPayment.AmountOld = amountOld;
                    contributionPayment.AmountTotal = amountTotal;
                    contributionPayment.AmountPayed = amountPayed;
                    contributionPayment.IsAutomatic = isAutomatic;
                    contributionPayment.BankName = cols[12].Trim();
                    contributionPayment.AccountNumber = cols[13].Trim();
                    contributionPayment.TransactionNumber = cols[14].Trim();
                    contributionPayment.Reference = cols[15].Trim();
                    contributionPayment.Description = cols[16].Trim();
                    contributionPayment.ScheduledDateOnUtc = scheduledDate;
                    contributionPayment.ProcessedDateOnUtc = processedDate;

                    result.Add(contributionPayment);
                }
            }

            return result;
        }

        [NonAction]
        protected virtual bool TryParseDate(string value, string[] formats, out DateTime result)
        {
            return DateTime.TryParseExact(
                value,
                formats,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out result);
        }

        #endregion
    }
}