using System;
using System.Collections.Generic;
using System.Web;
using Ks.Core;
using Ks.Core.Domain.Customers;
using Ks.Core.Domain.Directory;
using Ks.Core.Domain.Messages;
using Ks.Core.Domain.System;
using Ks.Services.Common;
using Ks.Services.Customers;
using Ks.Services.Directory;
using Ks.Services.Events;
using Ks.Services.Helpers;
using Ks.Services.KsSystems;
using Ks.Services.Localization;
using Ks.Services.Media;

namespace Ks.Services.Messages
{
    public partial class MessageTokenProvider : IMessageTokenProvider
    {
        #region Fields

        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        //private readonly IPriceFormatter _priceFormatter;
        private readonly ICurrencyService _currencyService;
        private readonly IWorkContext _workContext;
        private readonly IDownloadService _downloadService;
        //private readonly IOrderService _orderService;
        //private readonly IPaymentService _paymentService;
        //private readonly IProductAttributeParser _productAttributeParser;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly IKsSystemService _ksSystemService;
        private readonly IKsSystemContext _ksSystemContext;

        private readonly MessageTemplatesSettings _templatesSettings;
        //private readonly CatalogSettings _catalogSettings;
        //private readonly TaxSettings _taxSettings;
        private readonly CurrencySettings _currencySettings;
        //private readonly ShippingSettings _shippingSettings;

        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public MessageTokenProvider(ILanguageService languageService,
            ILocalizationService localizationService,
            IDateTimeHelper dateTimeHelper,
            //IPriceFormatter priceFormatter,
            ICurrencyService currencyService,
            IWorkContext workContext,
            IDownloadService downloadService,
            //IOrderService orderService,
            //IPaymentService paymentService,
            IKsSystemService ksSystemService,
            IKsSystemContext ksSystemContext,
            //IProductAttributeParser productAttributeParser,
            IAddressAttributeFormatter addressAttributeFormatter,
            MessageTemplatesSettings templatesSettings,
            //CatalogSettings catalogSettings,
            //TaxSettings taxSettings,
            CurrencySettings currencySettings,
            //ShippingSettings shippingSettings,
            IEventPublisher eventPublisher)
        {
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._dateTimeHelper = dateTimeHelper;
            //this._priceFormatter = priceFormatter;
            this._currencyService = currencyService;
            this._workContext = workContext;
            this._downloadService = downloadService;
            //this._orderService = orderService;
            //this._paymentService = paymentService;
            //this._productAttributeParser = productAttributeParser;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._ksSystemService = ksSystemService;
            this._ksSystemContext = ksSystemContext;

            this._templatesSettings = templatesSettings;
            //this._catalogSettings = catalogSettings;
            //this._taxSettings = taxSettings;
            this._currencySettings = currencySettings;
            //this._shippingSettings = shippingSettings;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get store URL
        /// </summary>
        /// <param name="storeId">Store identifier; Pass 0 to load URL of the current store</param>
        /// <param name="useSsl">Use SSL</param>
        /// <returns></returns>
        protected virtual string GetStoreUrl(int storeId = 0, bool useSsl = false)
        {
            var system = _ksSystemService.GetKsSystemById(storeId) ?? _ksSystemContext.CurrentSystem;

            if (system == null)
                throw new Exception("No system could be loaded");

            return useSsl ? system.SecureUrl : system.Url;
        }

        #endregion

        #region Methods

        public virtual void AddSystemTokens(IList<Token> tokens, KsSystem system, EmailAccount emailAccount)
        {
            if (emailAccount == null)
                throw new ArgumentNullException("emailAccount");

            tokens.Add(new Token("System.Name", system.GetLocalized(x => x.Name)));
            tokens.Add(new Token("System.URL", system.Url, true));
            tokens.Add(new Token("System.Email", emailAccount.Email));
            tokens.Add(new Token("System.CompanyName", system.CompanyName));
            tokens.Add(new Token("System.CompanyAddress", system.CompanyAddress));
            tokens.Add(new Token("System.CompanyPhoneNumber", system.CompanyPhoneNumber));
            //tokens.Add(new Token("Store.CompanyVat", system.CompanyVat));

            //event notification
            _eventPublisher.EntityTokensAdded(system, tokens);
        }

        public virtual void AddCustomerTokens(IList<Token> tokens, Customer customer)
        {
            tokens.Add(new Token("Customer.Email", customer.Email));
            tokens.Add(new Token("Customer.Username", customer.Username));
            tokens.Add(new Token("Customer.FullName", customer.GetFullName()));
            tokens.Add(new Token("Customer.FirstName", customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName)));
            tokens.Add(new Token("Customer.LastName", customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName)));
            //tokens.Add(new Token("Customer.VatNumber", customer.GetAttribute<string>(SystemCustomerAttributeNames.VatNumber)));
            //tokens.Add(new Token("Customer.VatNumberStatus", ((VatNumberStatus)customer.GetAttribute<int>(SystemCustomerAttributeNames.VatNumberStatusId)).ToString()));



            //note: we do not use SEO friendly URLS because we can get errors caused by having .(dot) in the URL (from the email address)
            //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
            string passwordRecoveryUrl = string.Format("{0}passwordrecovery/confirm?token={1}&email={2}", GetStoreUrl(), customer.GetAttribute<string>(SystemCustomerAttributeNames.PasswordRecoveryToken), HttpUtility.UrlEncode(customer.Email));
            string accountActivationUrl = string.Format("{0}customer/activation?token={1}&email={2}", GetStoreUrl(), customer.GetAttribute<string>(SystemCustomerAttributeNames.AccountActivationToken), HttpUtility.UrlEncode(customer.Email));
            //var wishlistUrl = string.Format("{0}wishlist/{1}", GetStoreUrl(), customer.CustomerGuid);
            tokens.Add(new Token("Customer.PasswordRecoveryURL", passwordRecoveryUrl, true));
            tokens.Add(new Token("Customer.AccountActivationURL", accountActivationUrl, true));
            //tokens.Add(new Token("Wishlist.URLForCustomer", wishlistUrl, true));

            //event notification
            _eventPublisher.EntityTokensAdded(customer, tokens);
        }

        public virtual string[] GetListOfAllowedTokens()
        {
            var allowedTokens = new List<string>
            {
                "%System.Name%",
                "%System.URL%",
                "%System.Email%",
                "%System.CompanyName%",
                "%System.CompanyAddress%",
                "%System.CompanyPhoneNumber%",
                "%Customer.Email%", 
                "%Customer.Username%",
                "%Customer.FullName%",
                "%Customer.FirstName%",
                "%Customer.LastName%",
                "%Customer.PasswordRecoveryURL%", 
                "%Customer.AccountActivationURL%", 
                
            };
            return allowedTokens.ToArray();
        }

        #endregion
    }
}