using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Ks.Core;
using Ks.Core.Caching;
using Ks.Core.Domain.Common;
using Ks.Core.Domain.Customers;
using Ks.Core.Domain.Localization;
using Ks.Services.Common;
using Ks.Services.Directory;
using Ks.Services.Localization;
using Ks.Services.Logging;
using Ks.Services.Security;
using Ks.Web.Framework.Controllers;
using Ks.Web.Infrastructure.Cache;
using Ks.Web.Models.Common;

namespace Ks.Web.Controllers
{
    public class CommonController : BasePublicController
    {
        #region Fields
         
        private readonly ILanguageService _languageService;
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IKsSystemContext _storeContext;
        private readonly ICacheManager _cacheManager;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWebHelper _webHelper;
        private readonly IPermissionService _permissionService;
    
        private readonly ICustomerActivityService _customerActivityService;
 

        private readonly CustomerSettings _customerSettings;
        private readonly CommonSettings _commonSettings;
        private readonly LocalizationSettings _localizationSettings;
  

        #endregion

        #region Ctor

        public CommonController(ILanguageService languageService, ICurrencyService currencyService, ILocalizationService localizationService, IWorkContext workContext, IKsSystemContext storeContext, ICacheManager cacheManager, IGenericAttributeService genericAttributeService, IWebHelper webHelper, IPermissionService permissionService, ICustomerActivityService customerActivityService, CustomerSettings customerSettings, CommonSettings commonSettings, LocalizationSettings localizationSettings)
        {
            _languageService = languageService;
            _currencyService = currencyService;
            _localizationService = localizationService;
            _workContext = workContext;
            _storeContext = storeContext;
            _cacheManager = cacheManager;
            _genericAttributeService = genericAttributeService;
            _webHelper = webHelper;
            _permissionService = permissionService;
            _customerActivityService = customerActivityService;
            _customerSettings = customerSettings;
            _commonSettings = commonSettings;
            _localizationSettings = localizationSettings;
        }

        #endregion

        #region Methods

        //page not found
        public ActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;

            return View();
        }

        //favicon
        [ChildActionOnly]
        public ActionResult Favicon()
        {
            //try loading a store specific favicon
            var faviconFileName = string.Format("favicon-{0}.ico", _storeContext.CurrentSystem.Id);
            var localFaviconPath = System.IO.Path.Combine(Request.PhysicalApplicationPath, faviconFileName);
            if (!System.IO.File.Exists(localFaviconPath))
            {
                //try loading a generic favicon
                faviconFileName = "favicon.ico";
                localFaviconPath = System.IO.Path.Combine(Request.PhysicalApplicationPath, faviconFileName);
                if (!System.IO.File.Exists(localFaviconPath))
                {
                    return Content("");
                }
            }

            var model = new FaviconModel
            {
                FaviconUrl = _webHelper.GetStoreLocation() + faviconFileName
            };
            return PartialView(model);
        }

        //currency
        [ChildActionOnly]
        public ActionResult CurrencySelector()
        {
            var availableCurrencies = _cacheManager.Get(string.Format(ModelCacheEventConsumer.AVAILABLE_CURRENCIES_MODEL_KEY, _workContext.WorkingLanguage.Id, _storeContext.CurrentSystem.Id), () =>
            {
                var result = _currencyService
                    .GetAllCurrencies(ksSystemId: _storeContext.CurrentSystem.Id)
                    .Select(x =>
                    {
                        //currency char
                        var currencySymbol = "";
                        if (!string.IsNullOrEmpty(x.DisplayLocale))
                            currencySymbol = new RegionInfo(x.DisplayLocale).CurrencySymbol;
                        else
                            currencySymbol = x.CurrencyCode;
                        //model
                        var currencyModel = new CurrencyModel
                        {
                            Id = x.Id,
                            Name = x.GetLocalized(y => y.Name),
                            CurrencySymbol = currencySymbol
                        };
                        return currencyModel;
                    })
                    .ToList();
                return result;
            });

            var model = new CurrencySelectorModel
            {
                CurrentCurrencyId = _workContext.WorkingCurrency.Id,
                AvailableCurrencies = availableCurrencies
            };

            if (model.AvailableCurrencies.Count == 1)
                Content("");

            return PartialView(model);
        }

        //language
        [ChildActionOnly]
        public ActionResult LanguageSelector()
        {
            var availableLanguages = _cacheManager.Get(string.Format(ModelCacheEventConsumer.AVAILABLE_LANGUAGES_MODEL_KEY, _storeContext.CurrentSystem.Id), () =>
            {
                var result = _languageService
                    .GetAllLanguages(storeId: _storeContext.CurrentSystem.Id)
                    .Select(x => new LanguageModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        FlagImageFileName = x.FlagImageFileName,
                    })
                    .ToList();
                return result;
            });

            var model = new LanguageSelectorModel
            {
                CurrentLanguageId = _workContext.WorkingLanguage.Id,
                AvailableLanguages = availableLanguages,
                UseImages = _localizationSettings.UseImagesForLanguageSelection
            };

            if (model.AvailableLanguages.Count == 1)
                Content("");

            return PartialView(model);
        }

        //header links
        [ChildActionOnly]
        public ActionResult HeaderLinks()
        {
            var customer = _workContext.CurrentCustomer;
            var model = new HeaderLinksModel
            {
                IsAuthenticated = customer.IsRegistered(),
                CustomerEmailUsername = customer.IsRegistered() ? (_customerSettings.UsernamesEnabled ? customer.Username : customer.Email) : "",
                AlertMessage = "alertMessage",
            };

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult AdminHeaderLinks()
        {
            var model = new AdminHeaderLinksModel
            {
                DisplayAdminLink = _permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel),
            };

            return PartialView(model);
        }

        //footer
        [ChildActionOnly]
        public ActionResult JavaScriptDisabledWarning()
        {
            if (!_commonSettings.DisplayJavaScriptDisabledWarning)
                return Content("");

            return PartialView();
        }

        #endregion
    }
}