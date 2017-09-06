using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ks.Core;
using Ks.Core.Domain;
using Ks.Core.Domain.Catalog;
using Ks.Core.Domain.Customers;
using Ks.Core.Domain.Security;
using Ks.Services.Authentication;
using Ks.Services.Common;
using Ks.Services.Customers;
using Ks.Services.Directory;
using Ks.Services.Events;
using Ks.Services.Helpers;
using Ks.Services.Localization;
using Ks.Services.Logging;
using Ks.Services.Messages;
using Ks.Web.Framework;
using Ks.Web.Framework.Controllers;
using Ks.Web.Framework.Security;
using Ks.Web.Framework.Security.Captcha;
using Ks.Web.Models.Common;
using Ks.Web.Models.Customer;
using WebGrease.Css.Extensions;

namespace Ks.Web.Controllers
{
    public class CustomerController : BasePublicController
    {
        #region Fields
        private readonly IAuthenticationService _authenticationService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly CustomerSettings _customerSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEventPublisher _eventPublisher;

        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly CaptchaSettings _captchaSettings;
        private readonly KsSystemInformationSettings _ksSystemInformationSettings;

        #endregion

        #region Ctor

        public CustomerController(IAuthenticationService authenticationService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ICustomerRegistrationService customerRegistrationService,
            CustomerSettings customerSettings,
            ICustomerActivityService customerActivityService,
            IEventPublisher eventPublisher,
            IWorkflowMessageService workflowMessageService,
            CaptchaSettings captchaSettings,
            KsSystemInformationSettings ksSystemInformationSettings)
        {
            this._authenticationService = authenticationService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._customerService = customerService;
            this._genericAttributeService = genericAttributeService;
            this._customerRegistrationService = customerRegistrationService;
            this._customerSettings = customerSettings;
            this._customerActivityService = customerActivityService;
            this._eventPublisher = eventPublisher;
            this._workflowMessageService = workflowMessageService;
            this._captchaSettings = captchaSettings;
            this._ksSystemInformationSettings = ksSystemInformationSettings;
        }

        #endregion

        

        #region Login / logout

        [KsHttpsRequirement(SslRequirement.Yes)]
        public ActionResult Login(bool? checkoutAsGuest)
        {
            var model = new LoginModel
            {
                UsernamesEnabled = _customerSettings.UsernamesEnabled,
                CheckoutAsGuest = checkoutAsGuest.GetValueOrDefault(),
                DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage
            };
            return View(model);
        }

        [HttpPost]
        [CaptchaValidator]
        //available even when navigation is not allowed
        [PublicKsSystemAllowNavigation(true)]
        public ActionResult Login(LoginModel model, string returnUrl, bool captchaValid)
        {
            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage && !captchaValid)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Common.WrongCaptcha"));
            }

            if (ModelState.IsValid)
            {
                if (_customerSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }
                var loginResult = _customerRegistrationService.ValidateCustomer(_customerSettings.UsernamesEnabled ? model.Username : model.Email, model.Password);
                switch (loginResult)
                {
                    case CustomerLoginResults.Successful:
                        {
                            var customer = _customerSettings.UsernamesEnabled ? _customerService.GetCustomerByUsername(model.Username) : _customerService.GetCustomerByEmail(model.Email);

                            //sign in new customer
                            _authenticationService.SignIn(customer, model.RememberMe);

                            //raise event       
                            _eventPublisher.Publish(new CustomerLoggedinEvent(customer));

                            //activity log
                            _customerActivityService.InsertActivity("PublicStore.Login", _localizationService.GetResource("ActivityLog.PublicStore.Login"), customer);

                            if (String.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                                return RedirectToRoute("HomePage");

                            return Redirect(returnUrl);
                        }
                    case CustomerLoginResults.CustomerNotExist:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist"));
                        break;
                    case CustomerLoginResults.Deleted:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.Deleted"));
                        break;
                    case CustomerLoginResults.NotActive:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotActive"));
                        break;
                    case CustomerLoginResults.NotRegistered:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered"));
                        break;
                    case CustomerLoginResults.WrongPassword:
                    default:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials"));
                        break;
                }
            }

            //If we got this far, something failed, redisplay form
            model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage;
            return View(model);
        }

        //available even when navigation is not allowed
        [PublicKsSystemAllowNavigation(true)]
        public ActionResult Logout()
        {
            ////external authentication
            //ExternalAuthorizerHelper.RemoveParameters();

            //if (_workContext.OriginalCustomerIfImpersonated != null)
            //{
            //    //logout impersonated customer
            //    _genericAttributeService.SaveAttribute<int?>(_workContext.OriginalCustomerIfImpersonated,
            //        SystemCustomerAttributeNames.ImpersonatedCustomerId, null);
            //    //redirect back to customer details page (admin area)
            //    return this.RedirectToAction("Edit", "Customer", new { id = _workContext.CurrentCustomer.Id, area = "Admin" });

            //}

            //activity log
            _customerActivityService.InsertActivity("PublicStore.Logout", _localizationService.GetResource("ActivityLog.PublicStore.Logout"));
            //standard logout 
            _authenticationService.SignOut();

            //EU Cookie
            if (_ksSystemInformationSettings.DisplayEuCookieLawWarning)
            {
                //the cookie law message should not pop up immediately after logout.
                //otherwise, the user will have to click it again...
                //and thus next visitor will not click it... so violation for that cookie law..
                //the only good solution in this case is to store a temporary variable
                //indicating that the EU cookie popup window should not be displayed on the next page open (after logout redirection to homepage)
                //but it'll be displayed for further page loads
                TempData["ks.IgnoreEuCookieLawWarning"] = true;
            }

            return RedirectToRoute("HomePage");
        }

        #endregion

        #region Password recovery

        [KsHttpsRequirement(SslRequirement.Yes)]
        //available even when navigation is not allowed
        [PublicKsSystemAllowNavigation(true)]
        public ActionResult PasswordRecovery()
        {
            var model = new PasswordRecoveryModel();
            return View(model);
        }

        [HttpPost, ActionName("PasswordRecovery")]
        [PublicAntiForgery]
        [FormValueRequired("send-email")]
        //available even when navigation is not allowed
        [PublicKsSystemAllowNavigation(true)]
        public ActionResult PasswordRecoverySend(PasswordRecoveryModel model)
        {
            if (ModelState.IsValid)
            {
                var customer = _customerService.GetCustomerByEmail(model.Email);
                if (customer != null && customer.Active && !customer.Deleted)
                {
                    //save token and current date
                    var passwordRecoveryToken = Guid.NewGuid();
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.PasswordRecoveryToken, passwordRecoveryToken.ToString());
                    DateTime? generatedDateTime = DateTime.UtcNow;
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.PasswordRecoveryTokenDateGenerated, generatedDateTime);

                    //send email
                    _workflowMessageService.SendCustomerPasswordRecoveryMessage(customer, _workContext.WorkingLanguage.Id);

                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.EmailHasBeenSent");
                }
                else
                {
                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.EmailNotFound");
                }

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }


        [KsHttpsRequirement(SslRequirement.Yes)]
        //available even when navigation is not allowed
        [PublicKsSystemAllowNavigation(true)]
        public ActionResult PasswordRecoveryConfirm(string token, string email)
        {
            var customer = _customerService.GetCustomerByEmail(email);
            if (customer == null)
                return RedirectToRoute("HomePage");

            var model = new PasswordRecoveryConfirmModel();
            return View(model);
        }

        [HttpPost, ActionName("PasswordRecoveryConfirm")]
        [PublicAntiForgery]
        [FormValueRequired("set-password")]
        //available even when navigation is not allowed
        [PublicKsSystemAllowNavigation(true)]
        public ActionResult PasswordRecoveryConfirmPOST(string token, string email, PasswordRecoveryConfirmModel model)
        {
            var customer = _customerService.GetCustomerByEmail(email);
            if (customer == null)
                return RedirectToRoute("HomePage");

            //validate token
            if (!customer.IsPasswordRecoveryTokenValid(token))
            {
                model.DisablePasswordChanging = true;
                model.Result = _localizationService.GetResource("Account.PasswordRecovery.WrongToken");
            }

            //validate token expiration date
            if (customer.IsPasswordRecoveryLinkExpired(_customerSettings))
            {
                model.DisablePasswordChanging = true;
                model.Result = _localizationService.GetResource("Account.PasswordRecovery.LinkExpired");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var response = _customerRegistrationService.ChangePassword(new ChangePasswordRequest(email,
                    false, _customerSettings.DefaultPasswordFormat, model.NewPassword));
                if (response.Success)
                {
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.PasswordRecoveryToken, "");

                    model.DisablePasswordChanging = true;
                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.PasswordHasBeenChanged");
                }
                else
                {
                    model.Result = response.Errors.FirstOrDefault();
                }

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion
        
    }
}