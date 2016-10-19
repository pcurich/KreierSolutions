using System;
using System.Linq;
using System.Web;
using Ks.Core;
using Ks.Core.Domain.Customers;
using Ks.Core.Domain.Directory;
using Ks.Core.Domain.Localization;
using Ks.Core.Fakes;
using Ks.Services.Authentication;
using Ks.Services.Common;
using Ks.Services.Customers;
using Ks.Services.Directory;
using Ks.Services.Helpers;
using Ks.Services.Localization;
using Ks.Web.Framework.Localization;

namespace Ks.Web.Framework
{
    /// <summary>
    ///     Work context for web application
    /// </summary>
    public class WebWorkContext : IWorkContext
    {
        #region Const

        private const string CUSTOMER_COOKIE_NAME = "Ks.customer";

        #endregion

        #region Ctor

        public WebWorkContext(HttpContextBase httpContext,
            ICustomerService customerService,
            IKsSystemContext ksSystemContext,
            IAuthenticationService authenticationService,
            ILanguageService languageService,
            ICurrencyService currencyService,
            IGenericAttributeService genericAttributeService,
            CurrencySettings currencySettings,
            LocalizationSettings localizationSettings,
            IUserAgentHelper userAgentHelper)
        {
            _httpContext = httpContext;
            _customerService = customerService;
            _ksSystemContext = ksSystemContext;
            _authenticationService = authenticationService;
            _languageService = languageService;
            _currencyService = currencyService;
            _genericAttributeService = genericAttributeService;
            _currencySettings = currencySettings;
            _localizationSettings = localizationSettings;
            _userAgentHelper = userAgentHelper;
        }

        #endregion

        #region Fields

        private readonly HttpContextBase _httpContext;
        private readonly ICustomerService _customerService;
        private readonly IKsSystemContext _ksSystemContext;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILanguageService _languageService;
        private readonly ICurrencyService _currencyService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly CurrencySettings _currencySettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IUserAgentHelper _userAgentHelper;

        private Customer _cachedCustomer;
        private Language _cachedLanguage;
        private Currency _cachedCurrency;

        #endregion

        #region Utilities

        protected virtual HttpCookie GetCustomerCookie()
        {
            if (_httpContext == null || _httpContext.Request == null)
                return null;

            return _httpContext.Request.Cookies[CUSTOMER_COOKIE_NAME];
        }

        protected virtual void SetCustomerCookie(Guid customerGuid)
        {
            if (_httpContext != null && _httpContext.Response != null)
            {
                var cookie = new HttpCookie(CUSTOMER_COOKIE_NAME);
                cookie.HttpOnly = true;
                cookie.Value = customerGuid.ToString();
                if (customerGuid == Guid.Empty)
                {
                    cookie.Expires = DateTime.Now.AddMonths(-1);
                }
                else
                {
                    var cookieExpires = 24*365; //TODO make configurable
                    cookie.Expires = DateTime.Now.AddHours(cookieExpires);
                }

                _httpContext.Response.Cookies.Remove(CUSTOMER_COOKIE_NAME);
                _httpContext.Response.Cookies.Add(cookie);
            }
        }

        protected virtual Language GetLanguageFromUrl()
        {
            if (_httpContext == null || _httpContext.Request == null)
                return null;

            var virtualPath = _httpContext.Request.AppRelativeCurrentExecutionFilePath;
            var applicationPath = _httpContext.Request.ApplicationPath;
            if (!virtualPath.IsLocalizedUrl(applicationPath, false))
                return null;

            var seoCode = virtualPath.GetLanguageSeoCodeFromUrl(applicationPath, false);
            if (String.IsNullOrEmpty(seoCode))
                return null;

            var language = _languageService
                .GetAllLanguages()
                .FirstOrDefault(l => seoCode.Equals(l.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase));
            if (language != null && language.Published)
            {
                return language;
            }

            return null;
        }

        protected virtual Language GetLanguageFromBrowserSettings()
        {
            if (_httpContext == null ||
                _httpContext.Request == null ||
                _httpContext.Request.UserLanguages == null)
                return null;

            var userLanguage = _httpContext.Request.UserLanguages.FirstOrDefault();
            if (String.IsNullOrEmpty(userLanguage))
                return null;

            var language = _languageService
                .GetAllLanguages()
                .FirstOrDefault(l => userLanguage.Equals(l.LanguageCulture, StringComparison.InvariantCultureIgnoreCase));
            if (language != null && language.Published)
            {
                return language;
            }

            return null;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the current customer
        /// </summary>
        public virtual Customer CurrentCustomer
        {
            get
            {
                if (_cachedCustomer != null)
                    return _cachedCustomer;

                Customer customer = null;
                if (_httpContext == null || _httpContext is FakeHttpContext)
                {
                    //check whether request is made by a background task
                    //in this case return built-in customer record for background task
                    customer = _customerService.GetCustomerBySystemName(SystemCustomerNames.BackgroundTask);
                }

                //check whether request is made by a search engine
                //in this case return built-in customer record for search engines 
                //or comment the following two lines of code in order to disable this functionality
                if (customer == null || customer.Deleted || !customer.Active)
                {
                    if (_userAgentHelper.IsSearchEngine())
                        customer = _customerService.GetCustomerBySystemName(SystemCustomerNames.SearchEngine);
                }

                //registered user
                if (customer == null || customer.Deleted || !customer.Active)
                {
                    customer = _authenticationService.GetAuthenticatedCustomer();
                }


                //load guest customer
                if (customer == null || customer.Deleted || !customer.Active)
                {
                    var customerCookie = GetCustomerCookie();
                    if (customerCookie != null && !String.IsNullOrEmpty(customerCookie.Value))
                    {
                        Guid customerGuid;
                        if (Guid.TryParse(customerCookie.Value, out customerGuid))
                        {
                            var customerByCookie = _customerService.GetCustomerByGuid(customerGuid);
                            if (customerByCookie != null &&
                                //this customer (from cookie) should not be registered
                                !customerByCookie.IsRegistered())
                                customer = customerByCookie;
                        }
                    }
                }

                //create guest if not exists
                if (customer == null || customer.Deleted || !customer.Active)
                {
                    customer = _customerService.InsertGuestCustomer();
                    //customer = new Customer
                    //{
                    //    CustomerGuid = Guid.NewGuid(),
                    //    Active = true,
                    //    CreatedOnUtc = DateTime.UtcNow,
                    //    LastActivityDateUtc = DateTime.UtcNow
                    //};
                }


                //validation
                if (!customer.Deleted && customer.Active)
                {
                    SetCustomerCookie(customer.CustomerGuid);
                    _cachedCustomer = customer;
                }

                return _cachedCustomer;
            }
            set
            {
                SetCustomerCookie(value.CustomerGuid);
                _cachedCustomer = value;
            }
        }

        /// <summary>
        ///     Get or set current user working language
        /// </summary>
        public virtual Language WorkingLanguage
        {
            get
            {
                if (_cachedLanguage != null)
                    return _cachedLanguage;

                Language detectedLanguage = null;
                if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    //get language from URL
                    detectedLanguage = GetLanguageFromUrl();
                }
                if (detectedLanguage == null && _localizationSettings.AutomaticallyDetectLanguage)
                {
                    //get language from browser settings
                    //but we do it only once
                    if (!CurrentCustomer.GetAttribute<bool>(SystemCustomerAttributeNames.LanguageAutomaticallyDetected,
                        _genericAttributeService, _ksSystemContext.CurrentSystem.Id))
                    {
                        detectedLanguage = GetLanguageFromBrowserSettings();
                        if (detectedLanguage != null)
                        {
                            _genericAttributeService.SaveAttribute(CurrentCustomer,
                                SystemCustomerAttributeNames.LanguageAutomaticallyDetected,
                                true, _ksSystemContext.CurrentSystem.Id);
                        }
                    }
                }
                if (detectedLanguage != null)
                {
                    //the language is detected. now we need to save it
                    if (CurrentCustomer.GetAttribute<int>(SystemCustomerAttributeNames.LanguageId,
                        _genericAttributeService, _ksSystemContext.CurrentSystem.Id) != detectedLanguage.Id)
                    {
                        _genericAttributeService.SaveAttribute(CurrentCustomer, SystemCustomerAttributeNames.LanguageId,
                            detectedLanguage.Id, _ksSystemContext.CurrentSystem.Id);
                    }
                }

                var allLanguages = _languageService.GetAllLanguages(storeId: _ksSystemContext.CurrentSystem.Id);
                //find current customer language
                var languageId = CurrentCustomer.GetAttribute<int>(SystemCustomerAttributeNames.LanguageId,
                    _genericAttributeService, _ksSystemContext.CurrentSystem.Id);
                var language = allLanguages.FirstOrDefault(x => x.Id == languageId);
                if (language == null)
                {
                    //it not found, then let's load the default currency for the current language (if specified)
                    languageId = _ksSystemContext.CurrentSystem.DefaultLanguageId;
                    language = allLanguages.FirstOrDefault(x => x.Id == languageId);
                }
                if (language == null)
                {
                    //it not specified, then return the first (filtered by current store) found one
                    language = allLanguages.FirstOrDefault();
                }
                if (language == null)
                {
                    //it not specified, then return the first found one
                    language = _languageService.GetAllLanguages().FirstOrDefault();
                }

                //cache
                _cachedLanguage = language;
                return _cachedLanguage;
            }
            set
            {
                var languageId = value != null ? value.Id : 0;
                _genericAttributeService.SaveAttribute(CurrentCustomer,
                    SystemCustomerAttributeNames.LanguageId,
                    languageId, _ksSystemContext.CurrentSystem.Id);

                //reset cache
                _cachedLanguage = null;
            }
        }

        /// <summary>
        ///     Get or set current user working currency
        /// </summary>
        public virtual Currency WorkingCurrency
        {
            get
            {
                if (_cachedCurrency != null)
                    return _cachedCurrency;

                //return primary store currency when we're in admin area/mode
                if (IsAdmin)
                {
                    var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
                    if (primaryStoreCurrency != null)
                    {
                        //cache
                        _cachedCurrency = primaryStoreCurrency;
                        return primaryStoreCurrency;
                    }
                }

                var allCurrencies = _currencyService.GetAllCurrencies(ksSystemId: _ksSystemContext.CurrentSystem.Id);
                //find a currency previously selected by a customer
                var currencyId = CurrentCustomer.GetAttribute<int>(SystemCustomerAttributeNames.CurrencyId,
                    _genericAttributeService, _ksSystemContext.CurrentSystem.Id);
                var currency = allCurrencies.FirstOrDefault(x => x.Id == currencyId);
                if (currency == null)
                {
                    //it not found, then let's load the default currency for the current language (if specified)
                    currencyId = WorkingLanguage.DefaultCurrencyId;
                    currency = allCurrencies.FirstOrDefault(x => x.Id == currencyId);
                }
                if (currency == null)
                {
                    //it not found, then return the first (filtered by current store) found one
                    currency = allCurrencies.FirstOrDefault();
                }
                if (currency == null)
                {
                    //it not specified, then return the first found one
                    currency = _currencyService.GetAllCurrencies().FirstOrDefault();
                }

                //cache
                _cachedCurrency = currency;
                return _cachedCurrency;
            }
            set
            {
                var currencyId = value != null ? value.Id : 0;
                _genericAttributeService.SaveAttribute(CurrentCustomer,
                    SystemCustomerAttributeNames.CurrencyId,
                    currencyId, _ksSystemContext.CurrentSystem.Id);

                //reset cache
                _cachedCurrency = null;
            }
        }

        /// <summary>
        ///     Get or set value indicating whether we're in admin area
        /// </summary>
        public virtual bool IsAdmin { get; set; }

        #endregion
    }
}