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
using Ks.Services.Security;
using Ks.Web.Framework;
using Ks.Web.Framework.Controllers;
using Ks.Web.Framework.Kendoui;

namespace Ks.Admin.Controllers
{
    public partial class ContributionsController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IContributionService _contributionService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IExportManager _exportManager;

        private readonly ContributionSettings _contributionSettings;
        private readonly BankSettings _bankSettings;

        #endregion

        #region Constructors

        public ContributionsController(
            IPermissionService permissionService,
            IContributionService contributionService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            IDateTimeHelper dateTimeHelper,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IExportManager exportManager,
            ContributionSettings contributionSettings,
            BankSettings bankSettings)
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

            var customer = _contributionService.GetContributionById(id);
            var model = new ContributionPaymentListModel
            {
                ContributionId = id,
                CustomerId = customer.Id,
                States = ContributionState.EnProceso.ToSelectList(false).ToList(),
                Banks = PrepareBanks(),
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

            if (!ModelState.IsValid)
                return View(model);

            contributionPayment.IsAutomatic = false;
            contributionPayment.StateId = (int)ContributionState.Pagado;
            contributionPayment.ContributionId = model.ContributionId;
            contributionPayment.BankName = GetBank(model.BankName);
            contributionPayment.AccountNumber = model.AccountNumber;
            contributionPayment.TransactionNumber = model.TransactionNumber;
            contributionPayment.Reference = model.Reference;
            contributionPayment.Description = model.Description;
            contributionPayment.ProcessedDateOnUtc = DateTime.UtcNow;
            contributionPayment.AmountPayed = model.Amount1 + model.Amount2 + model.Amount3;

            if (_contributionSettings.IsActiveAmount1)
                contributionPayment.Amount1 = model.Amount1;
            if (_contributionSettings.IsActiveAmount2)
                contributionPayment.Amount2 = model.Amount2;
            if (_contributionSettings.IsActiveAmount3)
                contributionPayment.Amount3 = model.Amount3;

            _contributionService.UpdateContributionPayment(contributionPayment);

            var contribution = _contributionService.GetContributionById(model.ContributionId);
            contribution.UpdatedOnUtc = DateTime.UtcNow;
            contribution.AmountPayed += contributionPayment.Amount1 + contributionPayment.Amount2 +
                                       contributionPayment.Amount3;

            contribution.DelayCycles = 0;
            contribution.IsDelay = false;
            _contributionService.UpdateContribution(contribution);

            SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Contributions.Updated"));

            if (continueEditing)
            {
                return RedirectToAction("CreatePayment", new { id = contributionPayment.Id });
            }
            return RedirectToAction("Edit", new { id = contributionPayment.ContributionId });
        }

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

        #endregion

        #region Utilities

        [NonAction]
        protected virtual List<SelectListItem> PrepareBanks()
        {
            var model = new List<SelectListItem>();
            model.Insert(0, new SelectListItem { Value = "0", Text = "----------------" });

            if (_bankSettings.IsActive1)
                model.Add(new SelectListItem { Value = _bankSettings.AccountNumber1, Text = _bankSettings.NameBank1 });
            if (_bankSettings.IsActive2)
                model.Add(new SelectListItem { Value = _bankSettings.AccountNumber2, Text = _bankSettings.NameBank2 });
            if (_bankSettings.IsActive3)
                model.Add(new SelectListItem { Value = _bankSettings.AccountNumber3, Text = _bankSettings.NameBank3 });
            if (_bankSettings.IsActive4)
                model.Add(new SelectListItem { Value = _bankSettings.AccountNumber4, Text = _bankSettings.NameBank4 });
            if (_bankSettings.IsActive5)
                model.Add(new SelectListItem { Value = _bankSettings.AccountNumber5, Text = _bankSettings.NameBank5 });

            return model;
        }

        [NonAction]
        protected virtual ContributionPaymentsModel PrepareContributionPayment(ContributionPayment contributionPayment)
        {
            var model = contributionPayment.ToModel();
            model.Banks = PrepareBanks();
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

        #endregion
    }
}