using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
    public partial class CustomerController : BaseAdminController
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly ICustomerService _customerService;
        private readonly IEncryptionService _encryptionService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkFlowService _workFlowService;
        //private readonly TaxSettings _taxSettings;
        //private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICityService _cityService;
        private readonly IAddressService _addressService;
        private readonly CustomerSettings _customerSettings;
        //private readonly ITaxService _taxService;
        private readonly IWorkContext _workContext;
        //private readonly IVendorService _vendorService;
        private readonly IKsSystemContext _ksSystemContext;
        //private readonly IPriceFormatter _priceFormatter;
        //private readonly IOrderService _orderService;
        private readonly IExportManager _exportManager;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IContributionService _contributionService;
        private readonly ILoanService _loanService;
        private readonly IBenefitService _benefitService;
        private readonly ITabService _tabService;
        //private readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
        //private readonly IPriceCalculationService _priceCalculationService;
        //private readonly IProductAttributeFormatter _productAttributeFormatter;
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

        #endregion

        #region Constructors

        public CustomerController(ISettingService settingService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ICustomerRegistrationService customerRegistrationService,
            IEncryptionService encryptionService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IWorkFlowService workFlowService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            ICityService cityService,
            IAddressService addressService,
            CustomerSettings customerSettings,
            IWorkContext workContext,
            IExportManager exportManager,
            ICustomerActivityService customerActivityService,
            IContributionService contributionService,
            ILoanService loanService,
            IBenefitService benefitService,
            ITabService tabService,
            IPermissionService permissionService,
            IQueuedEmailService queuedEmailService,
            EmailAccountSettings emailAccountSettings,
            IEmailAccountService emailAccountService,
            AddressSettings addressSettings,
            IKsSystemService ksSystemService,
            ICustomerAttributeParser customerAttributeParser,
            ICustomerAttributeService customerAttributeService,
            IAddressAttributeParser addressAttributeParser,
            IAddressAttributeService addressAttributeService,
            IAddressAttributeFormatter addressAttributeFormatter,
            ContributionSettings contributionSettings,
            SequenceIdsSettings sequenceIdsSettings,
            LoanSettings stateActivityettings, IKsSystemContext ksSystemContext)
        {
            this._settingService = settingService;
            this._customerService = customerService;
            this._genericAttributeService = genericAttributeService;
            this._customerRegistrationService = customerRegistrationService;
            this._encryptionService = encryptionService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._workFlowService = workFlowService;
            //this._taxSettings = taxSettings;
            //this._rewardPointsSettings = rewardPointsSettings;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._cityService = cityService;
            this._addressService = addressService;
            this._customerSettings = customerSettings;
            //this._taxService = taxService;
            this._workContext = workContext;
            //this._vendorService = vendorService;
            //this._storeContext = storeContext;
            //this._priceFormatter = priceFormatter;
            //this._orderService = orderService;
            this._exportManager = exportManager;
            this._customerActivityService = customerActivityService;
            this._contributionService = contributionService;
            this._loanService = loanService;
            this._tabService = tabService;
            this._benefitService = benefitService;
            //this._backInStockSubscriptionService = backInStockSubscriptionService;
            //this._priceCalculationService = priceCalculationService;
            //this._productAttributeFormatter = productAttributeFormatter;
            this._permissionService = permissionService;
            this._queuedEmailService = queuedEmailService;
            this._emailAccountSettings = emailAccountSettings;
            this._emailAccountService = emailAccountService;
            //this._forumSettings = forumSettings;
            //this._forumService = forumService;
            //this._openAuthenticationService = openAuthenticationService;
            this._addressSettings = addressSettings;
            this._ksSystemService = ksSystemService;
            this._customerAttributeParser = customerAttributeParser;
            this._customerAttributeService = customerAttributeService;
            this._addressAttributeParser = addressAttributeParser;
            this._addressAttributeService = addressAttributeService;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._contributionSettings = contributionSettings;
            this._sequenceIdsSettings = sequenceIdsSettings;
            //this._affiliateService = affiliateService;
            //this._workflowMessageService = workflowMessageService;
            //this._rewardPointService = rewardPointService;
            this._loanSettings = stateActivityettings;
            _ksSystemContext = ksSystemContext;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual void PrepareSettingPaymentAmount(CustomerModel model)
        {
            model.IsActiveAmount1 = _contributionSettings.IsActiveAmount1;
            model.NameAmount1 = _contributionSettings.NameAmount1;
            model.IsActiveAmount2 = _contributionSettings.IsActiveAmount2;
            model.NameAmount2 = _contributionSettings.NameAmount2;
            model.IsActiveAmount3 = _contributionSettings.IsActiveAmount3;
            model.NameAmount3 = _contributionSettings.NameAmount3;
        }
        [NonAction]
        protected virtual string GetCustomerRolesNames(IList<CustomerRole> customerRoles, string separator = ",")
        {
            var sb = new StringBuilder();
            for (int i = 0; i < customerRoles.Count; i++)
            {
                sb.Append(customerRoles[i].Name);
                if (i != customerRoles.Count - 1)
                {
                    sb.Append(separator);
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }

        [NonAction]
        protected virtual CustomerModel PrepareCustomerModelForList(Customer customer)
        {
            return new CustomerModel
            {
                Id = customer.Id,
                Email = customer.Email,
                Username = customer.Username,
                FullName = customer.GetFullName(),
                Phone = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone),
                CustomerRoleNames = GetCustomerRolesNames(customer.CustomerRoles.ToList()),
                Active = customer.Active,
                CreatedOn = _dateTimeHelper.ConvertToUserTime(customer.CreatedOnUtc, DateTimeKind.Utc),
                LastActivityDate = _dateTimeHelper.ConvertToUserTime(customer.LastActivityDateUtc, DateTimeKind.Utc),
            };
        }

        [NonAction]
        protected virtual void PrepareCustomerAttributeModel(CustomerModel model, Customer customer)
        {
            var customerAttributes = _customerAttributeService.GetAllCustomerAttributes();
            foreach (var attribute in customerAttributes)
            {
                var attributeModel = new CustomerModel.CustomerAttributeModel
                {
                    Id = attribute.Id,
                    Name = attribute.Name,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _customerAttributeService.GetCustomerAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new CustomerModel.CustomerAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.Name,
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(attributeValueModel);
                    }
                }


                //set already selected attributes
                if (customer != null)
                {
                    var selectedCustomerAttributes = customer.GetAttribute<string>(SystemCustomerAttributeNames.CustomCustomerAttributes, _genericAttributeService);
                    switch (attribute.AttributeControlType)
                    {
                        case AttributeControlType.DropdownList:
                        case AttributeControlType.RadioList:
                        case AttributeControlType.Checkboxes:
                            {
                                if (!String.IsNullOrEmpty(selectedCustomerAttributes))
                                {
                                    //clear default selection
                                    foreach (var item in attributeModel.Values)
                                        item.IsPreSelected = false;

                                    //select new values
                                    var selectedValues = _customerAttributeParser.ParseCustomerAttributeValues(selectedCustomerAttributes);
                                    foreach (var attributeValue in selectedValues)
                                        foreach (var item in attributeModel.Values)
                                            if (attributeValue.Id == item.Id)
                                                item.IsPreSelected = true;
                                }
                            }
                            break;
                        case AttributeControlType.ReadonlyCheckboxes:
                            {
                                //do nothing
                                //values are already pre-set
                            }
                            break;
                        case AttributeControlType.TextBox:
                        case AttributeControlType.MultilineTextbox:
                        case AttributeControlType.Datepicker:
                            {
                                if (!String.IsNullOrEmpty(selectedCustomerAttributes))
                                {
                                    var enteredText = _customerAttributeParser.ParseValues(selectedCustomerAttributes, attribute.Id);
                                    if (enteredText.Count > 0)
                                        attributeModel.DefaultValue = enteredText[0];
                                }
                            }
                            break;
                        case AttributeControlType.ColorSquares:
                        case AttributeControlType.FileUpload:
                        default:
                            //not supported attribute control types
                            break;
                    }
                }

                model.CustomerAttributes.Add(attributeModel);
            }
        }


        [NonAction]
        protected virtual void PrepareCustomerModel(CustomerModel model, Customer customer, bool excludeProperties)
        {
            if (customer != null)
            {
                model.Id = customer.Id;
                var contribution = _contributionService.GetContributionById(customerId: model.Id, stateId: 1);
                model.HasContributions = contribution != null;
                model.Contribution = contribution.ToModel();
                var loans = _loanService.GetLoansByCustomer(model.Id);
                if (loans != null && loans.Count > 0)
                {
                    model.HasLoans = true;
                    model.LoanModels = loans.Select(x =>
                    {
                        var toModel = x.ToModel();
                        toModel.StateName = x.IsAuthorized ? "Aprobado" : "No Aprobado";
                        return toModel;
                    }).ToList();
                }

                var benefits = _benefitService.GetAllContributionBenefitByCustomer(customer.Id);
                if (benefits != null && benefits.Count > 0)
                    model.HasContributionBenefits = true;

                if (benefits != null)
                    foreach (var b in benefits)
                    {
                        var benefit = _benefitService.GetBenefitById(b.BenefitId);
                        if (benefit.BenefitTypeId == (int)BenefitType.Beneficio && b.Active)
                            model.HasBenefit = true;
                    }

                if (model.HasContributions && contribution != null)
                {
                    model.Contribution.AuthorizeDiscount = contribution.AuthorizeDiscount;
                    model.Contribution.AmountPayed = contribution.AmountPayed;
                    model.Contribution.AmountMeta = contribution.AmountMeta;
                    model.Contribution.DelayCycles = contribution.DelayCycles;
                    model.Contribution.CreatedOn = _dateTimeHelper.ConvertToUserTime(contribution.CreatedOnUtc, DateTimeKind.Utc);
                    if (contribution.UpdatedOnUtc.HasValue)
                    {
                        model.Contribution.TotalOfCycles = ((contribution.UpdatedOnUtc.Value.Year - contribution.CreatedOnUtc.Year) * 12) + contribution.UpdatedOnUtc.Value.Month - contribution.CreatedOnUtc.Month;
                        model.Contribution.TotalOfCycles = Convert.ToInt32(Math.Round(contribution.UpdatedOnUtc.Value.Subtract(contribution.CreatedOnUtc).Days / (365.25 / 12), 2));
                        model.Contribution.UpdatedOn = _dateTimeHelper.ConvertToUserTime(contribution.UpdatedOnUtc.Value, DateTimeKind.Utc);
                    }
                    model.Contribution.Description = contribution.Description;
                }

                if (!excludeProperties)
                {
                    model.Email = customer.Email;
                    model.Username = customer.Username;
                    model.AdminComment = customer.AdminComment;
                    model.Active = customer.Active;

                    model.CreatedOn = _dateTimeHelper.ConvertToUserTime(customer.CreatedOnUtc, DateTimeKind.Utc);
                    model.LastActivityDate = _dateTimeHelper.ConvertToUserTime(customer.LastActivityDateUtc, DateTimeKind.Utc);
                    model.LastIpAddress = customer.LastIpAddress;
                    model.LastVisitedPage = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastVisitedPage);
                    model.SelectedCustomerRoleIds = customer.CustomerRoles.Select(cr => cr.Id).ToArray();

                    //form fields
                    model.FirstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName);
                    model.LastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName);
                    model.AdmCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.AdmCode);
                    model.Dni = customer.GetAttribute<string>(SystemCustomerAttributeNames.Dni);
                    model.MilitarySituationId = customer.GetAttribute<int>(SystemCustomerAttributeNames.MilitarySituationId);
                    model.DeclaratoryLetter = customer.GetAttribute<int>(SystemCustomerAttributeNames.DeclaratoryLetter);
                    model.DateOfAdmission = customer.GetAttribute<DateTime?>(SystemCustomerAttributeNames.DateOfAdmission);
                    model.Gender = customer.GetAttribute<string>(SystemCustomerAttributeNames.Gender);
                    model.DateOfBirth = customer.GetAttribute<DateTime?>(SystemCustomerAttributeNames.DateOfBirth);
                    model.StreetAddress = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress);
                    model.StreetAddress2 = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress2);
                    model.CountryId = customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId);
                    model.StateProvinceId = customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId);
                    model.CityId = customer.GetAttribute<int>(SystemCustomerAttributeNames.CityId);
                    model.Phone = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
                    model.Fax = customer.GetAttribute<string>(SystemCustomerAttributeNames.Fax);

                }
            }

            model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
            model.AllowUsersToChangeUsernames = _customerSettings.AllowUsersToChangeUsernames;

            PrepareCustomerAttributeModel(model, customer);

            model.GenderEnabled = _customerSettings.GenderEnabled;
            model.GenderRequired = _customerSettings.GenderRequired;
            model.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
            model.DateOfBirthRequired = _customerSettings.DateOfBirthRequired;
            model.StreetAddressEnabled = _customerSettings.StreetAddressEnabled;
            model.StreetAddress2Enabled = _customerSettings.StreetAddress2Enabled;
            model.CityEnabled = _customerSettings.CityEnabled;
            model.CountryEnabled = _customerSettings.CountryEnabled;
            model.StateProvinceEnabled = _customerSettings.StateProvinceEnabled;
            model.PhoneEnabled = _customerSettings.PhoneEnabled;
            model.FaxEnabled = _customerSettings.FaxEnabled;

            model.AvailableMilitarySituations = Ks.Web.Framework.Extensions.GetDescriptions(typeof(CustomerMilitarySituation));
            model.AvailableMilitarySituations.Insert(0, new SelectListItem { Value = "0", Text = "-------------" });


            //countries and states
            if (_customerSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
                foreach (var c in _countryService.GetAllCountries(showHidden: true))
                {
                    model.AvailableCountries.Add(new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (_customerSettings.StateProvinceEnabled)
                {
                    //states
                    var states = _stateProvinceService.GetStateProvincesByCountryId(model.CountryId).ToList();
                    if (states.Count > 0)
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectState"), Value = "0" });

                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                        }
                    }
                    else
                    {
                        bool anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);

                        model.AvailableStates.Add(new SelectListItem
                        {
                            Text = _localizationService.GetResource(anyCountrySelected ? "Admin.Address.Other" : "Admin.Address.SelectState"),
                            Value = "0"
                        });
                    }

                    //districts
                    var cities = _cityService.GetCitiesByStateProvinceId(model.StateProvinceId).ToList();
                    if (cities.Count > 0)
                    {
                        model.AvailableCities.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectCity"), Value = "0" });

                        foreach (var s in cities)
                        {
                            model.AvailableCities.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == model.CityId) });
                        }
                    }
                    else
                    {
                        bool anyStateSelected = model.AvailableCountries.Any(x => x.Selected);

                        model.AvailableCities.Add(new SelectListItem
                        {
                            Text = _localizationService.GetResource(anyStateSelected ? "Admin.Address.Other" : "Admin.Address.SelectCity"),
                            Value = "0"
                        });
                    }
                }
            }

            //customer roles
            model.AvailableCustomerRoles = _customerService
                .GetAllCustomerRoles(true)
                .Select(cr => cr.ToModel())
                .ToList();

            //sending of the welcome message:
            //1. "admin approval" registration method
            //2. already created customer
            //3. registered
            model.AllowSendingOfWelcomeMessage = _customerSettings.UserRegistrationType == UserRegistrationType.AdminApproval &&
                customer != null &&
                customer.IsRegistered();
            //sending of the activation message
            //1. "email validation" registration method
            //2. already created customer
            //3. registered
            //4. not active
            model.AllowReSendingOfActivationMessage = _customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation &&
                customer != null &&
                customer.IsRegistered() &&
                !customer.Active;
        }

        [NonAction]
        protected virtual void PrepareAddressModel(CustomerAddressModel model, Address address, Customer customer, bool excludeProperties)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            model.CustomerId = customer.Id;
            if (address != null)
            {
                if (!excludeProperties)
                {
                    model.Address = address.ToModel();
                }
            }

            if (model.Address == null)
                model.Address = new AddressModel();

            model.Address.FirstNameEnabled = true;
            model.Address.FirstNameRequired = true;
            model.Address.LastNameEnabled = true;
            model.Address.LastNameRequired = true;
            model.Address.EmailEnabled = true;
            model.Address.EmailRequired = true;
            model.Address.CountryEnabled = _addressSettings.CountryEnabled;
            model.Address.StateProvinceEnabled = _addressSettings.StateProvinceEnabled;
            model.Address.CityEnabled = _addressSettings.CityEnabled;
            model.Address.CityRequired = _addressSettings.CityRequired;
            model.Address.StreetAddressEnabled = _addressSettings.StreetAddressEnabled;
            model.Address.StreetAddressRequired = _addressSettings.StreetAddressRequired;
            model.Address.StreetAddress2Enabled = _addressSettings.StreetAddress2Enabled;
            model.Address.StreetAddress2Required = _addressSettings.StreetAddress2Required;
            model.Address.ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled;
            model.Address.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
            model.Address.PhoneEnabled = _addressSettings.PhoneEnabled;
            model.Address.PhoneRequired = _addressSettings.PhoneRequired;
            model.Address.FaxEnabled = _addressSettings.FaxEnabled;
            model.Address.FaxRequired = _addressSettings.FaxRequired;
            //countries
            model.Address.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(showHidden: true))
                model.Address.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == model.Address.CountryId) });
            //states
            var states = model.Address.CountryId.HasValue ? _stateProvinceService.GetStateProvincesByCountryId(model.Address.CountryId.Value, showHidden: true).ToList() : new List<StateProvince>();
            if (states.Count > 0)
            {
                foreach (var s in states)
                    model.Address.AvailableStates.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == model.Address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });
            //customer attribute services
            model.Address.PrepareCustomAddressAttributes(address, _addressAttributeService, _addressAttributeParser);
        }

        [NonAction]
        protected virtual string ValidateCustomerRoles(IList<CustomerRole> customerRoles)
        {
            if (customerRoles == null)
                throw new ArgumentNullException("customerRoles");

            //ensure a customer is not added to both 'Guests' and 'Registered' customer roles
            //ensure that a customer is in at least one required role ('Guests' and 'Registered')
            bool isInEmployerRole = customerRoles.FirstOrDefault(cr => cr.SystemName == SystemCustomerRoleNames.Employee) != null;
            bool isInAssociatedRole = customerRoles.FirstOrDefault(cr => cr.SystemName == SystemCustomerRoleNames.Associated) != null;
            if (!isInEmployerRole && !isInAssociatedRole)
                return "Ingrese por lo menos un rol de Trabajador o de Asociado";

            //no errors
            return "";
        }

        [NonAction]
        protected virtual string ParseCustomCustomerAttributes(Customer customer, FormCollection form)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (form == null)
                throw new ArgumentNullException("form");

            string attributesXml = "";
            var customerAttributes = _customerAttributeService.GetAllCustomerAttributes();
            foreach (var attribute in customerAttributes)
            {
                string controlId = string.Format("customer_attribute_{0}", attribute.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                int selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(cblAttributes))
                            {
                                foreach (var item in cblAttributes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _customerAttributeService.GetCustomerAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                    case AttributeControlType.Datepicker:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                string enteredText = ctrlAttributes.Trim();
                                attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.FileUpload:
                    //not supported customer attributes
                    default:
                        break;
                }
            }

            return attributesXml;
        }
        #endregion

        #region Customers

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //load registered customers by default
            var defaultRoleIds = new[] { _customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Associated).Id };
            var model = new CustomerListModel
            {
                UsernamesEnabled = _customerSettings.UsernamesEnabled,
                DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled,
                MonthOfBirthValues = DateTime.Now.GetMonthsList(_localizationService),
                DayOfBirthValues = DateTime.Now.GetDaysList(_localizationService),
                PhoneEnabled = _customerSettings.PhoneEnabled,
                AvailableCustomerRoles = _customerService.GetAllCustomerRoles(true).Select(cr => cr.ToModel()).ToList(),
                SearchCustomerRoleIds = defaultRoleIds,
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult CustomerList(DataSourceRequest command, CustomerListModel model,
            [ModelBinder(typeof(CommaSeparatedModelBinder))] int[] searchCustomerRoleIds)
        {
            //we use own own binder for searchCustomerRoleIds property 
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var searchDayOfBirth = 0;
            int searchMonthOfBirth = 0;
            if (!String.IsNullOrWhiteSpace(model.SearchDayOfBirth))
                searchDayOfBirth = Convert.ToInt32(model.SearchDayOfBirth);
            if (!String.IsNullOrWhiteSpace(model.SearchMonthOfBirth))
                searchMonthOfBirth = Convert.ToInt32(model.SearchMonthOfBirth);

            var customers = _customerService.GetAllCustomers(
                customerRoleIds: searchCustomerRoleIds,
                email: model.SearchEmail,
                username: model.SearchUsername,
                firstName: model.SearchFirstName,
                lastName: model.SearchLastName,
                dayOfBirth: searchDayOfBirth,
                monthOfBirth: searchMonthOfBirth,
                admCode: model.SearchAdmCode,
                dni: model.SearchDni,
                phone: model.SearchPhone,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = customers.Select(PrepareCustomerModelForList),
                Total = customers.TotalCount
            };

            return Json(gridModel);
        }

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var model = new CustomerModel();
            PrepareCustomerModel(model, null, false);
            //default value
            model.Active = true;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        [ValidateInput(false)]
        public ActionResult Create(CustomerModel model, bool continueEditing, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            if (!String.IsNullOrWhiteSpace(model.Email))
            {
                var cust2 = _customerService.GetCustomerByEmail(model.Email);
                if (cust2 != null)
                    ModelState.AddModelError("", "Email is already registered");
            }
            if (!String.IsNullOrWhiteSpace(model.Username) & _customerSettings.UsernamesEnabled)
            {
                var cust2 = _customerService.GetCustomerByUsername(model.Username);
                if (cust2 != null)
                    ModelState.AddModelError("", "Username is already registered");
            }

            //validate customer roles
            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            var newCustomerRoles = new List<CustomerRole>();
            foreach (var customerRole in allCustomerRoles)
                if (model.SelectedCustomerRoleIds != null && model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                    newCustomerRoles.Add(customerRole);

            if (newCustomerRoles.Count == 0)
                newCustomerRoles.Add(allCustomerRoles.FirstOrDefault(x => x.SystemName == SystemCustomerRoleNames.Associated));
            var customerRolesError = ValidateCustomerRoles(newCustomerRoles);
            if (!String.IsNullOrEmpty(customerRolesError))
            {
                ModelState.AddModelError("", customerRolesError);
                ErrorNotification(customerRolesError, false);
            }

            if (ModelState.IsValid)
            {
                string saltKey = _encryptionService.CreateSaltKey(5);

                var customer = new Customer
                {
                    CustomerGuid = Guid.NewGuid(),
                    Email = model.Email,
                    Username = model.AdmCode,
                    //VendorId = model.VendorId,
                    AdminComment = model.AdminComment,
                    //IsTaxExempt = model.IsTaxExempt,
                    Active = model.Active,
                    CreatedOnUtc = DateTime.UtcNow,
                    LastActivityDateUtc = DateTime.UtcNow,
                    Password = _encryptionService.CreatePasswordHash(model.Dni, saltKey, _customerSettings.HashedPasswordFormat),
                    PasswordSalt = saltKey,
                };
                _customerService.InsertCustomer(customer);

                //form fields
                if (_customerSettings.GenderEnabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Gender, model.Gender);
                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.FirstName, model.FirstName);
                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.LastName, model.LastName);
                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.AdmCode, model.AdmCode);
                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Dni, model.Dni);
                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.MilitarySituationId, model.MilitarySituationId);
                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.DeclaratoryLetter, _sequenceIdsSettings.DeclaratoryLetter);
                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.DateOfAdmission, model.DateOfAdmission);
                if (_customerSettings.DateOfBirthEnabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.DateOfBirth, model.DateOfBirth);
                if (_customerSettings.StreetAddressEnabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress, model.StreetAddress);
                if (_customerSettings.StreetAddress2Enabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress2, model.StreetAddress2);
                if (_customerSettings.CountryEnabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CountryId, model.CountryId);
                if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StateProvinceId, model.StateProvinceId);
                if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled && _customerSettings.CityEnabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CityId, model.CityId);
                if (_customerSettings.PhoneEnabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Phone, model.Phone);
                if (_customerSettings.FaxEnabled)
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Fax, model.Fax);

                var storeScope = GetActiveStoreScopeConfiguration(_ksSystemService, _workContext);
                var sequenceIdsSettings = _settingService.LoadSetting<SequenceIdsSettings>(storeScope);

                sequenceIdsSettings.DeclaratoryLetter += 1;
                _settingService.SaveSetting(sequenceIdsSettings);
                //now clear settings cache
                _settingService.ClearCache();

                //custom customer attributes
                var customerAttributes = ParseCustomCustomerAttributes(customer, form);
                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CustomCustomerAttributes, customerAttributes);

                //password
                if (!String.IsNullOrWhiteSpace(model.Password))
                {
                    var changePassRequest = new ChangePasswordRequest(model.Email, false, _customerSettings.DefaultPasswordFormat, model.Password);
                    var changePassResult = _customerRegistrationService.ChangePassword(changePassRequest);
                    if (!changePassResult.Success)
                    {
                        foreach (var changePassError in changePassResult.Errors)
                            ErrorNotification(changePassError);
                    }
                }

                //customer roles
                foreach (var customerRole in newCustomerRoles)
                {
                    //ensure that the current customer cannot add to "Administrators" system role if he's not an admin himself
                    if (customerRole.SystemName == SystemCustomerRoleNames.Administrators &&
                        !_workContext.CurrentCustomer.IsAdmin())
                        continue;

                    customer.CustomerRoles.Add(customerRole);
                }
                _customerService.UpdateCustomer(customer);

                //activity log
                _customerActivityService.InsertActivity("AddNewMilitaryPerson", _localizationService.GetResource("ActivityLog.AddNewMilitaryPerson"), customer.Id, customer.Username ?? customer.Email);

                SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = customer.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareCustomerModel(model, null, true);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(id);
            if (customer == null || customer.Deleted)
                //No customer found with the specified id
                return RedirectToAction("List");

            var model = new CustomerModel();
            PrepareCustomerModel(model, customer, false);
            PrepareSettingPaymentAmount(model);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        [ValidateInput(false)]
        public ActionResult Edit(CustomerModel model, bool continueEditing, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(model.Id);
            if (customer == null || customer.Deleted)
                //No customer found with the specified id
                return RedirectToAction("List");

            //validate customer roles
            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            var newCustomerRoles = new List<CustomerRole>();
            foreach (var customerRole in allCustomerRoles)
                if (model.SelectedCustomerRoleIds != null && model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                    newCustomerRoles.Add(customerRole);
            var customerRolesError = ValidateCustomerRoles(newCustomerRoles);
            if (!String.IsNullOrEmpty(customerRolesError))
            {
                ModelState.AddModelError("", customerRolesError);
                ErrorNotification(customerRolesError, false);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    customer.AdminComment = model.AdminComment;
                    //customer.IsTaxExempt = model.IsTaxExempt;
                    customer.Active = model.Active;
                    //email
                    if (!String.IsNullOrWhiteSpace(model.Email))
                    {
                        _customerRegistrationService.SetEmail(customer, model.Email);
                    }
                    else
                    {
                        customer.Email = model.Email;
                    }

                    //username
                    if (_customerSettings.UsernamesEnabled && _customerSettings.AllowUsersToChangeUsernames)
                    {
                        if (!String.IsNullOrWhiteSpace(model.Username))
                        {
                            _customerRegistrationService.SetUsername(customer, model.Username);
                        }
                        else
                        {
                            customer.Username = model.Username;
                        }
                    }



                    //form fields
                    if (_customerSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Gender, model.Gender);
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.FirstName, model.FirstName);
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.LastName, model.LastName);
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.AdmCode, model.AdmCode);
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Dni, model.Dni);
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.MilitarySituationId, model.MilitarySituationId);
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.DeclaratoryLetter, model.DeclaratoryLetter);
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.DateOfAdmission, model.DateOfAdmission);
                    if (_customerSettings.DateOfBirthEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.DateOfBirth, model.DateOfBirth);
                    if (_customerSettings.StreetAddressEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress, model.StreetAddress);
                    if (_customerSettings.StreetAddress2Enabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress2, model.StreetAddress2);
                    if (_customerSettings.CountryEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CountryId, model.CountryId);
                    if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StateProvinceId, model.StateProvinceId);
                    if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled && _customerSettings.CityEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CityId, model.CityId);
                    if (_customerSettings.PhoneEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Phone, model.Phone);
                    if (_customerSettings.FaxEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Fax, model.Fax);

                    //custom customer attributes
                    var customerAttributes = ParseCustomCustomerAttributes(customer, form);
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CustomCustomerAttributes, customerAttributes);

                    //customer roles
                    foreach (var customerRole in allCustomerRoles)
                    {
                        //ensure that the current customer cannot add/remove to/from "Administrators" system role
                        //if he's not an admin himself
                        if (customerRole.SystemName == SystemCustomerRoleNames.Administrators &&
                            !_workContext.CurrentCustomer.IsAdmin())
                            continue;

                        if (model.SelectedCustomerRoleIds != null &&
                            model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                        {
                            //new role
                            if (customer.CustomerRoles.Count(cr => cr.Id == customerRole.Id) == 0)
                                customer.CustomerRoles.Add(customerRole);
                        }
                        else
                        {
                            //remove role
                            if (customer.CustomerRoles.Count(cr => cr.Id == customerRole.Id) > 0)
                                customer.CustomerRoles.Remove(customerRole);
                        }
                    }
                    _customerService.UpdateCustomer(customer);
                    PrepareSettingPaymentAmount(model);

                    //activity log
                    _customerActivityService.InsertActivity("EditMilitaryPerson", _localizationService.GetResource("ActivityLog.EditMilitaryPerson"), customer.Id, customer.Username ?? customer.Email);

                    SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Updated"));
                    if (continueEditing)
                    {
                        //selected tab
                        SaveSelectedTabIndex();

                        return RedirectToAction("Edit", new { id = customer.Id });
                    }
                    return RedirectToAction("List");
                }
                catch (Exception exc)
                {
                    ErrorNotification(exc.Message, false);
                }
            }


            //If we got this far, something failed, redisplay form
            PrepareCustomerModel(model, customer, true);
            PrepareSettingPaymentAmount(model);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("changepassword")]
        public ActionResult ChangePassword(CustomerModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(model.Id);
            if (customer == null)
                //No customer found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var changePassRequest = new ChangePasswordRequest(model.Email,
                    false, _customerSettings.DefaultPasswordFormat, model.Password);
                var changePassResult = _customerRegistrationService.ChangePassword(changePassRequest);
                if (changePassResult.Success)
                    SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.PasswordChanged"));
                else
                    foreach (var error in changePassResult.Errors)
                        ErrorNotification(error);
            }

            return RedirectToAction("Edit", new { id = customer.Id });
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(id);
            if (customer == null)
                //No customer found with the specified id
                return RedirectToAction("List");

            try
            {
                var loans = _loanService.GetLoansByCustomer(customer.Id);
                if (loans != null && loans.Count > 0)
                {
                    ErrorNotification("El asociado cuenta con  " + loans.Count +
                                      " apoyo económico activos, primero cancele dichos apoyos y despues proceda a desactivar al asociado");
                }
                else
                {
                    var contributions = _contributionService.GetContributionsByCustomer(customer.Id);
                    foreach (var contribution in contributions)
                    {
                        contribution.Active = false;
                        _contributionService.UpdateContribution(contribution);
                    }

                    //no necesito hacer nada con los beneficios ya que se amarran al estado de la aportacion 

                    customer.Active = !customer.Active;
                    _customerService.UpdateCustomer(customer);
                    if (customer.Active)
                    {
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.DeclaratoryLetter,
                            _sequenceIdsSettings.DeclaratoryLetter);
                        var storeScope = GetActiveStoreScopeConfiguration(_ksSystemService, _workContext);
                        var sequenceIdsSettings = _settingService.LoadSetting<SequenceIdsSettings>(storeScope);

                        sequenceIdsSettings.DeclaratoryLetter += 1;
                        _settingService.SaveSetting(sequenceIdsSettings);
                        //now clear settings cache
                        _settingService.ClearCache();
                    }
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.MilitarySituationId, 0);
                    //_customerService.DeleteCustomer(customer);

                    //activity log
                    _customerActivityService.InsertActivity(DefaultActivityLogType.ActivityLogEditCustomer.Name, _localizationService.GetResource("ActivityLog.DeleteMilitaryPerson"), customer.Id, customer.Username ?? customer.Email);

                    SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Deleted"));
                }
                return RedirectToAction("Edit", new { id = customer.Id });
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = customer.Id });
            }
        }

        public ActionResult SendEmail(CustomerModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(model.Id);
            if (customer == null)
                //No customer found with the specified id
                return RedirectToAction("List");

            try
            {
                if (String.IsNullOrWhiteSpace(customer.Email))
                    throw new KsException("Customer email is empty");
                if (!CommonHelper.IsValidEmail(customer.Email))
                    throw new KsException("Customer email is not valid");
                if (String.IsNullOrWhiteSpace(model.SendEmail.Subject))
                    throw new KsException("Email subject is empty");
                if (String.IsNullOrWhiteSpace(model.SendEmail.Body))
                    throw new KsException("Email body is empty");

                var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
                if (emailAccount == null)
                    emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
                if (emailAccount == null)
                    throw new KsException("Email account can't be loaded");

                var email = new QueuedEmail
                {
                    Priority = QueuedEmailPriority.High,
                    EmailAccountId = emailAccount.Id,
                    FromName = emailAccount.DisplayName,
                    From = emailAccount.Email,
                    ToName = customer.GetFullName(),
                    To = customer.Email,
                    Subject = model.SendEmail.Subject,
                    Body = model.SendEmail.Body,
                    CreatedOnUtc = DateTime.UtcNow,
                };
                _queuedEmailService.InsertQueuedEmail(email);
                SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.SendEmail.Queued"));
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
            }

            return RedirectToAction("Edit", new { id = customer.Id });
        }

        #endregion

        #region Addresses

        [HttpPost]
        public ActionResult AddressesSelect(int customerId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
                throw new ArgumentException("No customer found with the specified id", "customerId");

            var addresses = customer.Addresses.OrderByDescending(a => a.CreatedOnUtc).ThenByDescending(a => a.Id).ToList();
            var gridModel = new DataSourceResult
            {
                Data = addresses.Select(x =>
                {
                    var model = x.ToModel();
                    var addressHtmlSb = new StringBuilder("<div>");
                    if (_addressSettings.StreetAddressEnabled && !String.IsNullOrEmpty(model.Address1))
                        addressHtmlSb.AppendFormat("{0}<br />", Server.HtmlEncode(model.Address1));
                    if (_addressSettings.StreetAddress2Enabled && !String.IsNullOrEmpty(model.Address2))
                        addressHtmlSb.AppendFormat("{0}<br />", Server.HtmlEncode(model.Address2));
                    if (_addressSettings.CityEnabled && !String.IsNullOrEmpty(model.City))
                        addressHtmlSb.AppendFormat("{0},", Server.HtmlEncode(model.City));
                    if (_addressSettings.StateProvinceEnabled && !String.IsNullOrEmpty(model.StateProvinceName))
                        addressHtmlSb.AppendFormat("{0},", Server.HtmlEncode(model.StateProvinceName));
                    if (_addressSettings.ZipPostalCodeEnabled && !String.IsNullOrEmpty(model.ZipPostalCode))
                        addressHtmlSb.AppendFormat("{0}<br />", Server.HtmlEncode(model.ZipPostalCode));
                    if (_addressSettings.CountryEnabled && !String.IsNullOrEmpty(model.CountryName))
                        addressHtmlSb.AppendFormat("{0}", Server.HtmlEncode(model.CountryName));
                    var customAttributesFormatted = _addressAttributeFormatter.FormatAttributes(x.CustomAttributes);
                    if (!String.IsNullOrEmpty(customAttributesFormatted))
                    {
                        //already encoded
                        addressHtmlSb.AppendFormat("<br />{0}", customAttributesFormatted);
                    }
                    addressHtmlSb.Append("</div>");
                    model.AddressHtml = addressHtmlSb.ToString();
                    return model;
                }),
                Total = addresses.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult AddressDelete(int id, int customerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
                throw new ArgumentException("No customer found with the specified id", "customerId");

            var address = customer.Addresses.FirstOrDefault(a => a.Id == id);
            if (address == null)
                //No customer found with the specified id
                return Content("No customer found with the specified id");
            customer.RemoveAddress(address);
            _customerService.UpdateCustomer(customer);
            //now delete the address record
            _addressService.DeleteAddress(address);

            return new NullJsonResult();
        }

        public ActionResult AddressCreate(int customerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
                //No customer found with the specified id
                return RedirectToAction("List");

            var model = new CustomerAddressModel();
            PrepareAddressModel(model, null, customer, false);

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddressCreate(CustomerAddressModel model, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(model.CustomerId);
            if (customer == null)
                //No customer found with the specified id
                return RedirectToAction("List");

            //custom address attributes
            var customAttributes = form.ParseCustomAddressAttributes(_addressAttributeParser, _addressAttributeService);
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                var address = model.Address.ToEntity();
                address.CustomAttributes = customAttributes;
                address.CreatedOnUtc = DateTime.UtcNow;
                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;
                customer.Addresses.Add(address);
                _customerService.UpdateCustomer(customer);

                SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Addresses.Added"));
                return RedirectToAction("AddressEdit", new { addressId = address.Id, customerId = model.CustomerId });
            }

            //If we got this far, something failed, redisplay form
            PrepareAddressModel(model, null, customer, true);
            return View(model);
        }

        public ActionResult AddressEdit(int addressId, int customerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
                //No customer found with the specified id
                return RedirectToAction("List");

            var address = _addressService.GetAddressById(addressId);
            if (address == null)
                //No address found with the specified id
                return RedirectToAction("Edit", new { id = customer.Id });

            var model = new CustomerAddressModel();
            PrepareAddressModel(model, address, customer, false);
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddressEdit(CustomerAddressModel model, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(model.CustomerId);
            if (customer == null)
                //No customer found with the specified id
                return RedirectToAction("List");

            var address = _addressService.GetAddressById(model.Address.Id);
            if (address == null)
                //No address found with the specified id
                return RedirectToAction("Edit", new { id = customer.Id });

            //custom address attributes
            var customAttributes = form.ParseCustomAddressAttributes(_addressAttributeParser, _addressAttributeService);
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                address = model.Address.ToEntity(address);
                address.CustomAttributes = customAttributes;
                _addressService.UpdateAddress(address);

                SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Addresses.Updated"));
                return RedirectToAction("AddressEdit", new { addressId = model.Address.Id, customerId = model.CustomerId });
            }

            //If we got this far, something failed, redisplay form
            PrepareAddressModel(model, address, customer, true);

            return View(model);
        }

        #endregion

        #region Contributions

        public ActionResult ContributionCreate(int customerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
                //No customer found with the specified id
                return RedirectToAction("List");


            var model = new ContributionModel
            {
                CustomerId = customer.Id,
                CustomerDni = customer.GetAttribute<string>(SystemCustomerAttributeNames.Dni),
                CustomerAdmCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.AdmCode),
                CustomerCompleteName = customer.GetFullName(),
                TotalOfCycles = _contributionSettings.TotalCycle,
                AmountMeta = (_contributionSettings.Amount1 + _contributionSettings.Amount2 + _contributionSettings.Amount3) * _contributionSettings.TotalCycle,
                CreatedOn = DateTime.UtcNow,
                AuthorizeDiscount = _sequenceIdsSettings.AuthorizeDiscount,
                DayOfPayment = _contributionSettings.DayOfPaymentContribution,
                MonthsList = DateTime.Now.GetMonthsList(_localizationService),
                YearsList = DateTime.Now.GetYearsList(_localizationService)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ContributionCreate(ContributionModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(model.CustomerId);
            if (customer == null)
                //No customer found with the specified id
                return RedirectToAction("List");

            model.MonthsList = DateTime.Now.GetMonthsList(_localizationService);
            model.YearsList = DateTime.Now.GetYearsList(_localizationService);

            if (ModelState.IsValid)
            {
                #region Only one record ant the time
                var oldContributionActive = _contributionService.GetContributionById(customerId: model.CustomerId, stateId: 1);

                if (oldContributionActive != null)
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Contract.Contribution.Fields.ExistsContribution"));
                    return View(model);
                }

                #endregion

                #region Different of time
                var estimated = new DateTime(model.YearId, model.MonthId, model.DayOfPayment);
                var today = DateTime.Now;

                if (estimated <= today)
                {
                    ErrorNotification(string.Format(_localizationService.GetResource("Admin.Contract.Contribution.Fields.DayOfPayment.AfterNow"), DateTime.Now));
                    return View(model);
                }
                #endregion

                #region Contribution
                var contribution = new Contribution
                    {
                        CustomerId = model.CustomerId,
                        AuthorizeDiscount = _sequenceIdsSettings.AuthorizeDiscount,
                        Active = true,
                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = null,
                        AmountMeta = (_contributionSettings.Amount1 + _contributionSettings.Amount2 + _contributionSettings.Amount3) * _contributionSettings.TotalCycle,
                        TotalOfCycles = _contributionSettings.TotalCycle,
                        ContributionPayments = new List<ContributionPayment>(model.TotalOfCycles)
                    };

                for (var cycle = 0; cycle < model.TotalOfCycles; cycle++)
                {
                    contribution.ContributionPayments.Add(new ContributionPayment
                    {
                        Number = cycle + 1,
                        Amount1 = _contributionSettings.Amount1,
                        Amount2 = _contributionSettings.Amount2,
                        Amount3 = _contributionSettings.Amount3,
                        AmountTotal = _contributionSettings.Amount1 + _contributionSettings.Amount2 + _contributionSettings.Amount3,
                        StateId = 1,
                        BankName = "",
                        AccountNumber = "",
                        TransactionNumber = "",
                        Reference = "",
                        Description = "",
                        IsAutomatic = true,
                        ScheduledDateOnUtc = _dateTimeHelper.ConvertToUtcTime(estimated.AddMonths(cycle)),
                        ProcessedDateOnUtc = null
                    });
                }

                _contributionService.InsertContribution(contribution);

                #endregion

                var storeScope = GetActiveStoreScopeConfiguration(_ksSystemService, _workContext);
                var sequenceIdsSettings = _settingService.LoadSetting<SequenceIdsSettings>(storeScope);

                sequenceIdsSettings.AuthorizeDiscount += 1;
                _settingService.SaveSetting(sequenceIdsSettings);
                //now clear settings cache
                _settingService.ClearCache();

                //activity log
                _customerActivityService.InsertActivity("AddContribution", string.Format(_localizationService.GetResource("ActivityLog.AddContribution"), model.CustomerCompleteName, model.CustomerId));
                SuccessNotification(_localizationService.GetResource("Admin.Contract.Contribution.Updated"));

                SaveSelectedTabIndex(4);

                return RedirectToAction("Edit", new { id = model.CustomerId });

            }

            return View(model);
        }

        #endregion

        #region Loan

        public ActionResult LoanCreate(int customerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
                //No customer found with the specified id
                return RedirectToAction("List");

            var periods = CommonHelper.ConvertToSelectListItem(_loanSettings.Periods, ',');
            periods.Insert(0, new SelectListItem { Value = "0", Text = "------------------" });
            var totalOfContribution = _contributionService.GetAllPayments(customerId);
            var totalOfCyclesPayments = totalOfContribution.Count(x => x.StateId == (int)ContributionState.Pagado);
            var model = (LoanModel)Session["loanModel"];
            if (model == null || !model.IsPostBack)
            {
                var result = new LoanModel
                {
                    CustomerId = customer.Id,
                    Periods = periods,
                    TotalOfCycle = totalOfCyclesPayments
                };

                return View(result);
            }
            model.Periods = periods;
            model.TotalOfCycle = totalOfCyclesPayments;
            Session["loanModel"] = null;
            return View(model);

        }

        [HttpPost, ActionName("LoanCreate")]
        [FormValueRequired("EvaluatePopup")]
        public ActionResult EvaluatePopup(LoanModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                var periods = CommonHelper.ConvertToSelectListItem(_loanSettings.Periods, ',');
                periods.Insert(0, new SelectListItem { Value = "0", Text = "----" });
                model.Periods = periods;
                return View(model);
            }

            model = PrepareLoanModel(model);

            if (model.CashFlowModels == null)
            {
                ErrorNotification(_localizationService.GetResource("Admin.Contract.Loan.CashFlow.Error"));
                model.IsPostBack = false;
            }
            if (!(model.CashFlowModels != null && model.CashFlowModels.To >= model.LoanAmount))
            {
                ErrorNotification(_localizationService.GetResource("Admin.Contract.Loan.CashFlow.Amount.Error"));
                model.IsPostBack = false;
            }
            if (model.StateActivityModels == null)
            {
                ErrorNotification(string.Format(_localizationService.GetResource("Admin.Contract.Loan.StateActivity.Error"), model.LoanAmount.ToString("c", new CultureInfo("es-PE"))));
                model.IsPostBack = false;
            }

            if (model.StateActivityModels != null && model.StateActivityModels.HasWarranty && model.CustomerAdmCode == null)
            {
                ErrorNotification(_localizationService.GetResource("Admin.Contract.Loan.StateActivity.Warranty.Error"));
                model.IsPostBack = false;
            }
            Session["loanModel"] = model;
            return RedirectToAction("LoanCreate", new { customerId = model.CustomerId });
        }


        [HttpPost, ActionName("LoanCreate")]
        [FormValueRequired("LoanCreate")]
        public ActionResult LoanCreate(LoanModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            if (model.IsPostBack)
            {
                //No Errors
                model = PrepareLoanModel(model);
                var loan = new Loan
                {
                    CustomerId = model.CustomerId,
                    WarrantyId = model.StateActivityModels.CustomerWarranty != null ? model.StateActivityModels.CustomerWarranty.CustomerId : 0,
                    LoanNumber = _sequenceIdsSettings.AuthorizeLoan,
                    Period = model.Period,
                    Tea = model.PreCashFlow.Tea,
                    Safe = model.PreCashFlow.Safe,
                    LoanAmount = model.LoanAmount,
                    MonthlyQuota = model.PreCashFlow.MonthlyQuota,
                    TotalFeed = model.PreCashFlow.TotalFeed,
                    TotalSafe = model.PreCashFlow.TotalSafe,
                    TotalAmount = model.PreCashFlow.TotalAmount,
                    TotalToPay = model.PreCashFlow.TotalToPay,
                    IsAuthorized = false,
                    IsDelay = false,
                    Active = true,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = null
                };

                var estimated = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, _loanSettings.DayOfPaymentLoan);

                for (var cycle = 1; cycle <= model.Period; cycle++)
                {
                    loan.LoanPayments.Add(new LoanPayment
                    {
                        //Active = false,
                        Quota = cycle,
                        MonthlyFee = (loan.TotalFeed / loan.Period),
                        MonthlyCapital = (loan.MonthlyQuota - loan.TotalFeed / loan.Period),
                        MonthlyQuota = (loan.MonthlyQuota),
                        ScheduledDateOnUtc = (estimated.AddMonths(cycle)),
                        ProcessedDateOnUtc = null,
                        StateId = 1,
                        IsAutomatic = true
                    });
                }
                _loanService.InsertLoan(loan);

                var storeScope = GetActiveStoreScopeConfiguration(_ksSystemService, _workContext);
                var sequenceIdsSettings = _settingService.LoadSetting<SequenceIdsSettings>(storeScope);

                sequenceIdsSettings.AuthorizeLoan += 1;
                _settingService.SaveSetting(sequenceIdsSettings);
                //now clear settings cache
                _settingService.ClearCache();
                SaveSelectedTabIndex(5);

                #region Flow - Approval required

                _workFlowService.InsertWorkFlow(new WorkFlow
                {
                    CustomerCreatedId = _workContext.CurrentCustomer.Id,
                    EntityId = loan.LoanNumber,
                    EntityName = CommonHelper.GetKsCustomTypeConverter(typeof(Loan)).ConvertToInvariantString(loan),
                    RequireCustomer = false,
                    RequireSystemRole = true,
                    SystemRoleApproval = SystemCustomerRoleNames.Manager,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    Active = true,
                    Title = "Aprobar Apoyo Social Económico",
                    Description = "Se ha realizado la aprobacion del Apoyo Social Economico" +
                    " para el asociado " + _customerService.GetCustomerById(loan.CustomerId).GetFullName(),
                    GoTo = "Admin/Loans/Approval/" + loan.Id
                });
                #endregion

                _customerActivityService.InsertActivity("AddNewLoan", _localizationService.GetResource("ActivityLog.AddNewLoan"), loan.LoanNumber, loan.Id);
            }

            return RedirectToAction("Edit", new { id = model.CustomerId });
        }

        [HttpPost]
        public ActionResult ListLoans(DataSourceRequest command, int customerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return Content("");

            //var loanPayments = _loanService.GetAllPayments(customerId: customerId, pageIndex: command.Page - 1, pageSize: command.PageSize);
            var loanPayments = _loanService.GetAllPayments(pageIndex: command.Page - 1, pageSize: command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = loanPayments.Select(x =>
                {
                    var m = new LoanPaymentsModel
                    {
                        Quota = x.Quota,
                        MonthlyQuota = x.MonthlyQuota,
                        MonthlyFee = x.MonthlyFee,
                        MonthlyCapital = x.MonthlyCapital,
                        ScheduledDateOn = _dateTimeHelper.ConvertToUserTime(x.ScheduledDateOnUtc, DateTimeKind.Local),
                        ProcessedDateOn = x.ProcessedDateOnUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(x.ProcessedDateOnUtc.Value, DateTimeKind.Local) : x.ProcessedDateOnUtc,
                        //State = x.Active ? "Completado" : "Pendiente"
                    };
                    return m;

                }),
                Total = loanPayments.TotalCount
            };

            return Json(gridModel);
        }

        protected virtual CashFlowModel PrepareCashFlow(decimal cashFlow)
        {
            var avalibleCashFlow = XmlHelper.XmlToObject<List<CashFlowModel>>(_loanSettings.CashFlow);
            for (int i = 0; i < avalibleCashFlow.Count; i++)
            {
                if (i + 1 != avalibleCashFlow.Count)
                    if (avalibleCashFlow[i].Amount <= cashFlow && avalibleCashFlow[i + 1].Amount >= cashFlow)
                        return avalibleCashFlow[i];
            }
            return null;
        }

        protected virtual StateActivityModel PrepareClassState(decimal amount, int cycle, Customer warranty, string admCode)
        {
            var classState = new StateActivityModel();

            #region Class A
            if (_loanSettings.IsEnable1 &&
                _loanSettings.MinClycle1 * 12 <= cycle &&
                _loanSettings.MaxClycle1 * 12 >= cycle)
            {
                if (_loanSettings.HasOnlySignature1 &&
                    _loanSettings.MinAmountWithSignature1 <= amount &&
                    _loanSettings.MaxAmountWithSignature1 >= amount)
                {
                    classState.StateName = _loanSettings.StateName1;
                    classState.IsEnable = _loanSettings.IsEnable1;
                    classState.MinClycle = _loanSettings.MinClycle1;
                    classState.MaxClycle = _loanSettings.MaxClycle1;

                    classState.HasOnlySignature = _loanSettings.HasOnlySignature1;
                    classState.MinAmountWithSignature = _loanSettings.MinAmountWithSignature1;
                    classState.MaxAmountWithSignature = _loanSettings.MaxAmountWithSignature1;
                    return classState;
                }

                if (_loanSettings.HasWarranty1 &&
                    _loanSettings.MinAmountWithWarranty1 <= amount &&
                    _loanSettings.MaxAmountWithWarranty1 >= amount)
                {
                    classState.StateName = _loanSettings.StateName1;
                    classState.IsEnable = _loanSettings.IsEnable1;
                    classState.MinClycle = _loanSettings.MinClycle1;
                    classState.MaxClycle = _loanSettings.MaxClycle1;

                    classState.HasWarranty = _loanSettings.HasWarranty1;
                    classState.MinAmountWithWarranty = _loanSettings.MinAmountWithWarranty1;
                    classState.MaxAmountWithWarranty = _loanSettings.MaxAmountWithWarranty1;
                    classState.CustomerWarranty = new CustomerWarranty
                    {
                        CustomerId = warranty.Id,
                        CompleteName = warranty.GetFullName(),
                        AdminCode = admCode,
                        IsActive = warranty.Active,
                        IsContributor =
                            _contributionService.GetContributionsByCustomer(customerId: warranty.Id).Count > 0
                    };
                    return classState;
                }
            }

            #endregion

            #region Class B
            if (_loanSettings.IsEnable2 &&
                _loanSettings.MinClycle2 * 12 <= cycle &&
                _loanSettings.MaxClycle2 * 12 >= cycle)
            {
                if (_loanSettings.HasOnlySignature2 &&
                    _loanSettings.MinAmountWithSignature2 <= amount &&
                    _loanSettings.MaxAmountWithSignature2 >= amount)
                {
                    classState.StateName = _loanSettings.StateName2;
                    classState.IsEnable = _loanSettings.IsEnable2;
                    classState.MinClycle = _loanSettings.MinClycle2;
                    classState.MaxClycle = _loanSettings.MaxClycle2;

                    classState.HasOnlySignature = _loanSettings.HasOnlySignature2;
                    classState.MinAmountWithSignature = _loanSettings.MinAmountWithSignature2;
                    classState.MaxAmountWithSignature = _loanSettings.MaxAmountWithSignature2;
                    return classState;
                }

                if (_loanSettings.HasWarranty2 &&
                    _loanSettings.MinAmountWithWarranty2 <= amount &&
                    _loanSettings.MaxAmountWithWarranty2 >= amount)
                {
                    classState.StateName = _loanSettings.StateName2;
                    classState.IsEnable = _loanSettings.IsEnable2;
                    classState.MinClycle = _loanSettings.MinClycle2;
                    classState.MaxClycle = _loanSettings.MaxClycle2;

                    classState.HasWarranty = _loanSettings.HasWarranty2;
                    classState.MinAmountWithWarranty = _loanSettings.MinAmountWithWarranty2;
                    classState.MaxAmountWithWarranty = _loanSettings.MaxAmountWithWarranty2;
                    classState.CustomerWarranty = new CustomerWarranty
                    {
                        CustomerId = warranty.Id,
                        CompleteName = warranty.GetFullName(),
                        AdminCode = admCode,
                        IsActive = warranty.Active,
                        IsContributor =
                            _contributionService.GetContributionsByCustomer(customerId: warranty.Id).Count > 0
                    };
                    return classState;
                }
            }

            #endregion

            #region Class C
            if (_loanSettings.IsEnable3 &&
                _loanSettings.MinClycle3 * 12 <= cycle &&
                _loanSettings.MaxClycle3 * 12 >= cycle)
            {
                if (_loanSettings.HasOnlySignature3 &&
                    _loanSettings.MinAmountWithSignature3 <= amount &&
                    _loanSettings.MaxAmountWithSignature3 >= amount)
                {
                    classState.StateName = _loanSettings.StateName3;
                    classState.IsEnable = _loanSettings.IsEnable3;
                    classState.MinClycle = _loanSettings.MinClycle3;
                    classState.MaxClycle = _loanSettings.MaxClycle3;

                    classState.HasOnlySignature = _loanSettings.HasOnlySignature3;
                    classState.MinAmountWithSignature = _loanSettings.MinAmountWithSignature3;
                    classState.MaxAmountWithSignature = _loanSettings.MaxAmountWithSignature3;
                    return classState;
                }

                if (_loanSettings.HasWarranty3 &&
                    _loanSettings.MinAmountWithWarranty3 <= amount &&
                    _loanSettings.MaxAmountWithWarranty3 >= amount)
                {
                    classState.StateName = _loanSettings.StateName3;
                    classState.IsEnable = _loanSettings.IsEnable3;
                    classState.MinClycle = _loanSettings.MinClycle3;
                    classState.MaxClycle = _loanSettings.MaxClycle3;

                    classState.HasWarranty = _loanSettings.HasWarranty3;
                    classState.MinAmountWithWarranty = _loanSettings.MinAmountWithWarranty3;
                    classState.MaxAmountWithWarranty = _loanSettings.MaxAmountWithWarranty3;
                    classState.CustomerWarranty = new CustomerWarranty
                    {
                        CustomerId = warranty.Id,
                        CompleteName = warranty.GetFullName(),
                        AdminCode = admCode,
                        IsActive = warranty.Active,
                        IsContributor =
                            _contributionService.GetContributionsByCustomer(customerId: warranty.Id).Count > 0
                    };
                    return classState;
                }
            }
            #endregion

            #region Class D
            if (_loanSettings.IsEnable4 &&
                _loanSettings.MinClycle4 * 12 <= cycle &&
                _loanSettings.MaxClycle4 * 12 >= cycle)
            {
                if (_loanSettings.HasOnlySignature4 &&
                    _loanSettings.MinAmountWithSignature4 <= amount &&
                    _loanSettings.MaxAmountWithSignature4 >= amount)
                {
                    classState.StateName = _loanSettings.StateName4;
                    classState.IsEnable = _loanSettings.IsEnable4;
                    classState.MinClycle = _loanSettings.MinClycle4;
                    classState.MaxClycle = _loanSettings.MaxClycle4;

                    classState.HasOnlySignature = _loanSettings.HasOnlySignature4;
                    classState.MinAmountWithSignature = _loanSettings.MinAmountWithSignature4;
                    classState.MaxAmountWithSignature = _loanSettings.MaxAmountWithSignature4;
                    return classState;
                }

                if (_loanSettings.HasWarranty4 &&
                    _loanSettings.MinAmountWithWarranty4 <= amount &&
                    _loanSettings.MaxAmountWithWarranty4 >= amount)
                {
                    classState.StateName = _loanSettings.StateName4;
                    classState.IsEnable = _loanSettings.IsEnable4;
                    classState.MinClycle = _loanSettings.MinClycle4;
                    classState.MaxClycle = _loanSettings.MaxClycle4;

                    classState.HasWarranty = _loanSettings.HasWarranty4;
                    classState.MinAmountWithWarranty = _loanSettings.MinAmountWithWarranty4;
                    classState.MaxAmountWithWarranty = _loanSettings.MaxAmountWithWarranty4;
                    classState.CustomerWarranty = new CustomerWarranty
                    {
                        CustomerId = warranty.Id,
                        CompleteName = warranty.GetFullName(),
                        AdminCode = admCode,
                        IsActive = warranty.Active,
                        IsContributor =
                            _contributionService.GetContributionsByCustomer(customerId: warranty.Id).Count > 0
                    };
                    return classState;
                }
            }
            #endregion

            #region Class E
            if (_loanSettings.IsEnable5 &&
                _loanSettings.MinClycle5 * 12 <= cycle &&
                _loanSettings.MaxClycle5 * 12 >= cycle)
            {
                if (_loanSettings.HasOnlySignature5 &&
                    _loanSettings.MinAmountWithSignature5 <= amount &&
                    _loanSettings.MaxAmountWithSignature5 >= amount)
                {
                    classState.StateName = _loanSettings.StateName5;
                    classState.IsEnable = _loanSettings.IsEnable5;
                    classState.MinClycle = _loanSettings.MinClycle5;
                    classState.MaxClycle = _loanSettings.MaxClycle5;

                    classState.HasOnlySignature = _loanSettings.HasOnlySignature5;
                    classState.MinAmountWithSignature = _loanSettings.MinAmountWithSignature5;
                    classState.MaxAmountWithSignature = _loanSettings.MaxAmountWithSignature5;
                    return classState;
                }

                if (_loanSettings.HasWarranty5 &&
                    _loanSettings.MinAmountWithWarranty5 <= amount &&
                    _loanSettings.MaxAmountWithWarranty5 >= amount)
                {
                    classState.StateName = _loanSettings.StateName5;
                    classState.IsEnable = _loanSettings.IsEnable5;
                    classState.MinClycle = _loanSettings.MinClycle5;
                    classState.MaxClycle = _loanSettings.MaxClycle5;

                    classState.HasWarranty = _loanSettings.HasWarranty5;
                    classState.MinAmountWithWarranty = _loanSettings.MinAmountWithWarranty5;
                    classState.MaxAmountWithWarranty = _loanSettings.MaxAmountWithWarranty5;
                    classState.CustomerWarranty = new CustomerWarranty
                    {
                        CustomerId = warranty.Id,
                        CompleteName = warranty.GetFullName(),
                        AdminCode = admCode,
                        IsActive = warranty.Active,
                        IsContributor =
                            _contributionService.GetContributionsByCustomer(customerId: warranty.Id).Count > 0
                    };
                    return classState;
                }
            }
            #endregion

            return null;
        }

        protected virtual LoanModel PrepareLoanModel(LoanModel model)
        {
            model.IsPostBack = true;
            var customer = new Customer();
            if (!string.IsNullOrWhiteSpace(model.CustomerAdmCode))
            {
                var entity = _genericAttributeService.GetAttributeForKeyValue("AdmCode", model.CustomerAdmCode);
                customer = _customerService.GetCustomerById(entity.EntityId);
            }

            //var totalOfContribution = _contributionService.GetAllPayments(customerId: model.CustomerId);
            var totalOfContribution = _contributionService.GetAllPayments();
            var totalOfCyclesPayments = totalOfContribution.Count(x => x.StateId == (int)ContributionState.Pagado);
            //1 ) Get ABCDE 
            model.StateActivityModels = PrepareClassState(model.LoanAmount, totalOfCyclesPayments, customer, model.CustomerAdmCode);
            //2) Get CashFlow
            model.CashFlowModels = PrepareCashFlow(model.CashFlow);
            //3) Calcule PreCashFlow
            var totalfeed = model.LoanAmount * Convert.ToDecimal(model.Period * _loanSettings.Tea / 12);
            var totalSafe = model.LoanAmount * Convert.ToDecimal(_loanSettings.Safe);
            model.PreCashFlow = new PreCashFlowModel
            {
                Period = model.Period,
                Amount = model.LoanAmount,
                Tea = _loanSettings.Tea * 100,
                Safe = _loanSettings.Safe * 100,
                TotalFeed = (totalfeed),
                TotalSafe = (totalSafe),
                MonthlyQuota = ((model.LoanAmount + totalfeed) / model.Period),
                TotalAmount = (totalfeed + model.LoanAmount),
                TotalToPay = (model.LoanAmount - totalSafe),
                StateName = model.IsAuthorized ? "Aprobado" : "No Aprobado"
            };

            return model;
        }

        #endregion

        #region Activity log

        [HttpPost]
        public ActionResult ListActivityLog(DataSourceRequest command, int customerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return Content("");

            var activityLog = _customerActivityService.GetAllActivities(null, null, customerId, 0, command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = activityLog.Select(x =>
                {
                    var m = new CustomerModel.ActivityLogModel
                    {
                        Id = x.Id,
                        ActivityLogTypeName = x.ActivityLogType.Name,
                        Comment = x.Comment,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc)
                    };
                    return m;

                }),
                Total = activityLog.TotalCount
            };

            return Json(gridModel);
        }

        #endregion

    }
}