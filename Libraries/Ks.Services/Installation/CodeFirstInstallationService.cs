using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ks.Core;
using Ks.Core.Data;
using Ks.Core.Domain.Batchs;
using Ks.Core.Domain.Catalog;
using Ks.Core.Domain.Common;
using Ks.Core.Domain.Contract;
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
using Ks.Services.Helpers;
using Ks.Services.Localization;
using Microsoft.Data.Edm.Library.Annotations;

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
        private readonly IRepository<CustomerAttribute> _customerAttributeRepository;
        private readonly IRepository<CustomerAttributeValue> _customerAttributeValueRepository;
        private readonly IRepository<CustomerRole> _customerRoleRepository;
        private readonly IRepository<SpecificationAttribute> _specificationAttributeRepository;
        private readonly IRepository<EmailAccount> _emailAccountRepository;
        private readonly IRepository<MessageTemplate> _messageTemplateRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<StateProvince> _stateProvinceRepository;
        private readonly IRepository<City> _cityRepository;
        private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
        private readonly IRepository<ScheduleTask> _scheduleTaskRepository;
        private readonly IRepository<Tab> _tabRepository;
        private readonly IRepository<Benefit> _benefitRepository;
        private readonly IRepository<Address> _addressRepository;

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public CodeFirstInstallationService(IRepository<KsSystem> ksSystemRepository,
            IRepository<MeasureDimension> measureDimensionRepository, IRepository<MeasureWeight> measureWeightRepository,
            IRepository<Language> languageRepository, IRepository<Currency> currencyRepository,
            IRepository<Customer> customerRepository,
            IRepository<CustomerAttribute> customerAttributeRepository,
            IRepository<CustomerAttributeValue> customerAttributeValueRepository,
            IRepository<CustomerRole> customerRoleRepository,
            IRepository<SpecificationAttribute> specificationAttributeRepository,
            IRepository<EmailAccount> emailAccountRepository,
            IRepository<MessageTemplate> messageTemplateRepository,
            IRepository<Country> countryRepository, IRepository<StateProvince> stateProvinceRepository,
            IRepository<City> cityRepository, IRepository<ActivityLogType> activityLogTypeRepository,
            IRepository<Tab> tabRepository, IRepository<Benefit> benefitRepository, IRepository<ScheduleTask> scheduleTaskRepository, IRepository<Address> addressRepository,
            IGenericAttributeService genericAttributeService, IWebHelper webHelper)
        {
            _ksSystemRepository = ksSystemRepository;
            _measureDimensionRepository = measureDimensionRepository;
            _measureWeightRepository = measureWeightRepository;
            _languageRepository = languageRepository;
            _currencyRepository = currencyRepository;
            _customerRepository = customerRepository;
            _customerAttributeRepository = customerAttributeRepository;
            _customerAttributeValueRepository = customerAttributeValueRepository;
            _customerRoleRepository = customerRoleRepository;
            _specificationAttributeRepository = specificationAttributeRepository;
            _emailAccountRepository = emailAccountRepository;
            _messageTemplateRepository = messageTemplateRepository;
            _countryRepository = countryRepository;
            _stateProvinceRepository = stateProvinceRepository;
            _cityRepository = cityRepository;
            _activityLogTypeRepository = activityLogTypeRepository;
            _scheduleTaskRepository = scheduleTaskRepository;
            _tabRepository = tabRepository;
            _benefitRepository = benefitRepository;
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
                Name = "Lima",
                Abbreviation = "LIM",
                Published = true,
                DisplayOrder = 15,
                Cities = new List<City>
                    {
                        new City{DisplayOrder=1,Name="Cercado de Lima",Published = true },
                        new City{DisplayOrder=2,Name="Ancón",Published = true },
                        new City{DisplayOrder=3,Name="Ate",Published = true },
                        new City{DisplayOrder=4,Name="Barranco",Published = true },
                        new City{DisplayOrder=5,Name="Breña",Published = true },
                        new City{DisplayOrder=6,Name="Carabayllo",Published = true },
                        new City{DisplayOrder=7,Name="Chaclacayo",Published = true },
                        new City{DisplayOrder=8,Name="Chorrillos",Published = true },
                        new City{DisplayOrder=9,Name="Cieneguilla",Published = true },
                        new City{DisplayOrder=10,Name="Comas",Published = true },
                        new City{DisplayOrder=11,Name="El Agustino",Published = true },
                        new City{DisplayOrder=12,Name="Independencia",Published = true },
                        new City{DisplayOrder=13,Name="Jesús María",Published = true },
                        new City{DisplayOrder=14,Name="La Molina",Published = true },
                        new City{DisplayOrder=15,Name="La Victoria",Published = true },
                        new City{DisplayOrder=16,Name="Lince",Published = true },
                        new City{DisplayOrder=17,Name="Los Olivos",Published = true },
                        new City{DisplayOrder=18,Name="Lurin",Published = true },
                        new City{DisplayOrder=19,Name="Magdalena del Mar",Published = true },
                        new City{DisplayOrder=20,Name="Miraflores",Published = true },
                        new City{DisplayOrder=21,Name="Pueblo Libre",Published = true },
                        new City{DisplayOrder=22,Name="Pachacámac",Published = true },
                        new City{DisplayOrder=23,Name="Pucusana",Published = true },
                        new City{DisplayOrder=24,Name="Puente Piedra",Published = true },
                        new City{DisplayOrder=25,Name="Punta Hermosa",Published = true },
                        new City{DisplayOrder=26,Name="Punta Negra",Published = true },
                        new City{DisplayOrder=27,Name="Rímac",Published = true },
                        new City{DisplayOrder=28,Name="San Bartolo",Published = true },
                        new City{DisplayOrder=29,Name="San Borja",Published = true },
                        new City{DisplayOrder=30,Name="San Isidro",Published = true },
                        new City{DisplayOrder=31,Name="San Juan de Lurigancho",Published = true },
                        new City{DisplayOrder=32,Name="San Juan de Miraflores",Published = true },
                        new City{DisplayOrder=33,Name="San Luis",Published = true },
                        new City{DisplayOrder=34,Name="San Martín de Porres",Published = true },
                        new City{DisplayOrder=35,Name="San Miguel",Published = true },
                        new City{DisplayOrder=36,Name="Santa Anita",Published = true },
                        new City{DisplayOrder=37,Name="Santa María del Mar",Published = true },
                        new City{DisplayOrder=38,Name="Santa Rosa",Published = true },
                        new City{DisplayOrder=39,Name="Santiago de Surco",Published = true },
                        new City{DisplayOrder=40,Name="Surquillo",Published = true },
                        new City{DisplayOrder=41,Name="Villa ElSalvador",Published = true },
                        new City{DisplayOrder=42,Name="Villa María del Triunfo",Published = true}}
            });

            var countries = new List<Country> { cPer };
            _countryRepository.Insert(countries);
        }

        protected virtual void InstallCustomersAndUsers(string defaultUserEmail, string defaultUserPassword)
        {
            var crAdministrators = new CustomerRole
            {
                Name = "Administrador",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemCustomerRoleNames.Administrators,
            };

            var crAssociated = new CustomerRole
            {
                Name = "Asociado",
                Active = true,
                IsSystemRole = false,
                SystemName = SystemCustomerRoleNames.Associated
            };

            var crAuxAccountant = new CustomerRole
            {
                Name = "Auxiliar Contable",
                Active = true,
                IsSystemRole = false,
                SystemName = SystemCustomerRoleNames.Accountant,
            };

            var crSecretary = new CustomerRole
            {
                Name = "Secretaria",
                Active = true,
                IsSystemRole = false,
                SystemName = SystemCustomerRoleNames.Secretary
            };
            var crManager = new CustomerRole
            {
                Name = "Gerente",
                Active = true,
                IsSystemRole = false,
                SystemName = SystemCustomerRoleNames.Manager
            };
            var crEmployer = new CustomerRole
            {
                Name = "Trabajador",
                Active = true,
                IsSystemRole = false,
                SystemName = SystemCustomerRoleNames.Employee
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
                                    crEmployer,
                                    crManager,
                                    crAssociated
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
                IsSystemAccount = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
            };

            var employerUser = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = "abc@xyz.com",
                Username = "abc@xyz.com",
                Password = "admin",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = "",
                Active = true,
                IsSystemAccount = true,
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
                Address1 = "",
                Address2 = "",
                City = _cityRepository.Table.FirstOrDefault(ct => ct.Name == "Cercado de Lima"),
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "Lima"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "PER"),
                ZipPostalCode = "LIMA01",
                CreatedOnUtc = DateTime.UtcNow,
            };
            adminUser.Addresses.Add(defaultAdminUserAddress);
            adminUser.CustomerRoles.Add(crAdministrators);
            adminUser.CustomerRoles.Add(crAssociated);
            adminUser.CustomerRoles.Add(crEmployer);
            adminUser.CustomerRoles.Add(crManager);
            _customerRepository.Insert(adminUser);
            //set default customer name
            _genericAttributeService.SaveAttribute(adminUser, SystemCustomerAttributeNames.FirstName, "Pedro");
            _genericAttributeService.SaveAttribute(adminUser, SystemCustomerAttributeNames.LastName, "Curich");
            _genericAttributeService.SaveAttribute(adminUser, SystemCustomerAttributeNames.AdmCode, "123456789");
            _genericAttributeService.SaveAttribute(adminUser, SystemCustomerAttributeNames.Dni, "43617372");

            //test manager rol
            employerUser.CustomerRoles.Add(crEmployer);
            _customerRepository.Insert(employerUser);

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

        protected virtual void InstallCustomerAttribute()
        {
            var customerAttributes = new List<CustomerAttribute>
            {
                
                new CustomerAttribute {Name = "Grado", AttributeControlTypeId = 1},
                new CustomerAttribute {Name = "Arma", AttributeControlTypeId = 1},
                new CustomerAttribute {Name = "Estado Civil", AttributeControlTypeId = 1},
                new CustomerAttribute {Name = "Fecha de egreso de la Escuela Militar", AttributeControlTypeId = 20},
                new CustomerAttribute {Name = "Unidad", AttributeControlTypeId = 4},
                new CustomerAttribute {Name = "Gran Unidad", AttributeControlTypeId = 4},
                new CustomerAttribute {Name = "División Militar", AttributeControlTypeId = 4},
				
				//nuevos
				new CustomerAttribute {Name = "Guarnición", AttributeControlTypeId = 4},
				new CustomerAttribute {Name = "Nombre Conyuge", AttributeControlTypeId = 4},
				new CustomerAttribute {Name = "Nro de Solicitud Pago Personal", AttributeControlTypeId = 4},
				new CustomerAttribute {Name = "Fecha de Solicitud Pago Personal", AttributeControlTypeId = 20},
				new CustomerAttribute {Name = "Nro de Solicitud de Renuncia", AttributeControlTypeId = 4},
				new CustomerAttribute {Name = "Fecha de Solicitud de Renuncia", AttributeControlTypeId = 20},
				new CustomerAttribute {Name = "Es fallecido?", AttributeControlTypeId =2},
				new CustomerAttribute {Name = "Fecha Fallecimiento", AttributeControlTypeId = 20},
				new CustomerAttribute {Name = "Nro de Acta de Apertura de Carta Declaratoria", AttributeControlTypeId = 4},
				new CustomerAttribute {Name = "Fecha de Acta de Apertura de Carta Declaratoria", AttributeControlTypeId = 20},
				new CustomerAttribute {Name = "Nro Resolución Baja Asociado", AttributeControlTypeId = 4},
				new CustomerAttribute {Name = "Fecha Resolución Baja Asociado", AttributeControlTypeId = 20},
				new CustomerAttribute {Name = "Proceso Judicial (Nº Expediente)", AttributeControlTypeId = 4},
				new CustomerAttribute {Name = "Beneficio Económico Comprometido", AttributeControlTypeId = 4},
				new CustomerAttribute {Name = "Auxilio Económico Comprometido", AttributeControlTypeId = 4},
				new CustomerAttribute {Name = "Motivo de Baja", AttributeControlTypeId = 4},
				new CustomerAttribute {Name = "Fecha Pase Situación de Retiro", AttributeControlTypeId = 20},
				new CustomerAttribute {Name = "Causal Pase Situación de Retiro", AttributeControlTypeId = 4},
				new CustomerAttribute {Name = "Teléfono Fijo", AttributeControlTypeId = 4},
				new CustomerAttribute {Name = "Teléfonos Moviles", AttributeControlTypeId = 4},
				new CustomerAttribute {Name = "Teléfono de Referencia", AttributeControlTypeId = 4},
				new CustomerAttribute {Name = "Comentario del Usuario", AttributeControlTypeId = 4}				
				
				
            };

            var customerAttributeValues = new List<CustomerAttributeValue>
            {
                new CustomerAttributeValue{Name = "SUB-TENIENTE",DisplayOrder = 1, CustomerAttributeId =1},
                new CustomerAttributeValue{Name = "TENIENTE",DisplayOrder = 2, CustomerAttributeId =1},
                new CustomerAttributeValue{Name = "ALFERES CABALLERIA",DisplayOrder = 3, CustomerAttributeId =1},
                new CustomerAttributeValue{Name = "ALFERES ARTILLERIA",DisplayOrder = 4, CustomerAttributeId =1},
                new CustomerAttributeValue{Name = "CAPITAN",DisplayOrder = 5, CustomerAttributeId =1},
                new CustomerAttributeValue{Name = "MAYOR",DisplayOrder = 6, CustomerAttributeId =1},
                new CustomerAttributeValue{Name = "TENIENTE CORONEL",DisplayOrder = 7, CustomerAttributeId =1},
                new CustomerAttributeValue{Name = "CORONEL",DisplayOrder = 8, CustomerAttributeId =1},
                new CustomerAttributeValue{Name = "GENERAL DE BRIGADA",DisplayOrder = 9, CustomerAttributeId =1},
                new CustomerAttributeValue{Name = "GENERAL DE DIVISION",DisplayOrder = 10, CustomerAttributeId =1},

                new CustomerAttributeValue{Name = "CASADO(A)".ToUpper(),DisplayOrder = 1, CustomerAttributeId =3},
                new CustomerAttributeValue{Name = "DIVORCIADO(A)".ToUpper(),DisplayOrder = 2, CustomerAttributeId =3},
                new CustomerAttributeValue{Name = "SOLTERO(A)".ToUpper(),DisplayOrder = 3, CustomerAttributeId =3},
                new CustomerAttributeValue{Name = "VIUDO(A)".ToUpper(),DisplayOrder = 4, CustomerAttributeId =3},
				
				
				//nuevos
				new CustomerAttributeValue{Name = "ART",DisplayOrder = 1, CustomerAttributeId = 2},
				new CustomerAttributeValue{Name = "CAB",DisplayOrder = 2, CustomerAttributeId = 2},
				new CustomerAttributeValue{Name = "COM",DisplayOrder = 3, CustomerAttributeId = 2},
				new CustomerAttributeValue{Name = "DICYT",DisplayOrder = 4, CustomerAttributeId = 2},
				new CustomerAttributeValue{Name = "FARM",DisplayOrder = 5, CustomerAttributeId = 2},
				new CustomerAttributeValue{Name = "INF",DisplayOrder = 6, CustomerAttributeId = 2},
				new CustomerAttributeValue{Name = "ING",DisplayOrder = 7, CustomerAttributeId = 2},
				new CustomerAttributeValue{Name = "INT",DisplayOrder = 8, CustomerAttributeId = 2},
				new CustomerAttributeValue{Name = "INTG",DisplayOrder = 9, CustomerAttributeId = 2},
				new CustomerAttributeValue{Name = "MED",DisplayOrder = 10, CustomerAttributeId = 2},
				new CustomerAttributeValue{Name = "MG",DisplayOrder = 11, CustomerAttributeId = 2},
				new CustomerAttributeValue{Name = "ODONT",DisplayOrder = 12, CustomerAttributeId = 2},
				new CustomerAttributeValue{Name = "PEF",DisplayOrder = 13, CustomerAttributeId = 2},
				new CustomerAttributeValue{Name = "PSICO",DisplayOrder = 14, CustomerAttributeId = 2},
				new CustomerAttributeValue{Name = "SAN",DisplayOrder = 15, CustomerAttributeId = 2},
				new CustomerAttributeValue{Name = "SCYT",DisplayOrder = 16, CustomerAttributeId = 2},
				new CustomerAttributeValue{Name = "SJE",DisplayOrder = 17, CustomerAttributeId = 2},
				new CustomerAttributeValue{Name = "VET",DisplayOrder = 18, CustomerAttributeId = 2}

            };
            _customerAttributeRepository.Insert(customerAttributes);
            _customerAttributeValueRepository.Insert(customerAttributeValues);
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
                                           Subject = "%System.Name%. Validación de correo",
                                           Body = "<a href=\"%Ks.URL%\">%Ks.Name%</a>  <br />  <br />  Para activar su cuenta <a href=\"%Customer.AccountActivationURL%\">click aqui</a>.     <br />  <br />  %System.Name%",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                new MessageTemplate
                                       {
                                           Name = "Customer.PasswordRecovery",
                                           Subject = "%System.Name%. Recuperación de contraseña",
                                           Body = "<a href=\"%System.URL%\">%System.Name%</a>  <br />  <br />  Para cambiar su contraseña <a href=\"%Customer.PasswordRecoveryURL%\">click aqui</a>.     <br />  <br />  %System.Name%",
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

            settingService.SaveSetting(new CommonSettings
            {
                UseSystemEmailForContactUsForm = true,
                UseStoredProceduresIfSupported = true,
                SitemapEnabled = true,
                SitemapIncludeCategories = true,
                SitemapIncludeManufacturers = true,
                SitemapIncludeProducts = false,
                DisplayJavaScriptDisabledWarning = false,
                UseFullTextSearch = false,
                FullTextMode = FulltextSearchMode.ExactMatch,
                Log404Errors = true,
                BreadcrumbDelimiter = "/",
                RenderXuaCompatible = false,
                XuaCompatibleValue = "IE=edge"
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
                ShowCustomersLocation = false,
                ShowCustomersJoinDate = false,
                AllowViewingProfiles = false,
                NotifyNewCustomerRegistration = false,
                CustomerNameFormat = CustomerNameFormat.ShowFirstName,
                GenderEnabled = true,
                DateOfBirthEnabled = true,
                DateOfBirthRequired = false,
                DateOfBirthMinimumAge = null,
                StreetAddressEnabled = true,
                StreetAddress2Enabled = false,
                AdmiCodeEnabled= true,
                DniEnabled = true,
                CityEnabled = true,
                CountryEnabled = true,
                CountryRequired = false,
                StateProvinceEnabled = true,
                StateProvinceRequired = false,
                PhoneEnabled = false,
                FaxEnabled = false,
                OnlineCustomerMinutes = 20,
                KsSystemLastVisitedPage = false,
                SuffixDeletedCustomers = false,
            });

            settingService.SaveSetting(new AdminAreaSettings
            {
                DefaultGridPageSize = 15,
                GridPageSizes = "10, 15, 20, 50, 100",
                RichEditorAdditionalSettings = null,
                RichEditorAllowJavaScript = false
            });

            settingService.SaveSetting(new DateTimeSettings
            {
                AllowCustomersToSetTimeZone = false,
                DefaultStoreTimeZoneId = "SA Pacific Standard Time"
            });

            var eaGeneral = _emailAccountRepository.Table.FirstOrDefault();
            if (eaGeneral == null)
                throw new Exception("Default email account cannot be loaded");

            settingService.SaveSetting(new EmailAccountSettings
            {
                DefaultEmailAccountId = eaGeneral.Id
            });

            settingService.SaveSetting(new CurrencySettings
            {
                DisplayCurrencyLabel = false,
                PrimaryStoreCurrencyId = _currencyRepository.Table.Single(c => c.CurrencyCode == "USD").Id,
                PrimaryExchangeRateCurrencyId = _currencyRepository.Table.Single(c => c.CurrencyCode == "USD").Id,
                ActiveExchangeRateProviderSystemName = "CurrencyExchange.MoneyConverter",
                AutoUpdateEnabled = false
            });

            settingService.SaveSetting(new MeasureSettings
            {
                BaseDimensionId = _measureDimensionRepository.Table.Single(m => m.SystemKeyword == "metros").Id,
                BaseWeightId = _measureWeightRepository.Table.Single(m => m.SystemKeyword == "kg").Id,
            });

            settingService.SaveSetting(new MessageTemplatesSettings
            {
                CaseInvariantReplacement = false,
                Color1 = "#b9babe",
                Color2 = "#ebecee",
                Color3 = "#dde2e6",
            });

            settingService.SaveSetting(new DateTimeSettings
            {
                DefaultStoreTimeZoneId = "",
                AllowCustomersToSetTimeZone = false
            });

            settingService.SaveSetting(new SequenceIdsSettings
            {
                AuthorizeDiscount = 1583,
                DeclaratoryLetter = 11671,
                RegistrationForm = 54651,
                AuthorizeLoan = 1000,
                NumberOfLiquidation = 1291
            });

            settingService.SaveSetting(new ContributionSettings
            {
                TotalCycle = 12 * 35,
                DayOfPaymentContribution = 20,
                CycleOfDelay = 6,
                IsActiveAmount1 = true,
                NameAmount1 = "Valor de aportación",
                Amount1 = 34.85M,
                //Amount1 = 35M,
                Amount2 = 0M,
                Amount3 = 0M,
                //MaximumCharge = (decimal) (70)
                MaximumCharge = (decimal)(34.85 * 2)
            });

            settingService.SaveSetting(new LoanSettings
            {
                Periods = "12,18,24,36",
                Tea = .08,
                Safe = .01,
                DayOfPaymentLoan = 15,
                IsEnable1 = true,
                StateName1 = "A",
                MinClycle1 = 0,
                MaxClycle1 = 15,
                HasOnlySignature1 = true,
                MinAmountWithSignature1 = 500M,
                MaxAmountWithSignature1 = 5000M,
                HasWarranty1 = true,
                MinAmountWithWarranty1 = 5500M,
                MaxAmountWithWarranty1 = 12000M,

                StateName2 = "B",
                MinClycle2 = 15,
                MaxClycle2 = 20,
                HasOnlySignature2 = true,
                MinAmountWithSignature2 = 500M,
                MaxAmountWithSignature2 = 6000M,
                HasWarranty2 = true,
                MinAmountWithWarranty2 = 6500M,
                MaxAmountWithWarranty2 = 12000M,

                StateName3 = "C",
                MinClycle3 = 20,
                MaxClycle3 = 35,
                HasOnlySignature3 = true,
                MinAmountWithSignature3 = 500M,
                MaxAmountWithSignature3 = 12000M,
                HasWarranty3 = false,
                MinAmountWithWarranty3 = 0M,
                MaxAmountWithWarranty3 = 0M,

                StateName4 = "D",
                MinClycle4 = 20,
                MaxClycle4 = 35,
                HasOnlySignature4 = true,
                MinAmountWithSignature4 = 500M,
                MaxAmountWithSignature4 = 6000M,
                HasWarranty4 = false,
                MinAmountWithWarranty4 = 6500M,
                MaxAmountWithWarranty4 = 12000M,

                StateName5 = "E",

                CashFlow = "<?xml version=\"1.0\"?>\r\n<ArrayOfCashFlowModel xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <CashFlowModel>\r\n    <Id>1</Id>\r\n    <Since>500</Since>\r\n    <To>3500</To>\r\n    <Amount>1300</Amount>\r\n  </CashFlowModel>\r\n  <CashFlowModel>\r\n    <Id>2</Id>\r\n    <Since>4000</Since>\r\n    <To>6500</To>\r\n    <Amount>2000</Amount>\r\n  </CashFlowModel>\r\n  <CashFlowModel>\r\n    <Id>3</Id>\r\n    <Since>7000</Since>\r\n    <To>9500</To>\r\n    <Amount>3000</Amount>\r\n  </CashFlowModel>\r\n  <CashFlowModel>\r\n    <Id>4</Id>\r\n    <Since>1000</Since>\r\n    <To>12000</To>\r\n    <Amount>4000</Amount>\r\n  </CashFlowModel>\r\n</ArrayOfCashFlowModel>"
            });

            settingService.SaveSetting(new BankSettings
            {
                IdBank1 = 1,
                IsActive1 = true,
                NameBank1 = "BBVA Continental",
                AccountNumber1 = "0011-0199-0200374278",
                IdBank2 = 2,
                IsActive2 = true,
                NameBank2 = "Banco de Crédito del Perú",
                AccountNumber2 = "2222222222222",
                IdBank3 = 3,
                IsActive3 = true,
                NameBank3 = "Interbank",
                AccountNumber3 = "33333333333333",
                IdBank4 = 4,
                IsActive4 = true,
                NameBank4 = "BANBIF Banco Interamericano de Finanzas",
                AccountNumber4 = "44444444444444",
                IdBank5 = 5,
                IsActive5 = true,
                NameBank5 = "Scotiabank Perú",
                AccountNumber5 = "5555555555555555"
            });

            settingService.SaveSetting(new BenefitValueSetting { AmountBaseOfBenefit = 12801.59M });

            settingService.SaveSetting(new ScheduleBatchsSetting
            {
                ServiceName1 = "Ks.Batch.Caja.Out",
                DayOfProcess1 = 15,
                DayOfProcess2 = 20,
                ServiceName2 = "Ks.Batch.Copere.Out"
            });

            settingService.SaveSetting(new SignatureSettings
            {
                DefaultName = "AUXILIO COOPERATIVO MILITAR DE RETIRO",
                BenefitLeftPosition = "CONTADOR",
                BenefitCenterPosition = "GERENTE ADMINISTRATIVO",
                BenefitRightPosition = "APORTES Y BENEFICIOS",

                BenefitLeftName = "CARLOS JESUS ALCANTARA MOLINA",
                BenefitCenterName = "MANUEL ANTONIO VELEZ CARRASCO",
                BenefitRightName = "RONNY ALBERTO CALLAN ISASI",

            });

        }

        protected virtual void InstallActivityLogTypes()
        {
            var activityLogMilitaryPerson = new List<ActivityLogType>
                {
                    new ActivityLogType
                    {SystemKeyword = "AddNewMilitaryPerson",Enabled = true,Name = "Registrar Nuevo Personal Militar"},
                    new ActivityLogType
                    {SystemKeyword = "ViewMilitaryPerson",Enabled = true,Name = "Ver Personal Militar"},
                    new ActivityLogType
                    {SystemKeyword = "EditMilitaryPerson",Enabled = true,Name = "Editar Personal Militar"},
                    new ActivityLogType
                    {SystemKeyword = "DeleteMilitaryPerson",Enabled = true,Name = "Eliminar Personal Militar"}
                };

            var activityLogMembership = new List<ActivityLogType>
                {
                    new ActivityLogType
                    {SystemKeyword = "AddMembership",Enabled = true,Name = "Registrar Personal Militar"},
                    new ActivityLogType
                    {SystemKeyword = "ViewMembership",Enabled = true,Name = "Ver Personal Militar"},
                    new ActivityLogType
                    {SystemKeyword = "EditMembership",Enabled = true,Name = "Editar Personal Militar"}
                };

            var activityLogMaintenance = new List<ActivityLogType>
                {
                    new ActivityLogType
                    {SystemKeyword = "AddMaintenance",Enabled = true,Name = "Registrar Mantenimiento"},
                    new ActivityLogType
                    {SystemKeyword = "ViewMaintenance",Enabled = true,Name = "Ver Mantenimiento"},
                    new ActivityLogType
                    {SystemKeyword = "EditMaintenance",Enabled = true,Name = "Editar Mantenimiento"}
                };

            var activityLogContributions = new List<ActivityLogType>
                {
                    new ActivityLogType
                    {SystemKeyword = "AddContributions",Enabled = true,Name = "Registrar Aportaciones"},
                    new ActivityLogType
                    {SystemKeyword = "ViewContributions",Enabled = true,Name = "Ver Aportaciones"},
                    new ActivityLogType
                    {SystemKeyword = "EditContributions",Enabled = true,Name = "Editar Aportaciones"}
                };

            var activityLogReturns = new List<ActivityLogType>
                {
                    new ActivityLogType
                    {SystemKeyword = "AddReturns",Enabled = true,Name = "Registrar Devoluciones"},
                    new ActivityLogType
                    {SystemKeyword = "ViewReturns",Enabled = true,Name = "Ver Devoluciones"},
                    new ActivityLogType
                    {SystemKeyword = "EditReturns",Enabled = true,Name = "Editar Devoluciones"}
                };

            var activityLogPaymentSocialSupport = new List<ActivityLogType>
                {
                    new ActivityLogType
                    {SystemKeyword = "AddPaymentSocialSupport",Enabled = true,Name = "Registrar Pago Apoyo Social"},
                    new ActivityLogType
                    {SystemKeyword = "ViewPaymentSocialSupport",Enabled = true,Name = "Ver Pago Apoyo Social"},
                    new ActivityLogType
                    {SystemKeyword = "EditPaymentSocialSupport",Enabled = true,Name = "Editar Pago Apoyo Social"}
                };
            var activityLogTabulator = new List<ActivityLogType>
                {
                    new ActivityLogType
                    {SystemKeyword = "AddTabulator",Enabled = true,Name = "Registrar Tabulador"},
                    new ActivityLogType
                    {SystemKeyword = "ViewTabulator",Enabled = true,Name = "Ver Tabulador"},
                    new ActivityLogType
                    {SystemKeyword = "EditTabulator",Enabled = true,Name = "Editar Tabulador"}
                };

            var activityLogScale = new List<ActivityLogType>
                {
                    new ActivityLogType
                    {SystemKeyword = "AddScale",Enabled = true,Name = "Registrar Escala ASE"},
                    new ActivityLogType
                    {SystemKeyword = "ViewScale",Enabled = true,Name = "Ver Escala ASE"},
                    new ActivityLogType
                    {SystemKeyword = "EditScale",Enabled = true,Name = "Editar Escala ASE"}
                };

            var activityLogLog = new List<ActivityLogType>
                {
                    new ActivityLogType
                    {SystemKeyword = "ViewLog",Enabled = true,Name = "Ver Log (auditoria)"} 
                };

            var activityLogSalaryScale = new List<ActivityLogType>
                {
                    new ActivityLogType
                    {SystemKeyword = "AddSalaryScale",Enabled = true,Name = "Registrar Escala Sueldo"},
                    new ActivityLogType
                    {SystemKeyword = "ViewSalaryScale",Enabled = true,Name = "Ver Escala Sueldo"},
                    new ActivityLogType
                    {SystemKeyword = "EditSalaryScale",Enabled = true,Name = "Editar Escala Sueldo"}
                };

            var activityLogCurrentAccount = new List<ActivityLogType>
                {
                    new ActivityLogType
                    {SystemKeyword = "AddCurrentAccount",Enabled = true,Name = "Registrar Cuenta Corriente"},
                    new ActivityLogType
                    {SystemKeyword = "ViewCurrentAccount",Enabled = true,Name = "Ver Cuenta Corriente"},
                    new ActivityLogType
                    {SystemKeyword = "EditCurrentAccount",Enabled = true,Name = "Editar Cuenta Corriente"}
                };

            var activityLogSetting = new List<ActivityLogType>
            {
                new ActivityLogType{SystemKeyword = "EditSettings",Enabled = true,Name = "Edit setting(s)"},
                new ActivityLogType{SystemKeyword = "EditSpecAttribute",Enabled = true,Name = "Edit a specification attribute"},
                new ActivityLogType{SystemKeyword = "DeleteSetting",Enabled = true,Name = "Delete a setting"},
                new ActivityLogType{SystemKeyword = "DeleteSpecAttribute",Enabled = true,Name = "Delete a specification attribute"},
            };

            _activityLogTypeRepository.Insert(activityLogMilitaryPerson);
            _activityLogTypeRepository.Insert(activityLogMembership);
            _activityLogTypeRepository.Insert(activityLogMaintenance);
            _activityLogTypeRepository.Insert(activityLogContributions);
            _activityLogTypeRepository.Insert(activityLogReturns);
            _activityLogTypeRepository.Insert(activityLogPaymentSocialSupport);
            _activityLogTypeRepository.Insert(activityLogTabulator);
            _activityLogTypeRepository.Insert(activityLogScale);
            _activityLogTypeRepository.Insert(activityLogLog);
            _activityLogTypeRepository.Insert(activityLogSalaryScale);
            _activityLogTypeRepository.Insert(activityLogCurrentAccount);

            _activityLogTypeRepository.Insert(activityLogSetting);

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

        protected virtual void InstallTabs()
        {
            var tab = new Tab
            {
                Name = "Tabulador para Beneficio " + DateTime.Now.ToShortDateString(),
                AmountBase = 12801.59M,
                IsActive = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                TabDetails = new List<TabDetail>
                {
                    new TabDetail{YearInActivity =1,TabValue = 0.02857,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =2,TabValue = 0.05714,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =3,TabValue = 0.08571,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =4,TabValue = 0.11429,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =5,TabValue = 0.14286,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =6,TabValue = 0.17143,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =7,TabValue = 0.2,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =8,TabValue = 0.22857,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =9,TabValue = 0.25714,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =10,TabValue = 0.28571,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =11,TabValue = 0.31429,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =12,TabValue = 0.34286,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =13,TabValue = 0.37143,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =14,TabValue = 0.4,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =15,TabValue = 0.42857,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =16,TabValue = 0.45714,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =17,TabValue = 0.48571,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =18,TabValue = 0.51429,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =19,TabValue = 0.54286,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =20,TabValue = 0.57143,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =21,TabValue = 0.6,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =22,TabValue = 0.62857,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =23,TabValue = 0.65714,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =24,TabValue = 0.68571,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =25,TabValue = 0.71429,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =26,TabValue = 0.74286,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =27,TabValue = 0.77143,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =28,TabValue = 0.8,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =29,TabValue = 0.82857,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =30,TabValue = 0.85714,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =31,TabValue = 0.88571,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =32,TabValue = 0.91429,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =33,TabValue = 0.94286,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =34,TabValue = 0.97143,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow},
                    new TabDetail{YearInActivity =35,TabValue = 1,CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc = DateTime.UtcNow}
                }
            };
            _tabRepository.Insert(tab);
        }

        protected virtual void InstallBenefit()
        {
            var benefits = new List<Benefit>
            {
                new Benefit{Name ="Fallecimiento de un Hijo(a)", BenefitTypeId =(int)BenefitType.Auxilio, Discount = .25, LetterDeclaratory=false,CancelLoans =true, IsActive= true,DisplayOrder = 1, CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc =DateTime.UtcNow, Description ="Ayuda economica que recibe el asociado al fallecer un hijo(a), teniendo en cuenta un factor del 25% y los años que ha realizado aportaciones. No se considera los montos por pagar de los prestamos adquiridos" },
                new Benefit{Name ="Fallecimiento de la Esposa",  BenefitTypeId =(int)BenefitType.Auxilio, Discount = .50, LetterDeclaratory=false,CancelLoans =false, IsActive= true,DisplayOrder = 1, CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc =DateTime.UtcNow, Description ="Ayuda economica que recibe el asociado al fallecer un esposa, teniendo en cuenta un factor del 50% y los años que ha realizado aportaciones. No se considera los montos por pagar de los prestamos adquiridos"},
                new Benefit{Name ="Falleciento del Asociado",  BenefitTypeId =(int)BenefitType.Beneficio, Discount = 1, LetterDeclaratory=true,CancelLoans =false, IsActive= true,DisplayOrder = 1, CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc =DateTime.UtcNow, Description ="Ayuda economica que recibe el asociado al fallecer un hijo(a), teniendo en cuenta un factor del 25% y los años que ha realizado aportaciones. No se considera los montos por pagar de los prestamos adquiridos" },
                new Benefit{Name ="Normal - 35 años", CloseContributions=true, BenefitTypeId =(int)BenefitType.Beneficio, Discount = 1,LetterDeclaratory=false,CancelLoans =true, IsActive= true,DisplayOrder = 1, CreatedOnUtc = DateTime.UtcNow,UpdatedOnUtc =DateTime.UtcNow, Description ="Ayuda economica que recibe el asociado al fallecer un hijo(a), teniendo en cuenta un factor del 25% y los años que ha realizado aportaciones. Si se considera los montos por pagar de los prestamos adquiridos" }
            };

            _benefitRepository.Insert(benefits);
        }

        #endregion

        #region Methods

        public void InstallData(string defaultUserEmail, string defaultUserPassword, bool installSampleData = true)
        {
            try
            {
                InstallSystem();
                InstallMeasures();
                InstallLanguages();
                InstallCurrencies();
                InstallCountriesAndStatesAndCities();
                InstallCustomersAndUsers(defaultUserEmail, defaultUserPassword);
                InstallCustomerAttribute();
                InstallEmailAccounts();
                InstallMessageTemplates();
                InstallSettings();
                InstallLocaleResources();
                InstallActivityLogTypes();
                HashDefaultCustomerPassword(defaultUserEmail, defaultUserPassword);
                InstallScheduleTasks();
                InstallTabs();
                InstallBenefit();
                if (installSampleData)
                {

                }
            }
            catch (Exception es)
            {
                var ess = es.Message;
            }
        }

        #endregion
    }
}