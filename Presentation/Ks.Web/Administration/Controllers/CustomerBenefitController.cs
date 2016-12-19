﻿using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly LoanSettings _loanSettings;
        private readonly BenefitValueSetting _benefitValueSetting;
        private readonly BankSettings _bankSettings;

        #endregion

        #region Constructor

        public CustomerBenefitController(ISettingService settingService, ICustomerService customerService, IEncryptionService encryptionService, IGenericAttributeService genericAttributeService, ICustomerRegistrationService customerRegistrationService, IDateTimeHelper dateTimeHelper, ILocalizationService localizationService, DateTimeSettings dateTimeSettings, ICountryService countryService, IStateProvinceService stateProvinceService, ICityService cityService, IAddressService addressService, CustomerSettings customerSettings, IWorkContext workContext, IKsSystemContext ksSystemContext, IExportManager exportManager, ICustomerActivityService customerActivityService, IContributionService contributionService, ILoanService loanService, IBenefitService benefitService, ITabService tabService, IPermissionService permissionService, IQueuedEmailService queuedEmailService, EmailAccountSettings emailAccountSettings, IEmailAccountService emailAccountService, AddressSettings addressSettings, IKsSystemService ksSystemService, ICustomerAttributeParser customerAttributeParser, ICustomerAttributeService customerAttributeService, IAddressAttributeParser addressAttributeParser, IAddressAttributeService addressAttributeService, IAddressAttributeFormatter addressAttributeFormatter, ContributionSettings contributionSettings, SequenceIdsSettings sequenceIdsSettings, LoanSettings loanSettings, BenefitValueSetting benefitValueSetting, BankSettings bankSettings)
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
            _loanSettings = loanSettings;
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

            var contribution = _contributionService.GetContributionsByCustomer(model.CustomerId, 1).FirstOrDefault();
            var year = 0;
            if (contribution != null)
                year = (new DateTime(1, 1, 1) + (DateTime.UtcNow - contribution.CreatedOnUtc)).Year;
            if (year > _contributionSettings.TotalCycle / 12)
                year = 35;

            model.ContributionStart = _dateTimeHelper.ConvertToUserTime(contribution.CreatedOnUtc, DateTimeKind.Utc);
            model.YearInActivity = year;
            model.TabValue = _tabService.GetValueFromActive(year).TabValue;
            model.TotalContributionCaja = amountCaja;
            model.TotalContributionCopere = amountCopere;
            model.TotalContributionPersonalPayment = amountTotal - amountCaja - amountCopere;

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
                if (year > _contributionSettings.TotalCycle / 12)
                    year = 35;

                var activeTab = _tabService.GetValueFromActive(year);
                var loans = _loanService.GetLoansByCustomer(model.CustomerId).Where(x => x.Active).ToList();

                model.TotalLoan = loans.Count;
                foreach (var loan in loans)
                {
                    model.TotalLoanToPay += loan.TotalAmount - loan.TotalPayed;
                }

                model.Discount = benefit.Discount;
                model.TabValue = activeTab != null ? activeTab.TabValue : 0;
                model.YearInActivity = year;
                model.TotalReationShip = 0;

                model.SubTotalToPay = ((decimal)model.Discount*model.AmountBaseOfBenefit*(decimal)model.TabValue)- model.TotalLoanToPay;
                model.ReserveFund = model.SubTotalToPay - model.TotalContributionCaja - model.TotalContributionCopere -
                                    model.TotalContributionPersonalPayment;

                model.TotalToPay = model.SubTotalToPay - model.TotalLoanToPay;
                var entity = model.ToEntity();
                entity.CreatedOnUtc = DateTime.UtcNow;
                entity.NumberOfLiquidation = _sequenceIdsSettings.NumberOfLiquidation;

                _benefitService.InsertContributionBenefit(entity);

                var storeScope = GetActiveStoreScopeConfiguration(_ksSystemService, _workContext);
                var sequenceIdsSettings = _settingService.LoadSetting<SequenceIdsSettings>(storeScope);

                sequenceIdsSettings.NumberOfLiquidation += 1;
                _settingService.SaveSetting(sequenceIdsSettings);

                //now clear settings cache
                _settingService.ClearCache();

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

            var contributionBenefit = _benefitService.GetContributionBenefitbyId(id);
            if (contributionBenefit == null)
                //No tab found with the specified id
                return RedirectToAction("List");

            var benefit = _benefitService.GetBenefitById(contributionBenefit.BenefitId);

            var model = contributionBenefit.ToModel();
            var contribution = _contributionService.GetContributionById(contributionBenefit.ContributionId);
            var customer = _customerService.GetCustomerById(contribution.Id);
            model.ContributionStart = _dateTimeHelper.ConvertToUserTime(contribution.CreatedOnUtc, DateTimeKind.Utc);
            model.BenefitModels = PrepareBenefitList(contribution.CustomerId, model.BenefitId, true);
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(contributionBenefit.CreatedOnUtc, DateTimeKind.Utc);
            model.CustomerId = customer.Id;
            model.CustomerDni = customer.GetAttribute<string>(SystemCustomerAttributeNames.Dni);
            model.CustomerAdmCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.AdmCode);
            model.CustomerCompleteName = customer.GetFullName();
            model.Banks = PrepareBanks();
            model.RelaTionShips = PrepareRelationShip();
            model.CustomField1 = benefit.CustomField1;
            model.CustomField2 = benefit.CustomField2;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(ContributionBenefitModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributionBenefit))
                return AccessDeniedView();

            var entity = _benefitService.GetContributionBenefitbyId(model.Id);
            entity.CustomValue1 = model.CustomValue1;
            entity.CustomValue2 = model.CustomValue2;
            entity.CustomField1 = model.CustomField1;
            entity.CustomField2 = model.CustomField2;

            _benefitService.UpdateContributionBenefit(entity);

            _customerActivityService.InsertActivity("EditContributionBenefit",
                    _localizationService.GetResource("ActivityLog.EditContributionBenefit"), model.BenefitName,
                    model.CustomerCompleteName, _workContext.CurrentCustomer.GetFullName());

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.ContributionBenefit.Updated"));

            return continueEditing
                    ? RedirectToAction("Edit", new { id = entity.Id })
                    : RedirectToAction("Edit", new { Controller = "Customer", id = model.CustomerId });

        }
        #endregion

        #region BenefitBank

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
                    model.Bank = x.BankName;
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
        public ActionResult BankCheckAdd([Bind(Exclude = "Id,CreatedOn")] ContributionBenefitBankModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributionBenefit))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            if (model.Dni == null || model.Dni.Length != 8)
            {
                return Json(new DataSourceResult { Errors = "El DNI no es valido" });
            }

            if (model.CheckNumber == null || model.CheckNumber.Length != 8)
            {
                return Json(new DataSourceResult { Errors = "El Número de cheque debe tener una longitud de 8 digitos" });
            }

            if (model.Ratio > 1)
            {
                return Json(new DataSourceResult { Errors = "Ingrese un valor entre 0 y 1" });
            }

            var entity = model.ToEntity();
            entity.BankName = GetBankById(Convert.ToInt32(model.BankId)).Text;
            entity.AccountNumber = GetBankById(Convert.ToInt32(model.BankId)).Value;

            entity.RelationShip = PrepareRelationShip().FirstOrDefault(x => x.Value == model.RelationShipId.ToString()).Text;
            entity.CreatedOnUtc = DateTime.UtcNow;
            entity.AmountToPay = model.TotalToPay * (decimal)model.Ratio;
            _benefitService.InsertContributionBenefitBank(entity);

            var contributionBenefit =
                _benefitService.GetContributionBenefitbyId(entity.ContributionBenefitId);

            contributionBenefit.TotalReationShip++;
            _benefitService.UpdateContributionBenefit(contributionBenefit);

            _customerActivityService.InsertActivity("AddNewContributionBenefitBank", _localizationService.GetResource("ActivityLog.AddNewContributionBenefitBank"), model.CompleteName, _workContext.CurrentCustomer.GetFullName());

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult BankCheckDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributionBenefit))
                return AccessDeniedView();

            var contributionBenefitBank = _benefitService.GetContributionBenefitBankById(id);
            if (contributionBenefitBank == null)
                throw new ArgumentException("No setting found with the specified id");

            var contributionBenefit =
                _benefitService.GetContributionBenefitbyId(contributionBenefitBank.ContributionBenefitId);

            contributionBenefit.TotalReationShip--;
            _benefitService.DeleteContributionBenefitBank(contributionBenefitBank);
            _benefitService.UpdateContributionBenefit(contributionBenefit);
            //activity log
            _customerActivityService.InsertActivity("DeleteContributionBenefitBank", _localizationService.GetResource("ActivityLog.DeleteContributionBenefitBank"), contributionBenefitBank.CompleteName, _workContext.CurrentCustomer.GetFullName());

            return new NullJsonResult();
        }

        #endregion

        #region Reports

        [HttpPost]
        [FormValueRequired("export-excel")]
        public ActionResult ExportExcel(FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageContributionBenefit))
                return AccessDeniedView();
            var customerId = Convert.ToInt32(form.GetValues("customerId")[0]);
            var customerBenefitId = Convert.ToInt32(form.GetValues("customerBenefitId")[0]);
            var customer = _customerService.GetCustomerById(customerId);
            var contributionBenefit = _benefitService.GetContributionBenefitbyId(customerBenefitId);
            var reportContributionBenefit = _benefitService.GetReportContributionBenefit(customerBenefitId);
            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    _exportManager.ExportReportContributionBenefitToXlsx(stream, customer, contributionBenefit, reportContributionBenefit);
                    bytes = stream.ToArray();
                }
                //Response.ContentType = "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //Response.AddHeader("content-disposition", "attachment; filename=Aportaciones.xlsx");
                return File(bytes, "aplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Beneficio - Auxilio.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        #endregion

        #region Util

        [NonAction]
        protected virtual List<SelectListItem> PrepareBanks()
        {
            var model = new List<SelectListItem>();

            if (_bankSettings.IsActive1)
                model.Add(new SelectListItem { Text = _bankSettings.NameBank1, Value = _bankSettings.IdBank1.ToString() });
            if (_bankSettings.IsActive2)
                model.Add(new SelectListItem { Text = _bankSettings.NameBank2, Value = _bankSettings.IdBank2.ToString() });
            if (_bankSettings.IsActive3)
                model.Add(new SelectListItem { Text = _bankSettings.NameBank3, Value = _bankSettings.IdBank3.ToString() });
            if (_bankSettings.IsActive4)
                model.Add(new SelectListItem { Text = _bankSettings.NameBank4, Value = _bankSettings.IdBank4.ToString() });
            if (_bankSettings.IsActive5)
                model.Add(new SelectListItem { Text = _bankSettings.NameBank5, Value = _bankSettings.IdBank5.ToString() });

            return model;
        }

        protected virtual SelectListItem GetBankById(int id)
        {
            if (_bankSettings.IsActive1 && _bankSettings.IdBank1 == id)
                return (new SelectListItem { Text = _bankSettings.NameBank1, Value = _bankSettings.AccountNumber1.ToString() });
            if (_bankSettings.IsActive2 && _bankSettings.IdBank2 == id)
                return (new SelectListItem { Text = _bankSettings.NameBank2, Value = _bankSettings.AccountNumber2.ToString() });
            if (_bankSettings.IsActive3 && _bankSettings.IdBank3 == id)
                return (new SelectListItem { Text = _bankSettings.NameBank3, Value = _bankSettings.AccountNumber3.ToString() });
            if (_bankSettings.IsActive4 && _bankSettings.IdBank4 == id)
                return (new SelectListItem { Text = _bankSettings.NameBank4, Value = _bankSettings.AccountNumber4.ToString() });
            if (_bankSettings.IsActive5 && _bankSettings.IdBank5 == id)
                return (new SelectListItem { Text = _bankSettings.NameBank5, Value = _bankSettings.AccountNumber5.ToString() });

            return new SelectListItem { Text = "", Value = "" };
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

        #endregion
    }
}