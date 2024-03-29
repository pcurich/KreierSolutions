﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ks.Admin.Extensions;
using Ks.Admin.Models.Settings;
using Ks.Core;
using Ks.Core.Domain.Batchs;
using Ks.Core.Domain.Common;
using Ks.Core.Domain.Contract;
using Ks.Core.Domain.Customers;
using Ks.Core.Domain.Directory;
using Ks.Services.Common;
using Ks.Services.Configuration;
using Ks.Services.Contract;
using Ks.Services.Customers;
using Ks.Services.Directory;
using Ks.Services.Helpers;
using Ks.Services.KsSystems;
using Ks.Services.Localization;
using Ks.Services.Logging;
using Ks.Services.Media;
using Ks.Services.Security;
using Ks.Web.Framework;
using Ks.Web.Framework.Controllers;
using Ks.Web.Framework.Kendoui;
using Ks.Web.Framework.Mvc;
using Ks.Web.Framework.Security;
using Ks.Web.Framework.Themes;

namespace Ks.Admin.Controllers
{
    public class SettingController : BaseAdminController
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IAddressService _addressService;
        private readonly ICurrencyService _currencyService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEncryptionService _encryptionService;
        private readonly IThemeProvider _themeProvider;
        private readonly ICustomerService _customerService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IFulltextService _fulltextService;
        private readonly IMaintenanceService _maintenanceService;
        private readonly IKsSystemService _ksSystemService;
        private readonly IWorkContext _workContext;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IContributionService _contributionService;

        #endregion

        #region Constructors

        public SettingController(ISettingService settingService, ICountryService countryService,
            IStateProvinceService stateProvinceService, IAddressService addressService, ICurrencyService currencyService,
            IPictureService pictureService, ILocalizationService localizationService, IDateTimeHelper dateTimeHelper,
            IEncryptionService encryptionService, IThemeProvider themeProvider, ICustomerService customerService,
            ICustomerActivityService customerActivityService, IPermissionService permissionService,
            IFulltextService fulltextService, IMaintenanceService maintenanceService, IKsSystemService ksSystemService,
            IWorkContext workContext, IGenericAttributeService genericAttributeService, ILanguageService languageService,
            ILocalizedEntityService localizedEntityService, IContributionService contributionService)
        {
            _settingService = settingService;
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
            _addressService = addressService;
            _currencyService = currencyService;
            _pictureService = pictureService;
            _localizationService = localizationService;
            _dateTimeHelper = dateTimeHelper;
            _encryptionService = encryptionService;
            _themeProvider = themeProvider;
            _customerService = customerService;
            _customerActivityService = customerActivityService;
            _permissionService = permissionService;
            _fulltextService = fulltextService;
            _maintenanceService = maintenanceService;
            _ksSystemService = ksSystemService;
            _workContext = workContext;
            _genericAttributeService = genericAttributeService;
            _languageService = languageService;
            _localizedEntityService = localizedEntityService;
            _contributionService = contributionService;
        }

        #endregion

        #region Methods

        #region CustomerUser

        public ActionResult CustomerUser()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var customerSettings = _settingService.LoadSetting<CustomerSettings>();
            var addressSettings = _settingService.LoadSetting<AddressSettings>();
            var dateTimeSettings = _settingService.LoadSetting<DateTimeSettings>();
            var externalAuthenticationSettings = _settingService.LoadSetting<ExternalAuthenticationSettings>();

            //merge settings
            var model = new CustomerUserSettingsModel
            {
                CustomerSettings = customerSettings.ToModel(),
                AddressSettings = addressSettings.ToModel(),
                DateTimeSettings =
                {
                    AllowCustomersToSetTimeZone = dateTimeSettings.AllowCustomersToSetTimeZone,
                    DefaultStoreTimeZoneId = _dateTimeHelper.DefaultStoreTimeZone.Id
                }
            };

            foreach (TimeZoneInfo timeZone in _dateTimeHelper.GetSystemTimeZones())
            {
                model.DateTimeSettings.AvailableTimeZones.Add(new SelectListItem
                    {
                        Text = timeZone.DisplayName,
                        Value = timeZone.Id,
                        Selected = timeZone.Id.Equals(_dateTimeHelper.DefaultStoreTimeZone.Id, StringComparison.InvariantCultureIgnoreCase)
                    });
            }

            model.ExternalAuthenticationSettings.AutoRegisterEnabled = externalAuthenticationSettings.AutoRegisterEnabled;

            return View(model);
        }
        [HttpPost]
        public ActionResult CustomerUser(CustomerUserSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var customerSettings = _settingService.LoadSetting<CustomerSettings>( );
            var addressSettings = _settingService.LoadSetting<AddressSettings>( );
            var dateTimeSettings = _settingService.LoadSetting<DateTimeSettings>( );
            var externalAuthenticationSettings = _settingService.LoadSetting<ExternalAuthenticationSettings>( );

            customerSettings = model.CustomerSettings.ToEntity(customerSettings);
            _settingService.SaveSetting(customerSettings);

            addressSettings = model.AddressSettings.ToEntity(addressSettings);
            _settingService.SaveSetting(addressSettings);

            dateTimeSettings.DefaultStoreTimeZoneId = model.DateTimeSettings.DefaultStoreTimeZoneId;
            dateTimeSettings.AllowCustomersToSetTimeZone = model.DateTimeSettings.AllowCustomersToSetTimeZone;
            _settingService.SaveSetting(dateTimeSettings);

            externalAuthenticationSettings.AutoRegisterEnabled = model.ExternalAuthenticationSettings.AutoRegisterEnabled;
            _settingService.SaveSetting(externalAuthenticationSettings);

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));

            //selected tab
            SaveSelectedTabIndex();

            return RedirectToAction("CustomerUser");
        }

        #endregion

        #region Bank

        public ActionResult Bank()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var bankSettings = _settingService.LoadSetting<BankSettings>( );

            var model = bankSettings.ToModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Bank(BankSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if(!ModelState.IsValid)
                return View(model);

            var bankSettings = _settingService.LoadSetting<BankSettings>( );

            bankSettings = model.ToEntity(bankSettings);
            
            bankSettings.IdBank1 = 1;
            bankSettings.IdBank2 = 2;
            bankSettings.IdBank3 = 3;
            bankSettings.IdBank4 = 4;
            bankSettings.IdBank5 = 5;

            _settingService.SaveSetting(bankSettings);

            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));

            //selected tab
            SaveSelectedTabIndex();

            return RedirectToAction("Bank");
        }

        #endregion

        #region SequenceIds

        public ActionResult SequenceIds()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var letterSettings = _settingService.LoadSetting<SequenceIdsSettings>( );

            var model = letterSettings.ToModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult SequenceIds(SequenceIdsSettingsModel model)
        {
            var letterSettings = _settingService.LoadSetting<SequenceIdsSettings>( );

            letterSettings = model.ToEntity(letterSettings);
            _settingService.SaveSetting(letterSettings);

            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));

            //selected tab
            SaveSelectedTabIndex();

            return RedirectToAction("SequenceIds");
        }

        #endregion

        #region Contributions

        public ActionResult Contributions()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var paymentSettings = _settingService.LoadSetting<ContributionSettings>( );

            var model = paymentSettings.ToModel(); 
            return View(model);
        }

        [HttpPost, ActionName("Contributions")]
        [FormValueRequired("save")]
        public ActionResult Contributions(ContributionSettingsModel model)
        {
            var paymentSettings = _settingService.LoadSetting<ContributionSettings>();

            #region clean
            if (model.NameAmount1 == null)
                model.NameAmount1 = "";
            if (model.NameAmount2 == null)
                model.NameAmount2 = "";
            if (model.NameAmount3 == null)
                model.NameAmount3 = "";
            if (model.NameAmount4 == null)
                model.NameAmount4 = "";
            if (model.NameAmount5 == null)
                model.NameAmount5 = "";
            if (model.NameAmount6 == null)
                model.NameAmount6 = "";

            if (!model.IsActiveAmount1)
            {
                model.Is1OnReport = false;
                model.NameAmount1 = "";
                model.Amount1 = 0;
            }

            if (!model.IsActiveAmount2)
            {
                model.Is2OnReport = false;
                model.NameAmount2 = "";
                model.Amount2 = 0;
            }

            if (!model.IsActiveAmount3)
            {
                model.Is3OnReport = false;
                model.NameAmount3 = "";
                model.Amount3 = 0;
            }

            if (!model.IsActiveAmount4)
            {
                model.Is4OnReport = false;
                model.NameAmount4 = "";
                model.Amount4 = 0;
            }

            if (!model.IsActiveAmount5)
            {
                model.Is5OnReport = false;
                model.NameAmount5 = "";
                model.Amount5 = 0;
            }
            if (!model.IsActiveAmount6)
            {
                model.Is6OnReport = false;
                model.NameAmount6 = "";
                model.Amount6 = 0;
            }
            #endregion

            if (ModelState.IsValid)
            {
                paymentSettings = model.ToEntity(paymentSettings);
                paymentSettings.Amount1Source = ((int)CustomerMilitarySituation.Actividad);
                paymentSettings.Amount2Source = ((int)CustomerMilitarySituation.Actividad);
                paymentSettings.Amount3Source = ((int)CustomerMilitarySituation.Actividad);
                paymentSettings.Amount4Source = ((int)CustomerMilitarySituation.Retiro);
                paymentSettings.Amount5Source = ((int)CustomerMilitarySituation.Retiro);
                paymentSettings.Amount6Source = ((int)CustomerMilitarySituation.Retiro);
                _settingService.SaveSetting(paymentSettings);

                //now clear settings cache
                _settingService.ClearCache();

                //activity log
                _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));

                //selected tab
                SaveSelectedTabIndex();

                return RedirectToAction("Contributions");
            }

            //model.Amount1Sources = new List<SelectListItem>
            //{
            //    new SelectListItem{Value = "0", Text = "Todos" },
            //    new SelectListItem{Value = "1", Text = "Copere" },
            //    new SelectListItem{Value = "2", Text = "Caja" },
            //};
            //model.Amount2Sources = new List<SelectListItem>
            //{
            //    new SelectListItem{Value = "0", Text = "Todos" },
            //    new SelectListItem{Value = "1", Text = "Copere" },
            //    new SelectListItem{Value = "2", Text = "Caja" },
            //};
            //model.Amount3Sources = new List<SelectListItem>
            //{
            //    new SelectListItem{Value = "0", Text = "Todos" },
            //    new SelectListItem{Value = "1", Text = "Copere" },
            //    new SelectListItem{Value = "2", Text = "Caja" },
            //};
            return View(model);

        }

        [HttpPost ] 
        public ActionResult ViewResultPopup(ContributionSettingsModel model)
        {
            var contributionsDelays = _contributionService.GetContributionGroupByDelay(); 

            var gridModel = new DataSourceResult
            {
                Data = contributionsDelays.Select(PrepareToChangeModelForList) ,
                Total = contributionsDelays.TotalCount
            };

            return Json(gridModel);
             
        }
        

        [HttpPost, ActionName("Contributions")]
        [FormValueRequired("ResetCopere")]
        public ActionResult ResetCopere(ContributionSettingsModel model)
        {
            //int[] customerId = _customerService.GetCustomersByGenericAttribute(SystemCustomerAttributeNames.MilitarySituationId, ((int)CustomerMilitarySituation.Actividad).ToString());
            var paymentSettings = _settingService.LoadSetting<ContributionSettings>();

            int size = _contributionService.UpdatePaymentAmount((int)CustomerMilitarySituation.Actividad, (int)ContributionState.Pendiente, paymentSettings.Amount1, paymentSettings.Amount2, paymentSettings.Amount3);
            SuccessNotification(string.Format("Se han actualizado {0} cuotas en estado pendiente con situacion militar en {1}", size, CustomerMilitarySituation.Actividad));
            return RedirectToAction("Contributions");
        }

        [HttpPost, ActionName("Contributions")]
        [FormValueRequired("ResetCaja")]
        public ActionResult ResetCaja(ContributionSettingsModel model)
        {
            //int[] customerId = _customerService.GetCustomersByGenericAttribute(SystemCustomerAttributeNames.MilitarySituationId, ((int)CustomerMilitarySituation.Retiro).ToString());
            var paymentSettings = _settingService.LoadSetting<ContributionSettings>();

            int size = _contributionService.UpdatePaymentAmount((int)CustomerMilitarySituation.Retiro, (int)ContributionState.Pendiente, paymentSettings.Amount4, paymentSettings.Amount5, paymentSettings.Amount6);
            SuccessNotification(string.Format("Se han actualizado {0} cuotas en estado pendiente con situacion militar en {1}",size, CustomerMilitarySituation.Retiro));
            return RedirectToAction("Contributions");
        }
        #endregion

        #region Loans

        public ActionResult Loans()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var stateActivitySettings = _settingService.LoadSetting<LoanSettings>( );

            var model = stateActivitySettings.ToModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Loans(LoanSettingsModel model)
        {
            var letterSettings = _settingService.LoadSetting<LoanSettings>( );

            model.CashFlow = letterSettings.CashFlow;
            letterSettings = model.ToEntity(letterSettings);
            _settingService.SaveSetting(letterSettings);

            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));

            //selected tab
            SaveSelectedTabIndex();

            return RedirectToAction("Loans");
        }

        [HttpPost]
        public ActionResult CashFlowSelect(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            
            var stateActivitySettings = _settingService.LoadSetting<LoanSettings>();
 
            var model = new List<CashFlowModel>();

            if (!string.IsNullOrEmpty(stateActivitySettings.CashFlow))
            {
                model = XmlHelper.XmlToObject<List<CashFlowModel>>(@stateActivitySettings.CashFlow);
                model = model.OrderBy(x => x.Amount).ToList();
            }

            var gridModel = new DataSourceResult
            {
                Data = model,
                Total = model.Count
            };

            return Json(gridModel);
        }

        public ActionResult CashFlowCreatePopup()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            return View(new CashFlowModel());
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CashFlowCreatePopup(string btnId, string formId, CashFlowModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            
            var stateActivitySettings = _settingService.LoadSetting<LoanSettings>();

            List<CashFlowModel> _model = new List<CashFlowModel>();

            if (!string.IsNullOrEmpty(stateActivitySettings.CashFlow))
                _model = XmlHelper.XmlToObject<List<CashFlowModel>>(@stateActivitySettings.CashFlow);
            if (_model.Count == 0)
                model.Id = 1;
            else
                model.Id = _model.Max(x => x.Id) + 1;
            
            _model.Add(model);

            stateActivitySettings.CashFlow = XmlHelper.Serialize2String(_model).Replace("\"", "'").Replace("\r", "").Replace("\n", "");
            _settingService.SaveSetting(stateActivitySettings);

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.StateActivitySettings.Added"));
            return View(model);
            
        }

        [HttpPost]
        public ActionResult CashFlowDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            
            var stateActivitySettings = _settingService.LoadSetting<LoanSettings>();

            List<CashFlowModel> model = null;
            var newModel = new List<CashFlowModel>();


            if (!string.IsNullOrEmpty(stateActivitySettings.CashFlow))
                model = XmlHelper.XmlToObject<List<CashFlowModel>>(@stateActivitySettings.CashFlow);

            if (model != null)
            {
                foreach (var m in model)
                {
                    if(m.Id!=id)
                        newModel.Add(m);
                }
                stateActivitySettings.CashFlow = XmlHelper.Serialize2String(newModel).Replace("\"", "'").Replace("\r", "").Replace("\n", "");
                _settingService.SaveSetting(stateActivitySettings);
            }
            return new NullJsonResult();
        }

        #endregion

        #region ScheduleBatch

        public ActionResult ScheduleBatch()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            
            var stateActivitySettings = _settingService.LoadSetting<ScheduleBatchsSetting>();

            var model = stateActivitySettings.ToModel();
            return  View(model);
        }

        [HttpPost]
        public ActionResult ScheduleBatch(ScheduleBatchSettingsModel model)
        {
            
            var batchsSetting = _settingService.LoadSetting<ScheduleBatchsSetting>();

            batchsSetting = model.ToEntity(batchsSetting);
            _settingService.SaveSetting(batchsSetting);

            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));

            //selected tab
            SaveSelectedTabIndex();

            return RedirectToAction("ScheduleBatch");
        }


        #endregion

        #region SignatureSetting

        public ActionResult Signature()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            
            var stateActivitySettings = _settingService.LoadSetting<SignatureSettings>();

            var model = stateActivitySettings.ToModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Signature(SignatureSettingsModel model)
        {
            
            var batchsSetting = _settingService.LoadSetting<SignatureSettings>();

            batchsSetting = model.ToEntity(batchsSetting);
            _settingService.SaveSetting(batchsSetting);

            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));

            //selected tab
            SaveSelectedTabIndex();

            return RedirectToAction("Signature");
        }

        #endregion

        #region Services of Pre

        #endregion  

        #region All Setting

        //all settings
        public ActionResult AllSettings()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            return View();
        }
        [HttpPost]
        //do not validate request token (XSRF)
        //for some reasons it does not work with "filtering" support
        [AdminAntiForgery(true)]
        public ActionResult AllSettings(DataSourceRequest command,
            Ks.Web.Framework.Kendoui.Filter filter = null, IEnumerable<Sort> sort = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var settings = _settingService
                .GetAllSettings()
                .Select(x =>
                {  
                    var settingModel = new SettingModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Value = x.Value
                    };
                    return settingModel;
                })
                .AsQueryable()
                .Filter(filter)
                .Sort(sort);

            var gridModel = new DataSourceResult
            {
                Data = settings.PagedForCommand(command).ToList(),
                Total = settings.Count()
            };

            return Json(gridModel);
        }
        [HttpPost]
        public ActionResult SettingUpdate(SettingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (model.Name != null)
                model.Name = model.Name.Trim();
            if (model.Value != null)
                model.Value = model.Value.Trim();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var setting = _settingService.GetSettingById(model.Id);
            if (setting == null)
                return Content("No setting could be loaded with the specified ID");

            if (!setting.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                //setting name or store has been changed
                _settingService.DeleteSetting(setting);
            }

            _settingService.SetSetting(model.Name, model.Value);

            //activity log
            _customerActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            return new NullJsonResult();
        }
        [HttpPost]
        public ActionResult SettingAdd([Bind(Exclude = "Id")] SettingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (model.Name != null)
                model.Name = model.Name.Trim();
            if (model.Value != null)
                model.Value = model.Value.Trim();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }
            _settingService.SetSetting(model.Name, model.Value);

            //activity log
            _customerActivityService.InsertActivity("AddNewSetting", _localizationService.GetResource("ActivityLog.AddNewSetting"), model.Name);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult SettingDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var setting = _settingService.GetSettingById(id);
            if (setting == null)
                throw new ArgumentException("No setting found with the specified id");
            _settingService.DeleteSetting(setting);

            //activity log
            _customerActivityService.InsertActivity("DeleteSetting", _localizationService.GetResource("ActivityLog.DeleteSetting"), setting.Name);

            return new NullJsonResult();
        }

        #endregion

        #region Helper

        [NonAction]
        protected virtual CustumerToChange PrepareToChangeModelForList(Contribution contribution)
        {
            return new CustumerToChange
            {
                Delay = contribution.DelayCycles,
                Size = Convert.ToInt32(contribution.AmountPayed)
            };
        }

        #endregion

        #endregion
    }
}