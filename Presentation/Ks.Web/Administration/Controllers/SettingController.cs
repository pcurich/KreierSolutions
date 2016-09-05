using System;
using System.Web.Mvc;
using Ks.Admin.Extensions;
using Ks.Admin.Models.Settings;
using Ks.Core;
using Ks.Core.Domain.Common;
using Ks.Core.Domain.Customers;
using Ks.Services.Common;
using Ks.Services.Configuration;
using Ks.Services.Customers;
using Ks.Services.Directory;
using Ks.Services.Helpers;
using Ks.Services.KsSystems;
using Ks.Services.Localization;
using Ks.Services.Logging;
using Ks.Services.Media;
using Ks.Services.Security;
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

        #endregion

        #region Constructors

        public SettingController(ISettingService settingService, ICountryService countryService, IStateProvinceService stateProvinceService, IAddressService addressService, ICurrencyService currencyService, IPictureService pictureService, ILocalizationService localizationService, IDateTimeHelper dateTimeHelper, IEncryptionService encryptionService, IThemeProvider themeProvider, ICustomerService customerService, ICustomerActivityService customerActivityService, IPermissionService permissionService, IFulltextService fulltextService, IMaintenanceService maintenanceService, IKsSystemService ksSystemService, IWorkContext workContext, IGenericAttributeService genericAttributeService, ILanguageService languageService, ILocalizedEntityService localizedEntityService)
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
        }

        #endregion

        #region Methods

        #region CustomerUser

        public ActionResult CustomerUser()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var storeScope = this.GetActiveStoreScopeConfiguration(_ksSystemService, _workContext);
            var customerSettings = _settingService.LoadSetting<CustomerSettings>(storeScope);
            var addressSettings = _settingService.LoadSetting<AddressSettings>(storeScope);
            var dateTimeSettings = _settingService.LoadSetting<DateTimeSettings>(storeScope);
            var externalAuthenticationSettings = _settingService.LoadSetting<ExternalAuthenticationSettings>(storeScope);

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

            return  View(model);
        }
        [HttpPost]
        public ActionResult CustomerUser(CustomerUserSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();


            var storeScope = this.GetActiveStoreScopeConfiguration(_ksSystemService, _workContext);
            var customerSettings = _settingService.LoadSetting<CustomerSettings>(storeScope);
            var addressSettings = _settingService.LoadSetting<AddressSettings>(storeScope);
            var dateTimeSettings = _settingService.LoadSetting<DateTimeSettings>(storeScope);
            var externalAuthenticationSettings = _settingService.LoadSetting<ExternalAuthenticationSettings>(storeScope);

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

        

        #endregion
    }
}