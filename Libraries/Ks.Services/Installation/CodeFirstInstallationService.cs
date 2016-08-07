using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ks.Core;
using Ks.Core.Data;
using Ks.Core.Domain.Catalog;
using Ks.Core.Domain.Common;
using Ks.Core.Domain.Customers;
using Ks.Core.Domain.Directory;
using Ks.Core.Domain.Localization;
using Ks.Core.Domain.Logging;
using Ks.Core.Domain.Messages;
using Ks.Core.Domain.Seo;
using Ks.Core.Domain.System;
using Ks.Core.Domain.Tasks;
using Ks.Core.Infrastructure;
using Ks.Services.Common;
using Ks.Services.Configuration;
using Ks.Services.Customers;
using Ks.Services.Localization;

namespace Ks.Services.Installation
{
    public class CodeFirstInstallationService : IInstallationService
    {
        #region Fields

        private readonly IRepository<KsSystem> _ksSystemRepository;
        private readonly IRepository<MeasureDimension> _measureDimensionRepository;
        private readonly IRepository<MeasureWeight> _measureWeightRepository;
        private readonly IRepository<Language> _languageRepository;
        private readonly IRepository<Currency> _currencyRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerRole> _customerRoleRepository;
        private readonly IRepository<SpecificationAttribute> _specificationAttributeRepository;
        private readonly IRepository<EmailAccount> _emailAccountRepository;
        private readonly IRepository<MessageTemplate> _messageTemplateRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<StateProvince> _stateProvinceRepository;
        private readonly IRepository<City> _cityRepository;
        private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
        private readonly IRepository<ScheduleTask> _scheduleTaskRepository;
        private readonly IRepository<Address> _addressRepository;
        
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public CodeFirstInstallationService(IRepository<KsSystem> ksSystemRepository, 
            IRepository<MeasureDimension> measureDimensionRepository, IRepository<MeasureWeight> measureWeightRepository,
            IRepository<Language> languageRepository, IRepository<Currency> currencyRepository, 
            IRepository<Customer> customerRepository, IRepository<CustomerRole> customerRoleRepository, 
            IRepository<SpecificationAttribute> specificationAttributeRepository, 
            IRepository<EmailAccount> emailAccountRepository, 
            IRepository<MessageTemplate> messageTemplateRepository, 
            IRepository<Country> countryRepository, IRepository<StateProvince> stateProvinceRepository, 
            IRepository<City> cityRepository, IRepository<ActivityLogType> activityLogTypeRepository,
            IRepository<ScheduleTask> scheduleTaskRepository, IRepository<Address> addressRepository, 
            IGenericAttributeService genericAttributeService, IWebHelper webHelper)
        {
            _ksSystemRepository = ksSystemRepository;
            _measureDimensionRepository = measureDimensionRepository;
            _measureWeightRepository = measureWeightRepository;
            _languageRepository = languageRepository;
            _currencyRepository = currencyRepository;
            _customerRepository = customerRepository;
            _customerRoleRepository = customerRoleRepository;
            _specificationAttributeRepository = specificationAttributeRepository;
            _emailAccountRepository = emailAccountRepository;
            _messageTemplateRepository = messageTemplateRepository;
            _countryRepository = countryRepository;
            _stateProvinceRepository = stateProvinceRepository;
            _cityRepository = cityRepository;
            _activityLogTypeRepository = activityLogTypeRepository;
            _scheduleTaskRepository = scheduleTaskRepository;
            _addressRepository = addressRepository;
            _genericAttributeService = genericAttributeService;
            _webHelper = webHelper;
        }

        #endregion

        #region Utilities

        protected virtual void InstallSystem()
        {
            var storeUrl = _webHelper.GetStoreLocation(false);
            var ksSystems = new List<KsSystem>
            {
                new KsSystem
                {
                    Name = "ACMR - Auxilio Cooperativo Militar en Retiro",
                    Url = storeUrl,
                    SslEnabled = false,
                    Hosts = storeUrl.Replace("www.","").Replace(".com","").Replace("http://","").Replace("https://","").Replace("/",""),
                    CompanyName = "Auxilio Cooperativo Militar en Retiro",
                    CompanyAddress = "Av ......",
                    CompanyPhoneNumber = "(511) 426 - 1212"
                },
            };
            _ksSystemRepository.Insert(ksSystems);
        }

        protected virtual void InstallMeasures()
        {
            var measureDimensions = new List<MeasureDimension>
            {
                new MeasureDimension{Name = "metro(s)",SystemKeyword = "metros",Ratio = 1M,DisplayOrder = 1,},
                new MeasureDimension{Name = "pies",SystemKeyword = "pies",Ratio = 0.3048000M,DisplayOrder = 2,},
                new MeasureDimension{Name = "centimetro(s)",SystemKeyword = "centimetros",Ratio = 0.01M,DisplayOrder = 3,},
                new MeasureDimension{Name = "millimetro(s)",SystemKeyword = "millimetros",Ratio = 0.001M,DisplayOrder = 4,}
            };

            _measureDimensionRepository.Insert(measureDimensions);

            var measureWeights = new List<MeasureWeight>
            {
                new MeasureWeight{Name = "miligramos(s)",SystemKeyword = "miligramos",Ratio = 0.001M,DisplayOrder = 1,},
                new MeasureWeight{Name = "libra(s)",SystemKeyword = "libras",Ratio = 0.4535970244035199M,DisplayOrder = 2,},
                new MeasureWeight{Name = "kg(s)",SystemKeyword = "kg",Ratio = 1M,DisplayOrder = 3,},
                new MeasureWeight{Name = "gramo(s)",SystemKeyword = "gramos",Ratio = 0.01M,DisplayOrder = 4,}
            };

            _measureWeightRepository.Insert(measureWeights);
        }

        protected virtual void InstallCurrencies()
        {
            var currencies = new List<Currency>
            {
                new Currency
                {
                    Name = "US Dolar",
                    CurrencyCode = "USD",
                    Rate = 1,
                    DisplayLocale = "en-US",
                    CustomFormatting = string.Format("{0}0.00", "\u20ac"),
                    Published = false,
                    DisplayOrder = 2,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                },
                new Currency
                {
                    Name = "Sol",
                    CurrencyCode = "PEN",
                    Rate = 1,
                    DisplayLocale = "es-PE",
                    CustomFormatting = string.Format("{0}0.00", "\u20ac"),
                    Published = true,
                    DisplayOrder = 1,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                }
            };

            _currencyRepository.Insert(currencies);
        }

        protected virtual void InstallLanguages()
        {
            var language = new Language
            {
                Name = "Español",
                LanguageCulture = "es-PE",
                UniqueSeoCode = "es",
                FlagImageFileName = "pe.png",
                Published = true,
                DisplayOrder = 1
            };
            _languageRepository.Insert(language);
        }

        protected virtual void InstallLocaleResources()
        {
            //'English' language
            var language = _languageRepository.Table.Single(l => l.Name == "Español");

            //save resources
            foreach (var filePath in System.IO.Directory.EnumerateFiles(_webHelper.MapPath("~/App_Data/Localization/"), "*.ksres.xml", SearchOption.TopDirectoryOnly))
            {
                var localesXml = File.ReadAllText(filePath);
                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                localizationService.ImportResourcesFromXml(language, localesXml);
            }

        }

        protected virtual void InstallCountriesAndStatesAndCities()
        {
            var cPer = new Country
            {
                Name = "Perú",
                TwoLetterIsoCode = "PE",
                ThreeLetterIsoCode = "PER",
                NumericIsoCode = 604,
                DisplayOrder = 1,
                Published = true,
            };

            cPer.StateProvinces.Add(new StateProvince { Name = "Amazonas", Abbreviation = "AMA", Published = true, DisplayOrder = 1 });
            cPer.StateProvinces.Add(new StateProvince { Name = "Ancash", Abbreviation = "ANC", Published = true, DisplayOrder = 2 });
            cPer.StateProvinces.Add(new StateProvince { Name = "Apurímac", Abbreviation = "APU", Published = true, DisplayOrder = 3 });
            cPer.StateProvinces.Add(new StateProvince { Name = "Arequipa", Abbreviation = "ARE", Published = true, DisplayOrder = 4 });
            cPer.StateProvinces.Add(new StateProvince { Name = "Ayacucho", Abbreviation = "AYA", Published = true, DisplayOrder = 5 });
            cPer.StateProvinces.Add(new StateProvince { Name = "Cajamarca", Abbreviation = "CAJ", Published = true, DisplayOrder = 6 });
            cPer.StateProvinces.Add(new StateProvince { Name = "Callao", Abbreviation = "CAL", Published = true, DisplayOrder = 7 });
            cPer.StateProvinces.Add(new StateProvince { Name = "Cusco", Abbreviation = "CUS", Published = true, DisplayOrder = 8 });
            cPer.StateProvinces.Add(new StateProvince { Name = "Huancavelica", Abbreviation = "HUV", Published = true, DisplayOrder = 9 });
            cPer.StateProvinces.Add(new StateProvince { Name = "Huánuco", Abbreviation = "HUC", Published = true, DisplayOrder = 10 });
            cPer.StateProvinces.Add(new StateProvince { Name = "Ica", Abbreviation = "ICA", Published = true, DisplayOrder = 11 });
            cPer.StateProvinces.Add(new StateProvince { Name = "Junín", Abbreviation = "JUN", Published = true, DisplayOrder = 12 });
            cPer.StateProvinces.Add(new StateProvince { Name = "La Libertad", Abbreviation = "LAL", Published = true, DisplayOrder = 13 });
            cPer.StateProvinces.Add(new StateProvince { Name = "Lambayeque", Abbreviation = "LAM", Published = true, DisplayOrder = 14 });
            cPer.StateProvinces.Add(new StateProvince { Name = "Loreto", Abbreviation = "LOR", Published = true, DisplayOrder = 16 });
            cPer.StateProvinces.Add(new StateProvince { Name = "Madre de Dios", Abbreviation = "MDD", Published = true, DisplayOrder = 17 });
            cPer.StateProvinces.Add(new StateProvince { Name = "Moquegua", Abbreviation = "MOQ", Published = true, DisplayOrder = 18 });
            cPer.StateProvinces.Add(new StateProvince { Name = "Pasco", Abbreviation = "PAS", Published = true, DisplayOrder = 19 });
            cPer.StateProvinces.Add(new StateProvince { Name = "Piura", Abbreviation = "PIU", Published = true, DisplayOrder = 20 });
            cPer.StateProvinces.Add(new StateProvince { Name = "Puno", Abbreviation = "PUN", Published = true, DisplayOrder = 21 });
            cPer.StateProvinces.Add(new StateProvince { Name = "San Martín", Abbreviation = "SAM", Published = true, DisplayOrder = 22 });
            cPer.StateProvinces.Add(new StateProvince { Name = "Tacna", Abbreviation = "TAC", Published = true, DisplayOrder = 23 });
            cPer.StateProvinces.Add(new StateProvince { Name = "Tumbes", Abbreviation = "TUM", Published = true, DisplayOrder = 24 });
            cPer.StateProvinces.Add(new StateProvince { Name = "Ucayali", Abbreviation = "UCA", Published = true, DisplayOrder = 25 });
            cPer.StateProvinces.Add(new StateProvince
            {
                Name = "Lima",Abbreviation = "LIM",Published = true,DisplayOrder = 15,Cities = new List<City>
                    {
                        new City{DisplayOrder=1,Name="Cercado de Lima"},
                        new City{DisplayOrder=2,Name="Ancón"},
                        new City{DisplayOrder=3,Name="Ate"},
                        new City{DisplayOrder=4,Name="Barranco"},
                        new City{DisplayOrder=5,Name="Breña"},
                        new City{DisplayOrder=6,Name="Carabayllo"},
                        new City{DisplayOrder=7,Name="Chaclacayo"},
                        new City{DisplayOrder=8,Name="Chorrillos"},
                        new City{DisplayOrder=9,Name="Cieneguilla"},
                        new City{DisplayOrder=10,Name="Comas"},
                        new City{DisplayOrder=11,Name="El Agustino"},
                        new City{DisplayOrder=12,Name="Independencia"},
                        new City{DisplayOrder=13,Name="Jesús María"},
                        new City{DisplayOrder=14,Name="La Molina"},
                        new City{DisplayOrder=15,Name="La Victoria"},
                        new City{DisplayOrder=16,Name="Lince"},
                        new City{DisplayOrder=17,Name="Los Olivos"},
                        new City{DisplayOrder=18,Name="Lurin"},
                        new City{DisplayOrder=19,Name="Magdalena del Mar"},
                        new City{DisplayOrder=20,Name="Miraflores"},
                        new City{DisplayOrder=21,Name="Pueblo Libre"},
                        new City{DisplayOrder=22,Name="Pachacámac"},
                        new City{DisplayOrder=23,Name="Pucusana"},
                        new City{DisplayOrder=24,Name="Puente Piedra"},
                        new City{DisplayOrder=25,Name="Punta Hermosa"},
                        new City{DisplayOrder=26,Name="Punta Negra"},
                        new City{DisplayOrder=27,Name="Rímac"},
                        new City{DisplayOrder=28,Name="San Bartolo"},
                        new City{DisplayOrder=29,Name="San Borja"},
                        new City{DisplayOrder=30,Name="San Isidro"},
                        new City{DisplayOrder=31,Name="San Juan de Lurigancho"},
                        new City{DisplayOrder=32,Name="San Juan de Miraflores"},
                        new City{DisplayOrder=33,Name="San Luis"},
                        new City{DisplayOrder=34,Name="San Martín de Porres"},
                        new City{DisplayOrder=35,Name="San Miguel"},
                        new City{DisplayOrder=36,Name="Santa Anita"},
                        new City{DisplayOrder=37,Name="Santa María del Mar"},
                        new City{DisplayOrder=38,Name="Santa Rosa"},
                        new City{DisplayOrder=39,Name="Santiago de Surco"},
                        new City{DisplayOrder=40,Name="Surquillo"},
                        new City{DisplayOrder=41,Name="Villa ElSalvador"},
                        new City{DisplayOrder=42,Name="Villa María del Triunfo"}}
                    });

            var countries = new List<Country>{cPer};
            _countryRepository.Insert(countries);
        }

        protected virtual void InstallCustomersAndUsers(string defaultUserEmail, string defaultUserPassword)
        {
            var crAdministrators = new CustomerRole
            {
                Name = "Administrador",Active = true,IsSystemRole = true,
                SystemName = SystemCustomerRoleNames.Administrators,
            };

            var crRegistered = new CustomerRole
            {
                Name = "Registrado",Active = true,IsSystemRole = false,
                SystemName = SystemCustomerRoleNames.Registered
            };

            var crAuxAccountant = new CustomerRole
            {
                Name = "Auxiliar Contable",Active = true,IsSystemRole = false,
                SystemName = SystemCustomerRoleNames.Registered,
            };

            var crSecretary = new CustomerRole
            {
                Name = "Secretaria",Active = true,IsSystemRole = false,
                SystemName = SystemCustomerRoleNames.Registered
            };
            var crManager = new CustomerRole
            {
                Name = "Gerente",Active = true,IsSystemRole = false,
                SystemName = SystemCustomerRoleNames.Registered
            };
            var crGuests = new CustomerRole
            {
                Name = "Invitado",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemCustomerRoleNames.Guests,
            };

            var customerRoles = new List<CustomerRole>
                                {
                                    crAdministrators,
                                    crAuxAccountant,
                                    crSecretary,
                                    crManager 
                                };
            _customerRoleRepository.Insert(customerRoles);

            //admin user
            var adminUser = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = defaultUserEmail,
                Username = defaultUserEmail,
                Password = defaultUserPassword,
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = "",
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
            };

            var defaultAdminUserAddress = new Address
            {
                FirstName = "Pedro",
                LastName = "Curich Gonzales",
                PhoneNumber = "973 905 013",
                Email = "pcurich@kreiersolutions.com",
                FaxNumber = "",
                Company = "Kreier Solutions",
                Address1 = "",
                Address2 = "",
                City = _cityRepository.Table.FirstOrDefault(ct => ct.Name == "Cercado de Lima"),
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "Lima"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "Perú"),
                ZipPostalCode = "LIMA01",
                CreatedOnUtc = DateTime.UtcNow,
            };
            adminUser.Addresses.Add(defaultAdminUserAddress);
            adminUser.CustomerRoles.Add(crAdministrators);
            adminUser.CustomerRoles.Add(crRegistered);
            _customerRepository.Insert(adminUser);
            //set default customer name
            _genericAttributeService.SaveAttribute(adminUser, SystemCustomerAttributeNames.FirstName, "Pedro");
            _genericAttributeService.SaveAttribute(adminUser, SystemCustomerAttributeNames.LastName, "Curich");

            //search engine (crawler) built-in user
            var searchEngineUser = new Customer
            {
                Email = "builtin@search_engine_record.com",
                CustomerGuid = Guid.NewGuid(),
                PasswordFormat = PasswordFormat.Clear,
                AdminComment = "Built-in system guest record used for requests from search engines.",
                Active = true,
                IsSystemAccount = true,
                SystemName = SystemCustomerNames.SearchEngine,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
            };
            searchEngineUser.CustomerRoles.Add(crGuests);
            _customerRepository.Insert(searchEngineUser);


            //built-in user for background tasks
            var backgroundTaskUser = new Customer
            {
                Email = "builtin@background-task-record.com",
                CustomerGuid = Guid.NewGuid(),
                PasswordFormat = PasswordFormat.Clear,
                AdminComment = "Built-in system record used for background tasks.",
                Active = true,
                IsSystemAccount = true,
                SystemName = SystemCustomerNames.BackgroundTask,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
            };
            backgroundTaskUser.CustomerRoles.Add(crGuests);
            _customerRepository.Insert(backgroundTaskUser);
        }

        protected virtual void HashDefaultCustomerPassword(string defaultUserEmail, string defaultUserPassword)
        {
            var customerRegistrationService = EngineContext.Current.Resolve<ICustomerRegistrationService>();
            customerRegistrationService.ChangePassword(new ChangePasswordRequest(defaultUserEmail, false,
                 PasswordFormat.Hashed, defaultUserPassword));
        }

        protected virtual void InstallEmailAccounts()
        {
            var emailAccounts = new List<EmailAccount>
                               {
                                   new EmailAccount
                                       {
                                           Email = "curichpedro@gmail.com",
                                           DisplayName = "Kreier Solutions",
                                           Host = "smtp.gmail.com",
                                           Port = 25,
                                           Username = "curichpedro",
                                           Password = "RADIKAL12345",
                                           EnableSsl = false,
                                           UseDefaultCredentials = false
                                       },
                               };
            _emailAccountRepository.Insert(emailAccounts);
        }

        protected virtual void InstallMessageTemplates()
        {
            var eaGeneral = _emailAccountRepository.Table.FirstOrDefault();
            if (eaGeneral == null)
                throw new Exception("Default email account cannot be loaded");
            var messageTemplates = new List<MessageTemplate>
            {
                new MessageTemplate
                                       {
                                           Name = "Customer.EmailValidationMessage",
                                           Subject = "%Ks.Name%. Validación de correo",
                                           Body = "<a href=\"%Ks.URL%\">%Ks.Name%</a>  <br />  <br />  Para activar su cuenta <a href=\"%Customer.AccountActivationURL%\">click aqui</a>.     <br />  <br />  %Ks.Name%",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                new MessageTemplate
                                       {
                                           Name = "Customer.PasswordRecovery",
                                           Subject = "%Store.Name%. Recuperación de contraseña",
                                           Body = "<a href=\"%Ks.URL%\">%Ks.Name%</a>  <br />  <br />  Para cambiar su contraseña <a href=\"%Customer.PasswordRecoveryURL%\">click aqui</a>.     <br />  <br />  %Store.Name%",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       }
            };
            _messageTemplateRepository.Insert(messageTemplates);
        }

        protected virtual void InstallSettings()
        {
            var settingService = EngineContext.Current.Resolve<ISettingService>();
            settingService.SaveSetting(new SeoSettings
            {
                PageTitleSeparator = ". ",
                PageTitleSeoAdjustment = PageTitleSeoAdjustment.PagenameAfterStorename,
                DefaultTitle = "KS",
                DefaultMetaKeywords = "",
                DefaultMetaDescription = "",
                GenerateProductMetaDescription = true,
                ConvertNonWesternChars = false,
                AllowUnicodeCharsInUrls = true,
                CanonicalUrlsEnabled = false,
                WwwRequirement = WwwRequirement.NoMatter,
                //we disable bundling out of the box because it requires a lot of server resources
                EnableJsBundling = false,
                EnableCssBundling = false,
                TwitterMetaTags = true,
                OpenGraphMetaTags = true,
                ReservedUrlRecordSlugs = new List<string>()
            });

            settingService.SaveSetting(new CustomerSettings
            {
                UsernamesEnabled = false,
                CheckUsernameAvailabilityEnabled = false,
                AllowUsersToChangeUsernames = false,
                DefaultPasswordFormat = PasswordFormat.Hashed,
                HashedPasswordFormat = "SHA1",
                PasswordMinLength = 6,
                PasswordRecoveryLinkDaysValid = 7,
                UserRegistrationType = UserRegistrationType.Standard,
                AllowCustomersToUploadAvatars = false,
                AvatarMaximumSizeBytes = 20000,
                DefaultAvatarEnabled = true,
                ShowCustomersLocation = false,
                ShowCustomersJoinDate = false,
                AllowViewingProfiles = false,
                NotifyNewCustomerRegistration = false,
                //HideDownloadableProductsTab = false,
                //HideBackInStockSubscriptionsTab = false,
                //DownloadableProductsValidateUser = false,
                CustomerNameFormat = CustomerNameFormat.ShowFirstName,
                GenderEnabled = true,
                DateOfBirthEnabled = true,
                DateOfBirthRequired = false,
                DateOfBirthMinimumAge = null,
                CompanyEnabled = true,
                StreetAddressEnabled = false,
                StreetAddress2Enabled = false,
                ZipPostalCodeEnabled = false,
                CityEnabled = false,
                CountryEnabled = false,
                CountryRequired = false,
                StateProvinceEnabled = false,
                StateProvinceRequired = false,
                PhoneEnabled = false,
                FaxEnabled = false,
                //AcceptPrivacyPolicyEnabled = false,
                //NewsletterEnabled = true,
                //NewsletterTickedByDefault = true,
                //HideNewsletterBlock = false,
                //NewsletterBlockAllowToUnsubscribe = false,
                OnlineCustomerMinutes = 20,
                KsSystemLastVisitedPage = false,
                SuffixDeletedCustomers = false,
            });
        }

        protected virtual void InstallActivityLogTypes()
        {
            var activityLogTypes = new List<ActivityLogType>
                                      {
                                          //admin area activities
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "AddNewCategory",
                                                  Enabled = true,
                                                  Name = "Add a new category"
                                              },
                                      };
            _activityLogTypeRepository.Insert(activityLogTypes);
        }

        protected virtual void InstallScheduleTasks()
        {
            var tasks = new List<ScheduleTask>
            {
                new ScheduleTask
                {
                    Name = "Send emails",
                    Seconds = 60,
                    Type = "Ks.Services.Messages.QueuedMessagesSendTask, Ks.Services",
                    Enabled = true,
                    StopOnError = false,
                },
                new ScheduleTask
                {
                    Name = "Keep alive",
                    Seconds = 300,
                    Type = "Ks.Services.Common.KeepAliveTask, Ks.Services",
                    Enabled = true,
                    StopOnError = false,
                },
                new ScheduleTask
                {
                    Name = "Delete guests",
                    Seconds = 600,
                    Type = "Ks.Services.Customers.DeleteGuestsTask, Ks.Services",
                    Enabled = true,
                    StopOnError = false,
                },
                new ScheduleTask
                {
                    Name = "Clear cache",
                    Seconds = 600,
                    Type = "Ks.Services.Caching.ClearCacheTask, Ks.Services",
                    Enabled = false,
                    StopOnError = false,
                },
                new ScheduleTask
                {
                    Name = "Clear log",
                    //60 minutes
                    Seconds = 3600,
                    Type = "Ks.Services.Logging.ClearLogTask, Ks.Services",
                    Enabled = false,
                    StopOnError = false,
                } 
            };

            _scheduleTaskRepository.Insert(tasks);
        }

        #endregion

        #region Methods

        public void InstallData(string defaultUserEmail, string defaultUserPassword, bool installSampleData = true)
        {
            InstallSystem();
            InstallMeasures();
            InstallLanguages();
            InstallCurrencies();
            InstallLocaleResources();
            InstallCountriesAndStatesAndCities();
            InstallCustomersAndUsers(defaultUserEmail,defaultUserPassword);
            InstallEmailAccounts();
            InstallMessageTemplates();
            InstallSettings();
            InstallActivityLogTypes();
            HashDefaultCustomerPassword(defaultUserEmail, defaultUserPassword);
            InstallScheduleTasks();

            if (installSampleData)
            {

            }
        }

        #endregion
    }
}