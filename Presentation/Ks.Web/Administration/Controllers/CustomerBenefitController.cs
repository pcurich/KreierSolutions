using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Ks.Admin.Extensions;
using Ks.Admin.Models.Contract;
using Ks.Admin.Models.Customers;
using Ks.Core;
using Ks.Core.Domain.Common;
using Ks.Core.Domain.Contract;
using Ks.Core.Domain.Customers;
using Ks.Core.Domain.Messages;
using Ks.Services.Common;
using Ks.Services.Configuration;
using Ks.Services.Contract;
using Ks.Services.Customers;
using Ks.Services.Directory;
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
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Controllers
{
    public class CustomerBenefitController : BaseAdminController
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly ICustomerService _customerService;
        private readonly IEncryptionService _encryptionService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICityService _cityService;
        private readonly IAddressService _addressService;
        private readonly CustomerSettings _customerSettings;
        private readonly IWorkContext _workContext;
        private readonly IKsSystemContext _ksSystemContext;
        private readonly IExportManager _exportManager;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IContributionService _contributionService;
        private readonly ILoanService _loanService;
        private readonly IBenefitService _benefitService;
        private readonly ITabService _tabService;
        private readonly IPermissionService _permissionService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEmailAccountService _emailAccountService;
        private readonly AddressSettings _addressSettings;
        private readonly IKsSystemService _ksSystemService;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly ContributionSettings _contributionSettings;
        private readonly SequenceIdsSettings _sequenceIdsSettings;
        private readonly StateActivitySettings _stateActivitySettings;
        private readonly BenefitValueSetting _benefitValueSetting;
        private readonly BankSettings _bankSettings;

        #endregion

        #region Constructor

        public CustomerBenefitController(ISettingService settingService, ICustomerService customerService, IEncryptionService encryptionService, IGenericAttributeService genericAttributeService, ICustomerRegistrationService customerRegistrationService, IDateTimeHelper dateTimeHelper, ILocalizationService localizationService, DateTimeSettings dateTimeSettings, ICountryService countryService, IStateProvinceService stateProvinceService, ICityService cityService, IAddressService addressService, CustomerSettings customerSettings, IWorkContext workContext, IKsSystemContext ksSystemContext, IExportManager exportManager, ICustomerActivityService customerActivityService, IContributionService contributionService, ILoanService loanService, IBenefitService benefitService, ITabService tabService, IPermissionService permissionService, IQueuedEmailService queuedEmailService, EmailAccountSettings emailAccountSettings, IEmailAccountService emailAccountService, AddressSettings addressSettings, IKsSystemService ksSystemService, ICustomerAttributeParser customerAttributeParser, ICustomerAttributeService customerAttributeService, IAddressAttributeParser addressAttributeParser, IAddressAttributeService addressAttributeService, IAddressAttributeFormatter addressAttributeFormatter, ContributionSettings contributionSettings, SequenceIdsSettings sequenceIdsSettings, StateActivitySettings stateActivitySettings, BenefitValueSetting benefitValueSetting, BankSettings bankSettings)
        {
            _settingService = settingService;
            _customerService = customerService;
            _encryptionService = encryptionService;
            _genericAttributeService = genericAttributeService;
            _customerRegistrationService = customerRegistrationService;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _dateTimeSettings = dateTimeSettings;
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
            _cityService = cityService;
            _addressService = addressService;
            _customerSettings = customerSettings;
            _workContext = workContext;
            _ksSystemContext = ksSystemContext;
            _exportManager = exportManager;
            _customerActivityService = customerActivityService;
            _contributionService = contributionService;
            _loanService = loanService;
            _benefitService = benefitService;
            _tabService = tabService;
            _permissionService = permissionService;
            _queuedEmailService = queuedEmailService;
            _emailAccountSettings = emailAccountSettings;
            _emailAccountService = emailAccountService;
            _addressSettings = addressSettings;
            _ksSystemService = ksSystemService;
            _customerAttributeParser = customerAttributeParser;
            _customerAttributeService = customerAttributeService;
            _addressAttributeParser = addressAttributeParser;
            _addressAttributeService = addressAttributeService;
            _addressAttributeFormatter = addressAttributeFormatter;
            _contributionSettings = contributionSettings;
            _sequenceIdsSettings = sequenceIdsSettings;
            _stateActivitySettings = stateActivitySettings;
            _benefitValueSetting = benefitValueSetting;
            _bankSettings = bankSettings;
        }

        #endregion

        #region Benefits

        [HttpPost]
        public ActionResult List(DataSourceRequest command, int customerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributionBenefit))
                return AccessDeniedView();

            var benefits = _benefitService.GetAllContributionBenefitByCustomer(customerId);
            var gridModel = new DataSourceResult
            {
                Data = benefits.Select(x =>
                {
                    var model = x.ToModel();
                    model.BenefitName = _benefitService.GetAllBenefits().FirstOrDefault(c => c.Id == model.BenefitId).Name;
                    model.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, TimeZoneInfo.Utc);
                    return model;
                }),
                Total = benefits.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        public ActionResult Create(int customerId, int contributionId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributionBenefit))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
                //No customer found with the specified id
                return RedirectToAction("List");

            var model = new ContributionBenefitModel
            {
                CustomerId = customer.Id,
                CustomerDni = customer.GetAttribute<string>(SystemCustomerAttributeNames.Dni),
                CustomerAdmCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.AdmCode),
                CustomerCompleteName = customer.GetFullName(),
                AmountBaseOfBenefit = _benefitValueSetting.AmountBaseOfBenefit,
                ContributionId = contributionId
            };

            var contributions = _contributionService.GetPaymentByContributionId(contributionId);
            var amountTotal = contributions.Sum(x => x.AmountPayed);
            var amountCaja = contributions.Where(x => x.BankName == "Caja").Sum(x => x.AmountPayed);
            var amountCopere = contributions.Where(x => x.BankName == "Copere").Sum(x => x.AmountPayed);

            model.TotalContributionCaja = amountCaja;
            model.TotalContributionCopere = amountCopere;
            model.TotalPersonalPayment = amountTotal - amountCaja - amountCopere;

            model.BenefitModels = PrepareBenefitList(customerId, model.BenefitId);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(ContributionBenefitModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributionBenefit))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var benefit = _benefitService.GetBenefitById(model.BenefitId);
                var contribution = _contributionService.GetContributionsByCustomer(model.CustomerId, 1).FirstOrDefault();
                var year = 0;

                if (contribution != null)
                    year = (new DateTime(1, 1, 1) + (DateTime.UtcNow - contribution.CreatedOnUtc)).Year;

                var activeTab = _tabService.GetValueFromActive(year);

                model.Discount = benefit.Discount;
                model.TabValue = activeTab != null ? activeTab.TabValue : 0;
                model.YearInActivity = year;
                model.TotalReationShip = 0;

                var entity = model.ToEntity();
                entity.CreatedOnUtc = DateTime.UtcNow;

                _benefitService.InsertContributionBenefit(entity);

                //activity log
                _customerActivityService.InsertActivity("AddNewContributionBenefit",
                    _localizationService.GetResource("ActivityLog.AddNewContributionBenefit"), entity.Id,
                    model.CustomerCompleteName, _workContext.CurrentCustomer.GetFullName());

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.ContributionBenefit.Added"));

                return continueEditing
                    ? RedirectToAction("Edit", new { id = entity.Id })
                    : RedirectToAction("Edit", new { Controller = "Customer", id = model.CustomerId });
            }
            ErrorNotification(_localizationService.GetResource("Admin.Configuration.ContributionBenefit.Error"));

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributionBenefit))
                return AccessDeniedView();

            var benefits = _benefitService.GetContributionBenefitbyId(id);
            if (benefits == null)
                //No tab found with the specified id
                return RedirectToAction("List");

            var model = benefits.ToModel();
            var contribution = _contributionService.GetContributionById(benefits.ContributionId);
            var customer = _customerService.GetCustomerById(contribution.Id);
            model.BenefitModels = PrepareBenefitList(contribution.CustomerId, model.BenefitId, true);
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(benefits.CreatedOnUtc, DateTimeKind.Utc);
            model.CustomerId = customer.Id;
            model.CustomerDni = customer.GetAttribute<string>(SystemCustomerAttributeNames.Dni);
            model.CustomerAdmCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.AdmCode);
            model.CustomerCompleteName = customer.GetFullName();
            model.Banks = PrepareBanks();
            model.RelaTionShips = PrepareRelationShip();
            return View(model);
        }

        [HttpPost]
        public ActionResult BankCheckList(DataSourceRequest command, int contributionBenefitId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributionBenefit))
                return AccessDeniedView();

            var benefitsBanks = _benefitService.GetAllContributionBenefitBank(contributionBenefitId);
            var gridModel = new DataSourceResult
            {
                Data = benefitsBanks.Select(x =>
                {
                    var model = x.ToModel();
                    model.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, TimeZoneInfo.Utc);
                    return model;
                }),
                Total = benefitsBanks.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }


        [HttpPost]
        public ActionResult BankCheckUpdate([Bind(Exclude = "CreatedOn")] ContributionBenefitBankModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributionBenefit))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }
            var entity =model.ToEntity();
            entity.CreatedOnUtc = DateTime.UtcNow;
            _benefitService.InsertContributionBenefitBank(entity);

            _customerActivityService.InsertActivity("EditContributionBenefitBank", _localizationService.GetResource("ActivityLog.EditContributionBenefitBank"), model.CompleteName, _workContext.CurrentCustomer.GetFullName());

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult TabDetailAdd([Bind(Exclude = "Id,CreatedOn")] ContributionBenefitBankModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributionBenefit))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var entity = model.ToEntity();
            entity.CreatedOnUtc = DateTime.UtcNow;
            //revisar si viene bien todo
            _benefitService.InsertContributionBenefitBank(entity);

            _customerActivityService.InsertActivity("AddNewContributionBenefitBank", _localizationService.GetResource("ActivityLog.AddNewContributionBenefitBank"), model.CompleteName, _workContext.CurrentCustomer.GetFullName());

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult TabDetailDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributionBenefit))
                return AccessDeniedView();

            var contributionBenefitBank = _benefitService.GetContributionBenefitBankById(id);
            if (contributionBenefitBank == null)
                throw new ArgumentException("No setting found with the specified id");
            _benefitService.DeleteContributionBenefitBank(contributionBenefitBank);

            //activity log
            _customerActivityService.InsertActivity("DeleteContributionBenefitBank", _localizationService.GetResource("ActivityLog.DeleteContributionBenefitBank"), contributionBenefitBank.CompleteName, _workContext.CurrentCustomer.GetFullName());

            return new NullJsonResult();
        }

        #endregion

        #region Util

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
        protected virtual List<SelectListItem> PrepareRelationShip()
        {
            var model = RelationShipType.Esposa.ToSelectList().ToList();
            model.Insert(0, new SelectListItem { Value = "0", Text = "----------------" });
            return model;
        }

        [NonAction]
        protected virtual List<SelectListItem> PrepareBenefitList(int customerId, int benefitId, bool isCreate = false)
        {
            var model = new List<SelectListItem>();
            var benefits = _benefitService.GetAllBenefits();
            var benefitInCustomer = _benefitService.GetAllContributionBenefitByCustomer(customerId);
            foreach (var benefit in benefits)
            {
                if (isCreate)
                {
                    model.Add(new SelectListItem
                    {
                        Value = benefit.Id.ToString(),
                        Text = benefit.Name,
                        Selected = benefit.Id == benefitId
                    });
                }
                else
                {
                    if (benefitInCustomer.Count(x => x.BenefitId == benefit.Id) == 0)
                        model.Add(new SelectListItem
                        {
                            Value = benefit.Id.ToString(),
                            Text = benefit.Name,
                            Selected = benefit.Id == benefitId
                        });
                }
            }
            model.Insert(0, new SelectListItem { Value = "0", Text = "----------------------------", Selected = false });

            return model;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ChangeTab(int customerId, int benefitId)
        {
            if (benefitId == 0)
                return Json(0.ToString("c", new CultureInfo("es-PE")), JsonRequestBehavior.AllowGet);

            var benefit = _benefitService.GetBenefitById(benefitId);
            var contribution = _contributionService.GetContributionsByCustomer(customerId, 1).FirstOrDefault();
            var zeroTime = new DateTime(1, 1, 1);
            var year = 0;
            if (contribution != null)
            {
                var span = DateTime.UtcNow - contribution.CreatedOnUtc;
                year = (zeroTime + span).Year;
            }
            var activeTab = _tabService.GetValueFromActive(year);
            var total = _benefitValueSetting.AmountBaseOfBenefit * (decimal)benefit.Discount * (decimal)activeTab.TabValue;
            if (benefit.CancelLoans)
                total -= _loanService.GetLoansByCustomer(customerId).Sum(x => x.TotalToPay);

            return Json(total.ToString("c", new CultureInfo("es-PE")), JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}