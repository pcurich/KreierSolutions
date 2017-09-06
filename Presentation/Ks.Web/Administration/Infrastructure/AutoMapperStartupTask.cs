using AutoMapper;
using Ks.Admin.Models.Batchs;
using Ks.Admin.Models.Common;
using Ks.Admin.Models.Contract;
using Ks.Admin.Models.Customers;
using Ks.Admin.Models.Directory;
using Ks.Admin.Models.Localization;
using Ks.Admin.Models.Logging;
using Ks.Admin.Models.Messages;
using Ks.Admin.Models.Settings;
using Ks.Admin.Models.Systems;
using Ks.Core.Domain.Batchs;
using Ks.Core.Domain.Common;
using Ks.Core.Domain.Contract;
using Ks.Core.Domain.Customers;
using Ks.Core.Domain.Directory;
using Ks.Core.Domain.Localization;
using Ks.Core.Domain.Logging;
using Ks.Core.Domain.Messages;
using Ks.Core.Domain.System;
using Ks.Core.Infrastructure;

namespace Ks.Admin.Infrastructure
{
    public class AutoMapperStartupTask : IStartupTask
    {
        public void Execute()
        {
            //TODO remove 'CreatedOnUtc' ignore mappings because now presentation layer models have 'CreatedOn' property and core entities have 'CreatedOnUtc' property (distinct names)

            #region Admin

            Mapper.CreateMap<WorkFlow, WorkFlowModel>()
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<WorkFlowModel, WorkFlow>()
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore());

            #endregion

            #region Address

            Mapper.CreateMap<Address, AddressModel>()
                .ForMember(dest => dest.AddressHtml, mo => mo.Ignore())
                .ForMember(dest => dest.CustomAddressAttributes, mo => mo.Ignore())
                .ForMember(dest => dest.FormattedCustomAddressAttributes, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableCountries, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableStates, mo => mo.Ignore())
                .ForMember(dest => dest.FirstNameEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.FirstNameRequired, mo => mo.Ignore())
                .ForMember(dest => dest.LastNameEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.LastNameRequired, mo => mo.Ignore())
                .ForMember(dest => dest.EmailEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.EmailRequired, mo => mo.Ignore())
                .ForMember(dest => dest.CountryEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.StateProvinceEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.CityEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.CityRequired, mo => mo.Ignore())
                .ForMember(dest => dest.StreetAddressEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.StreetAddressRequired, mo => mo.Ignore())
                .ForMember(dest => dest.StreetAddress2Enabled, mo => mo.Ignore())
                .ForMember(dest => dest.StreetAddress2Required, mo => mo.Ignore())
                .ForMember(dest => dest.ZipPostalCodeEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.ZipPostalCodeRequired, mo => mo.Ignore())
                .ForMember(dest => dest.PhoneEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.PhoneRequired, mo => mo.Ignore())
                .ForMember(dest => dest.FaxEnabled, mo => mo.Ignore())
                .ForMember(dest => dest.FaxRequired, mo => mo.Ignore())
                .ForMember(dest => dest.CountryName,
                    mo => mo.MapFrom(src => src.Country != null ? src.Country.Name : null))
                .ForMember(dest => dest.StateProvinceName,
                    mo => mo.MapFrom(src => src.StateProvince != null ? src.StateProvince.Name : null))
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<AddressModel, Address>()
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.Country, mo => mo.Ignore())
                .ForMember(dest => dest.CustomAttributes, mo => mo.Ignore())
                .ForMember(dest => dest.StateProvince, mo => mo.Ignore());

            #endregion

            #region Countries

            Mapper.CreateMap<CountryModel, Country>()
                .ForMember(dest => dest.StateProvinces, mo => mo.Ignore());
            //.ForMember(dest => dest.RestrictedShippingMethods, mo => mo.Ignore());
            Mapper.CreateMap<Country, CountryModel>()
                .ForMember(dest => dest.NumberOfStates,
                    mo => mo.MapFrom(src => src.StateProvinces != null ? src.StateProvinces.Count : 0))
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());

            #endregion

            #region Estado

            Mapper.CreateMap<StateProvince, StateProvinceModel>()
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<StateProvinceModel, StateProvince>()
                .ForMember(dest => dest.Country, mo => mo.Ignore());

            #endregion

            #region City

            Mapper.CreateMap<City, CityModel>()
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<CityModel, City>()
                .ForMember(dest => dest.StateProvince, mo => mo.Ignore());

            #endregion

            #region Language

            Mapper.CreateMap<Language, LanguageModel>()
                .ForMember(dest => dest.AvailableCurrencies, mo => mo.Ignore())
                .ForMember(dest => dest.FlagFileNames, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<LanguageModel, Language>()
                .ForMember(dest => dest.LocaleStringResources, mo => mo.Ignore());

            #endregion

            #region Email account
            Mapper.CreateMap<EmailAccount, EmailAccountModel>()
                .ForMember(dest => dest.Password, mo => mo.Ignore())
                .ForMember(dest => dest.IsDefaultEmailAccount, mo => mo.Ignore())
                .ForMember(dest => dest.SendTestEmailTo, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<EmailAccountModel, EmailAccount>()
                .ForMember(dest => dest.Password, mo => mo.Ignore());
            #endregion

            #region message template
            Mapper.CreateMap<MessageTemplate, MessageTemplateModel>()
                .ForMember(dest => dest.AllowedTokens, mo => mo.Ignore())
                .ForMember(dest => dest.HasAttachedDownload, mo => mo.Ignore())
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableEmailAccounts, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                .ForMember(dest => dest.ListOfStores, mo => mo.Ignore())
                .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<MessageTemplateModel, MessageTemplate>();
            #endregion

            #region Queued Email

            Mapper.CreateMap<QueuedEmail, QueuedEmailModel>()
                .ForMember(dest => dest.EmailAccountName, mo => mo.MapFrom(src => src.EmailAccount != null ? src.EmailAccount.FriendlyName : string.Empty))
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.PriorityName, mo => mo.Ignore())
                .ForMember(dest => dest.SentOn, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<QueuedEmailModel, QueuedEmail>()
                .ForMember(dest => dest.Priority, dt => dt.Ignore())
                .ForMember(dest => dest.PriorityId, dt => dt.Ignore())
                .ForMember(dest => dest.CreatedOnUtc, dt => dt.Ignore())
                .ForMember(dest => dest.SentOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.EmailAccount, mo => mo.Ignore())
                .ForMember(dest => dest.EmailAccountId, mo => mo.Ignore())
                .ForMember(dest => dest.AttachmentFilePath, mo => mo.Ignore())
                .ForMember(dest => dest.AttachmentFileName, mo => mo.Ignore())
                .ForMember(dest => dest.AttachedDownloadId, mo => mo.Ignore());

            #endregion

            #region Customer roles

            Mapper.CreateMap<CustomerRole, CustomerRoleModel>()
                //.ForMember(dest => dest.PurchasedWithProductName, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<CustomerRoleModel, CustomerRole>()
                .ForMember(dest => dest.PermissionRecords, mo => mo.Ignore());

            #endregion

            #region customer attributes
            Mapper.CreateMap<CustomerAttribute, CustomerAttributeModel>()
                .ForMember(dest => dest.AttributeControlTypeName, mo => mo.Ignore())
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<CustomerAttributeModel, CustomerAttribute>()
                .ForMember(dest => dest.AttributeControlType, mo => mo.Ignore())
                .ForMember(dest => dest.CustomerAttributeValues, mo => mo.Ignore());
            #endregion

            #region address attributes

            Mapper.CreateMap<AddressAttribute, AddressAttributeModel>()
                .ForMember(dest => dest.AttributeControlTypeName, mo => mo.Ignore())
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<AddressAttributeModel, AddressAttribute>()
                .ForMember(dest => dest.AttributeControlType, mo => mo.Ignore())
                .ForMember(dest => dest.AddressAttributeValues, mo => mo.Ignore());

            #endregion

            #region logs
            Mapper.CreateMap<Log, LogModel>()
                .ForMember(dest => dest.CustomerEmail, mo => mo.Ignore())
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<LogModel, Log>()
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.LogLevelId, mo => mo.Ignore())
                .ForMember(dest => dest.Customer, mo => mo.Ignore());
            #endregion

            #region ActivityLogType
            Mapper.CreateMap<ActivityLogTypeModel, ActivityLogType>()
                .ForMember(dest => dest.SystemKeyword, mo => mo.Ignore());
            Mapper.CreateMap<ActivityLogType, ActivityLogTypeModel>()
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<ActivityLog, ActivityLogModel>()
                .ForMember(dest => dest.ActivityLogTypeName, mo => mo.MapFrom(src => src.ActivityLogType.Name))
                .ForMember(dest => dest.CustomerEmail, mo => mo.MapFrom(src => src.Customer.Email))
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            #endregion

            #region Setting

            #region CustomerUser

            Mapper.CreateMap<CustomerSettings, CustomerUserSettingsModel.CustomerSettingsModel>()
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<CustomerUserSettingsModel.CustomerSettingsModel, CustomerSettings>()
                .ForMember(dest => dest.HashedPasswordFormat, mo => mo.Ignore())
                .ForMember(dest => dest.OnlineCustomerMinutes, mo => mo.Ignore())
                .ForMember(dest => dest.SuffixDeletedCustomers, mo => mo.Ignore());
            Mapper.CreateMap<AddressSettings, CustomerUserSettingsModel.AddressSettingsModel>()
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<CustomerUserSettingsModel.AddressSettingsModel, AddressSettings>();

            #endregion

            #region LetterSetting

            Mapper.CreateMap<SequenceIdsSettings, SequenceIdsSettingsModel>()
                   .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<SequenceIdsSettingsModel, SequenceIdsSettings>();
            #endregion

            #region PaymentSetting

            Mapper.CreateMap<ContributionSettings, ContributionSettingsModel>()
                   .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<ContributionSettingsModel, ContributionSettings>();

            #endregion

            #region BankSetting

            Mapper.CreateMap<BankSettings, BankSettingsModel>()
                   .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<BankSettingsModel, BankSettings>();

            #endregion

            #region LoanSettings

            Mapper.CreateMap<LoanSettings, LoanSettingsModel>()
                   .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<LoanSettingsModel, LoanSettings>();

            #endregion

            #region ScheduleBatchSettings

            Mapper.CreateMap<ScheduleBatchsSetting, ScheduleBatchSettingsModel>()
                   .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<ScheduleBatchSettingsModel, ScheduleBatchsSetting>();

            #endregion

            #region Signature

            Mapper.CreateMap<SignatureSettings, SignatureSettingsModel>()
                   .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<SignatureSettingsModel, SignatureSettings>();

            #endregion

            #endregion

            #region currencies
            Mapper.CreateMap<Currency, CurrencyModel>()
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                //.ForMember(dest => dest.IsPrimaryExchangeRateCurrency, mo => mo.Ignore())
                //.ForMember(dest => dest.IsPrimaryStoreCurrency, mo => mo.Ignore())
                //.ForMember(dest => dest.Locales, mo => mo.Ignore())
                //.ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                //.ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<CurrencyModel, Currency>()
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore());
            #endregion

            #region contribution
            Mapper.CreateMap<Contribution, ContributionModel>()
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<ContributionModel, Contribution>()
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore());
            #endregion

            #region contributionPayment
            Mapper.CreateMap<ContributionPayment, ContributionPaymentsModel>()
                .ForMember(dest => dest.ScheduledDateOn, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<ContributionPaymentsModel, ContributionPayment>()
                .ForMember(dest => dest.ScheduledDateOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.ProcessedDateOnUtc, mo => mo.Ignore());
            #endregion

            #region check
            Mapper.CreateMap<Check, CheckModel>()
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<CheckModel, Check>()
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore());
            #endregion

            #region loan
            Mapper.CreateMap<Loan, LoanModel>()
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<LoanModel, Loan>()
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore());
            #endregion

            #region loanPayment
            Mapper.CreateMap<LoanPayment, LoanPaymentsModel>()
                .ForMember(dest => dest.ScheduledDateOn, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<LoanPaymentsModel, LoanPayment>()
                .ForMember(dest => dest.ScheduledDateOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.ProcessedDateOnUtc, mo => mo.Ignore());
            #endregion

            #region returnPayment
            Mapper.CreateMap<ReturnPayment, ReturnPaymentModel>()
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<ReturnPaymentModel, ReturnPayment>()
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore());
            #endregion

            #region tab
            Mapper.CreateMap<Tab, TabModel>()
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<TabModel, Tab>()
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore());
            #endregion

            #region tabDetail
            Mapper.CreateMap<TabDetail, TabDetailModel>()
                 .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<TabDetailModel, TabDetail>()
                 .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore());
            #endregion

            #region benefit
            Mapper.CreateMap<Benefit, BenefitModel>()
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<BenefitModel, Benefit>()
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore());
            #endregion

            #region contributionbenefit
            Mapper.CreateMap<ContributionBenefit, ContributionBenefitModel>()
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<ContributionBenefitModel, ContributionBenefit>()
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore());
            #endregion

            #region contributionbenefitbank
            Mapper.CreateMap<ContributionBenefitBank, ContributionBenefitBankModel>()
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.ApprovedOn, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<ContributionBenefitBankModel, ContributionBenefitBank>()
                .ForMember(dest => dest.ApprovedOnUtc, mo => mo.Ignore())
                .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore());
            #endregion

            #region measure weights

            Mapper.CreateMap<MeasureWeight, MeasureWeightModel>()
                .ForMember(dest => dest.IsPrimaryWeight, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<MeasureWeightModel, MeasureWeight>();

            #endregion

            #region measure dimensions
            Mapper.CreateMap<MeasureDimension, MeasureDimensionModel>()
                .ForMember(dest => dest.IsPrimaryDimension, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<MeasureDimensionModel, MeasureDimension>();
            #endregion

            #region KsSystem

            Mapper.CreateMap<KsSystem, KsSystemModel>()
                .ForMember(dest => dest.AvailableLanguages, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<KsSystemModel, KsSystem>();

            #endregion

            #region ScheduleBatchs

            Mapper.CreateMap<ScheduleBatch, ScheduleBatchModel>()
                .ForMember(dest => dest.NextExecutionOn, mo => mo.Ignore())
                .ForMember(dest => dest.StartExecutionOn, mo => mo.Ignore())
                .ForMember(dest => dest.LastExecutionOn, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            Mapper.CreateMap<ScheduleBatchModel, ScheduleBatch>()
                .ForMember(dest => dest.StartExecutionOnUtc, et => et.Ignore())
                .ForMember(dest => dest.NextExecutionOnUtc, et => et.Ignore())
                .ForMember(dest => dest.LastExecutionOnUtc, et => et.Ignore());

            #endregion
        }

        public int Order
        {
            get { return 0; }
        }
    }
}