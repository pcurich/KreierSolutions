using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web.Mvc;
using Ks.Admin.Extensions;
using Ks.Admin.Models.Common;
using Ks.Admin.Models.Contract;
using Ks.Admin.Models.Customers;
using Ks.Admin.Models.Settings;
using Ks.Core;
using Ks.Core.Domain.Catalog;
using Ks.Core.Domain.Common;
using Ks.Core.Domain.Contract;
using Ks.Core.Domain.Customers;
using Ks.Core.Domain.Directory;
using Ks.Core.Domain.Messages;
using Ks.Services.Common;
using Ks.Services.Customers;
using Ks.Services.Directory;
using Ks.Services.ExportImport;
using Ks.Services.Helpers;
using Ks.Services.KsSystems;
using Ks.Services.Localization;
using Ks.Services.Logging;
using Ks.Services.Messages;
using Ks.Services.Security;
using Ks.Web.Framework.Controllers;
using Ks.Web.Framework.Kendoui;
using Ks.Web.Framework.Mvc;
using Ks.Services.Configuration;
using Ks.Services.Contract;
using Ks.Web.Framework;

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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
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


            var benefits = _benefitService.GetAllBenefits();
            var benefitInCustomer = _benefitService.GetContributionBenefitsByCustomer(customerId);
            foreach (var benefit in benefits)
            {
                if (benefitInCustomer.Count(x => x.BenefitId == benefit.Id) == 0)
                    model.BenefitModels.Add(new SelectListItem { Value = benefit.Id.ToString(), Text = benefit.Name });
            }
            model.BenefitModels.Insert(0, new SelectListItem { Value = "0", Text = "----------------------------" });

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
                int year = 0;

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
                _customerActivityService.InsertActivity("AddNewContributionBenefit", _localizationService.GetResource("ActivityLog.AddNewContributionBenefit"), entity.Id, model.CustomerCompleteName, _workContext.CurrentCustomer.GetFullName());

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.ContributionBenefit.Added"));

                return continueEditing ? RedirectToAction("Edit", new { id = entity.Id }) : RedirectToAction("Customer/Edit/" + model.CustomerId);
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
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(benefits.CreatedOnUtc, DateTimeKind.Utc);


            if (model.Id > 0)
                SaveSelectedTabIndex(1);

            return View(model);
        }

        [HttpPost]
        public ActionResult BankList(DataSourceRequest command, CustomerListModel model)
        {
            return View();
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

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ChangeTab(int customerId, int benefitId)
        {
            if (benefitId == 0)
                return Json(0.ToString("c", new CultureInfo("es-PE")), JsonRequestBehavior.AllowGet);

            var benefit = _benefitService.GetBenefitById(benefitId);
            var contribution = _contributionService.GetContributionsByCustomer(customerId, 1).FirstOrDefault();
            var zeroTime = new DateTime(1, 1, 1);
            int year = 0;
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