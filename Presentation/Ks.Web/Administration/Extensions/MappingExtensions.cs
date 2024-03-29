﻿using System;
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
using Ks.Core.Domain.Catalog;
using Ks.Core.Domain.Common;
using Ks.Core.Domain.Contract;
using Ks.Core.Domain.Customers;
using Ks.Core.Domain.Directory;
using Ks.Core.Domain.Localization;
using Ks.Core.Domain.Logging;
using Ks.Core.Domain.Messages;
using Ks.Core.Domain.System;
using Ks.Services.Common;

namespace Ks.Admin.Extensions
{
    public static class MappingExtensions
    {
        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return Mapper.Map(source, destination);
        }

        #region Admin

        public static WorkFlowModel ToModel(this WorkFlow entity)
        {
            return entity.MapTo<WorkFlow, WorkFlowModel>();
        }

        public static WorkFlow ToEntity(this WorkFlowModel model)
        {
            return model.MapTo<WorkFlowModel, WorkFlow>();
        }

        public static WorkFlow ToEntity(this WorkFlowModel model, WorkFlow destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Languages

        public static LanguageModel ToModel(this Language entity)
        {
            return entity.MapTo<Language, LanguageModel>();
        }

        public static Language ToEntity(this LanguageModel model)
        {
            return model.MapTo<LanguageModel, Language>();
        }

        public static Language ToEntity(this LanguageModel model, Language destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Email account

        public static EmailAccountModel ToModel(this EmailAccount entity)
        {
            return entity.MapTo<EmailAccount, EmailAccountModel>();
        }

        public static EmailAccount ToEntity(this EmailAccountModel model)
        {
            return model.MapTo<EmailAccountModel, EmailAccount>();
        }

        public static EmailAccount ToEntity(this EmailAccountModel model, EmailAccount destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Customer roles


        public static CustomerRoleModel ToModel(this CustomerRole entity)
        {
            return entity.MapTo<CustomerRole, CustomerRoleModel>();
        }

        public static CustomerRole ToEntity(this CustomerRoleModel model)
        {
            return model.MapTo<CustomerRoleModel, CustomerRole>();
        }

        public static CustomerRole ToEntity(this CustomerRoleModel model, CustomerRole destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Customer attributes

        public static CustomerAttributeModel ToModel(this CustomerAttribute entity)
        {
            return entity.MapTo<CustomerAttribute, CustomerAttributeModel>();
        }

        public static CustomerAttribute ToEntity(this CustomerAttributeModel model)
        {
            return model.MapTo<CustomerAttributeModel, CustomerAttribute>();
        }

        public static CustomerAttribute ToEntity(this CustomerAttributeModel model, CustomerAttribute destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Address

        public static AddressModel ToModel(this Address entity)
        {
            return entity.MapTo<Address, AddressModel>();
        }

        public static Address ToEntity(this AddressModel model)
        {
            return model.MapTo<AddressModel, Address>();
        }

        public static Address ToEntity(this AddressModel model, Address destination)
        {
            return model.MapTo(destination);
        }

        public static void PrepareCustomAddressAttributes(this AddressModel model,
            Address address,
            IAddressAttributeService addressAttributeService,
            IAddressAttributeParser addressAttributeParser)
        {
            //this method is very similar to the same one in Nop.Web project
            if (addressAttributeService == null)
                throw new ArgumentNullException("addressAttributeService");

            if (addressAttributeParser == null)
                throw new ArgumentNullException("addressAttributeParser");

            var attributes = addressAttributeService.GetAllAddressAttributes();
            foreach (var attribute in attributes)
            {
                var attributeModel = new AddressModel.AddressAttributeModel
                {
                    Id = attribute.Id,
                    Name = attribute.Name,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = addressAttributeService.GetAddressAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new AddressModel.AddressAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.Name,
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(attributeValueModel);
                    }
                }

                //set already selected attributes
                var selectedAddressAttributes = address != null ? address.CustomAttributes : null;
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                        {
                            if (!String.IsNullOrEmpty(selectedAddressAttributes))
                            {
                                //clear default selection
                                foreach (var item in attributeModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedValues = addressAttributeParser.ParseAddressAttributeValues(selectedAddressAttributes);
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
                        {
                            if (!String.IsNullOrEmpty(selectedAddressAttributes))
                            {
                                var enteredText = addressAttributeParser.ParseValues(selectedAddressAttributes, attribute.Id);
                                if (enteredText.Count > 0)
                                    attributeModel.DefaultValue = enteredText[0];
                            }
                        }
                        break;
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.FileUpload:
                    default:
                        //not supported attribute control types
                        break;
                }

                model.CustomAddressAttributes.Add(attributeModel);
            }
        }

        #endregion

        #region Address attributes

        public static AddressAttributeModel ToModel(this AddressAttribute entity)
        {
            return entity.MapTo<AddressAttribute, AddressAttributeModel>();
        }

        public static AddressAttribute ToEntity(this AddressAttributeModel model)
        {
            return model.MapTo<AddressAttributeModel, AddressAttribute>();
        }

        public static AddressAttribute ToEntity(this AddressAttributeModel model, AddressAttribute destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Contribution

        public static ContributionModel ToModel(this Contribution entity)
        {
            return entity.MapTo<Contribution, ContributionModel>();
        }

        public static Contribution ToEntity(this ContributionModel model)
        {
            return model.MapTo<ContributionModel, Contribution>();
        }

        public static Contribution ToEntity(this ContributionModel model, Contribution destination)
        {
            return model.MapTo(destination);
        }


        #endregion

        #region ContributionPayments

        public static ContributionPaymentsModel ToModel(this ContributionPayment entity)
        {
            return entity.MapTo<ContributionPayment, ContributionPaymentsModel>();
        }

        public static ContributionPayment ToEntity(this ContributionPaymentsModel model)
        {
            return model.MapTo<ContributionPaymentsModel, ContributionPayment>();
        }

        public static ContributionPayment ToEntity(this ContributionPaymentsModel model, ContributionPayment destination)
        {
            return model.MapTo(destination);
        }


        #endregion

        #region Check

        public static CheckModel ToModel(this Check entity)
        {
            return entity.MapTo<Check, CheckModel>();
        }

        public static Check ToEntity(this CheckModel model)
        {
            return model.MapTo<CheckModel, Check>();
        }

        public static Check ToEntity(this CheckModel model, Check destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Loan

        public static LoanModel ToModel(this Loan entity)
        {
            return entity.MapTo<Loan, LoanModel>();
        }

        public static Loan ToEntity(this LoanModel model)
        {
            return model.MapTo<LoanModel, Loan>();
        }

        public static Loan ToEntity(this LoanModel model, Loan destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region LoanPayments

        public static LoanPaymentsModel ToModel(this LoanPayment entity)
        {
            return entity.MapTo<LoanPayment, LoanPaymentsModel>();
        }

        public static LoanPayment ToEntity(this LoanPaymentsModel model)
        {
            return model.MapTo<LoanPaymentsModel, LoanPayment>();
        }

        public static LoanPayment ToEntity(this LoanPaymentsModel model, LoanPayment destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region ReturnPayment

        public static ReturnPaymentModel ToModel(this ReturnPayment entity)
        {
            return entity.MapTo<ReturnPayment, ReturnPaymentModel>();
        }

        public static ReturnPayment ToEntity(this ReturnPaymentModel model)
        {
            return model.MapTo<ReturnPaymentModel, ReturnPayment>();
        }

        public static ReturnPayment ToEntity(this ReturnPaymentModel model, ReturnPayment destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Tab

        public static TabModel ToModel(this Tab entity)
        {
            return entity.MapTo<Tab, TabModel>();
        }

        public static Tab ToEntity(this TabModel model)
        {
            return model.MapTo<TabModel, Tab>();
        }

        public static Tab ToEntity(this TabModel model, Tab destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region TabDetail

        public static TabDetailModel ToModel(this TabDetail entity)
        {
            return entity.MapTo<TabDetail, TabDetailModel>();
        }

        public static TabDetail ToEntity(this TabDetailModel model)
        {
            return model.MapTo<TabDetailModel, TabDetail>();
        }

        public static TabDetail ToEntity(this TabDetailModel model, TabDetail destination)
        {
            return model.MapTo(destination);
        }


        #endregion

        #region Benefit

        public static BenefitModel ToModel(this Benefit entity)
        {
            return entity.MapTo<Benefit, BenefitModel>();
        }

        public static Benefit ToEntity(this BenefitModel model)
        {
            return model.MapTo<BenefitModel, Benefit>();
        }

        public static Benefit ToEntity(this BenefitModel model, Benefit destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region ContributionBenefit

        public static ContributionBenefitModel ToModel(this ContributionBenefit entity)
        {
            return entity.MapTo<ContributionBenefit, ContributionBenefitModel>();
        }

        public static ContributionBenefit ToEntity(this ContributionBenefitModel model)
        {
            return model.MapTo<ContributionBenefitModel, ContributionBenefit>();
        }

        public static ContributionBenefit ToEntity(this ContributionBenefitModel model, ContributionBenefit destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region ContributionBenefitModelBank

        public static ContributionBenefitBankModel ToModel(this ContributionBenefitBank entity)
        {
            return entity.MapTo<ContributionBenefitBank, ContributionBenefitBankModel>();
        }

        public static ContributionBenefitBank ToEntity(this ContributionBenefitBankModel model)
        {
            return model.MapTo<ContributionBenefitBankModel, ContributionBenefitBank>();
        }

        public static ContributionBenefitBank ToEntity(this ContributionBenefitBankModel model, ContributionBenefitBank destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Log

        public static LogModel ToModel(this Log entity)
        {
            return entity.MapTo<Log, LogModel>();
        }

        public static Log ToEntity(this LogModel model)
        {
            return model.MapTo<LogModel, Log>();
        }

        public static Log ToEntity(this LogModel model, Log destination)
        {
            return model.MapTo(destination);
        }

        public static ActivityLogTypeModel ToModel(this ActivityLogType entity)
        {
            return entity.MapTo<ActivityLogType, ActivityLogTypeModel>();
        }

        public static ActivityLogModel ToModel(this ActivityLog entity)
        {
            return entity.MapTo<ActivityLog, ActivityLogModel>();
        }

        #endregion

        #region Setting

        #region CustomerUser

        public static CustomerUserSettingsModel.CustomerSettingsModel ToModel(this CustomerSettings entity)
        {
            return entity.MapTo<CustomerSettings, CustomerUserSettingsModel.CustomerSettingsModel>();
        }
        public static CustomerSettings ToEntity(this CustomerUserSettingsModel.CustomerSettingsModel model, CustomerSettings destination)
        {
            return model.MapTo(destination);
        }
        public static CustomerUserSettingsModel.AddressSettingsModel ToModel(this AddressSettings entity)
        {
            return entity.MapTo<AddressSettings, CustomerUserSettingsModel.AddressSettingsModel>();
        }
        public static AddressSettings ToEntity(this CustomerUserSettingsModel.AddressSettingsModel model, AddressSettings destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region BankSetting

        public static BankSettingsModel ToModel(this BankSettings entity)
        {
            return entity.MapTo<BankSettings, BankSettingsModel>();
        }
        public static BankSettings ToEntity(this BankSettingsModel model)
        {
            return model.MapTo<BankSettingsModel, BankSettings>();
        }

        public static BankSettings ToEntity(this BankSettingsModel model, BankSettings destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region LetterSetting

        public static SequenceIdsSettingsModel ToModel(this SequenceIdsSettings entity)
        {
            return entity.MapTo<SequenceIdsSettings, SequenceIdsSettingsModel>();
        }
        public static SequenceIdsSettings ToEntity(this SequenceIdsSettingsModel model)
        {
            return model.MapTo<SequenceIdsSettingsModel, SequenceIdsSettings>();
        }

        public static SequenceIdsSettings ToEntity(this SequenceIdsSettingsModel model, SequenceIdsSettings destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region PaymentSetting

        public static ContributionSettingsModel ToModel(this ContributionSettings entity)
        {
            return entity.MapTo<ContributionSettings, ContributionSettingsModel>();
        }
        public static ContributionSettings ToEntity(this ContributionSettingsModel model)
        {
            return model.MapTo<ContributionSettingsModel, ContributionSettings>();
        }

        public static ContributionSettings ToEntity(this ContributionSettingsModel model, ContributionSettings destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region LoanSettings

        public static LoanSettingsModel ToModel(this LoanSettings entity)
        {
            return entity.MapTo<LoanSettings, LoanSettingsModel>();
        }
        public static LoanSettings ToEntity(this LoanSettingsModel model)
        {
            return model.MapTo<LoanSettingsModel, LoanSettings>();
        }

        public static LoanSettings ToEntity(this LoanSettingsModel model, LoanSettings destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region ScheduleBatch

        public static ScheduleBatchSettingsModel ToModel(this ScheduleBatchsSetting entity)
        {
            return entity.MapTo<ScheduleBatchsSetting, ScheduleBatchSettingsModel>();
        }
        public static ScheduleBatchsSetting ToEntity(this ScheduleBatchSettingsModel model)
        {
            return model.MapTo<ScheduleBatchSettingsModel, ScheduleBatchsSetting>();
        }

        public static ScheduleBatchsSetting ToEntity(this ScheduleBatchSettingsModel model, ScheduleBatchsSetting destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Signature

        public static SignatureSettingsModel ToModel(this SignatureSettings entity)
        {
            return entity.MapTo<SignatureSettings, SignatureSettingsModel>();
        }
        public static SignatureSettings ToEntity(this SignatureSettingsModel model)
        {
            return model.MapTo<SignatureSettingsModel, SignatureSettings>();
        }

        public static SignatureSettings ToEntity(this SignatureSettingsModel model, SignatureSettings destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #endregion

        #region Countries / states / cities

        public static CountryModel ToModel(this Country entity)
        {
            return entity.MapTo<Country, CountryModel>();
        }

        public static Country ToEntity(this CountryModel model)
        {
            return model.MapTo<CountryModel, Country>();
        }

        public static Country ToEntity(this CountryModel model, Country destination)
        {
            return model.MapTo(destination);
        }

        public static StateProvinceModel ToModel(this StateProvince entity)
        {
            return entity.MapTo<StateProvince, StateProvinceModel>();
        }

        public static StateProvince ToEntity(this StateProvinceModel model)
        {
            return model.MapTo<StateProvinceModel, StateProvince>();
        }

        public static StateProvince ToEntity(this StateProvinceModel model, StateProvince destination)
        {
            return model.MapTo(destination);
        }

        public static CityModel ToModel(this City entity)
        {
            return entity.MapTo<City, CityModel>();
        }

        public static City ToEntity(this CityModel model)
        {
            return model.MapTo<CityModel, City>();
        }

        public static City ToEntity(this CityModel model, City destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Message templates

        public static MessageTemplateModel ToModel(this MessageTemplate entity)
        {
            return entity.MapTo<MessageTemplate, MessageTemplateModel>();
        }

        public static MessageTemplate ToEntity(this MessageTemplateModel model)
        {
            return model.MapTo<MessageTemplateModel, MessageTemplate>();
        }

        public static MessageTemplate ToEntity(this MessageTemplateModel model, MessageTemplate destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Queued email

        public static QueuedEmailModel ToModel(this QueuedEmail entity)
        {
            return entity.MapTo<QueuedEmail, QueuedEmailModel>();
        }

        public static QueuedEmail ToEntity(this QueuedEmailModel model)
        {
            return model.MapTo<QueuedEmailModel, QueuedEmail>();
        }

        public static QueuedEmail ToEntity(this QueuedEmailModel model, QueuedEmail destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Systems

        public static KsSystemModel ToModel(this KsSystem entity)
        {
            return entity.MapTo<KsSystem, KsSystemModel>();
        }

        public static KsSystem ToEntity(this KsSystemModel model)
        {
            return model.MapTo<KsSystemModel, KsSystem>();
        }

        public static KsSystem ToEntity(this KsSystemModel model, KsSystem destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region ScheduleBatchs

        public static ScheduleBatchModel ToModel(this ScheduleBatch entity)
        {
            return entity.MapTo<ScheduleBatch, ScheduleBatchModel>();
        }

        public static ScheduleBatch ToEntity(this ScheduleBatchModel model)
        {
            return model.MapTo<ScheduleBatchModel, ScheduleBatch>();
        }

        public static ScheduleBatch ToEntity(this ScheduleBatchModel model, ScheduleBatch destination)
        {
            return model.MapTo(destination);
        }

        #endregion
    }
}