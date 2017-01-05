using FluentValidation;
using FluentValidation.Results;
using Ks.Admin.Models.Customers;
using Ks.Core.Domain.Customers;
using Ks.Services.Customers;
using Ks.Services.Directory;
using Ks.Services.Localization;
using Ks.Web.Framework.Validators;

namespace Ks.Admin.Validators.Customers
{
    public class CustomerValidator : BaseKsValidator<CustomerModel>
    {
        public CustomerValidator(ILocalizationService localizationService,
            ICustomerService customerService,
            IStateProvinceService stateProvinceService,
            CustomerSettings customerSettings)
        {
            //form fields
            if (customerSettings.CountryEnabled && customerSettings.CountryRequired)
            {
                RuleFor(x => x.CountryId)
                    .NotEqual(0)
                    .WithMessage(localizationService.GetResource("Account.Fields.Country.Required"));
            }
            if (customerSettings.CountryEnabled &&
                customerSettings.StateProvinceEnabled &&
                customerSettings.StateProvinceRequired)
            {
                Custom(x =>
                {
                    //does selected country have states?
                    var hasStates = stateProvinceService.GetStateProvincesByCountryId(x.CountryId).Count > 0;
                    if (hasStates)
                    {
                        //if yes, then ensure that a state is selected
                        if (x.StateProvinceId == 0)
                        {
                            return new ValidationFailure("StateProvinceId",
                                localizationService.GetResource("Account.Fields.StateProvince.Required"));
                        }
                    }

                    return null;
                });
            }
            if (customerSettings.StreetAddressRequired && customerSettings.StreetAddressEnabled)
                RuleFor(x => x.StreetAddress)
                    .NotEmpty()
                    .WithMessage(
                        localizationService.GetResource("Admin.Customers.Customers.Fields.StreetAddress.Required"));
            if (customerSettings.StreetAddress2Required && customerSettings.StreetAddress2Enabled)
                RuleFor(x => x.StreetAddress2)
                    .NotEmpty()
                    .WithMessage(
                        localizationService.GetResource("Admin.Customers.Customers.Fields.StreetAddress2.Required"));
            if (customerSettings.ZipPostalCodeRequired && customerSettings.ZipPostalCodeEnabled)
                RuleFor(x => x.ZipPostalCode)
                    .NotEmpty()
                    .WithMessage(
                        localizationService.GetResource("Admin.Customers.Customers.Fields.ZipPostalCode.Required"));
            if (customerSettings.CityRequired && customerSettings.CityEnabled)
                RuleFor(x => x.City)
                    .NotEmpty()
                    .WithMessage(localizationService.GetResource("Admin.Customers.Customers.Fields.City.Required"));
            if (customerSettings.PhoneRequired && customerSettings.PhoneEnabled)
                RuleFor(x => x.Phone)
                    .NotEmpty()
                    .WithMessage(localizationService.GetResource("Admin.Customers.Customers.Fields.Phone.Required"));
            if (customerSettings.FaxRequired && customerSettings.FaxEnabled)
                RuleFor(x => x.Fax)
                    .NotEmpty()
                    .WithMessage(localizationService.GetResource("Admin.Customers.Customers.Fields.Fax.Required"));

            RuleFor(x => x.MilitarySituationId)
                .GreaterThan(0)
                .WithMessage(
                    localizationService.GetResource("Admin.Customers.Customers.Fields.MilitarySituation.GreaterThanZero"));

            Custom(x =>
            {
                if (x.AdmCode == null || x.AdmCode.Length != 9)
                    return new ValidationFailure("AdmCode",
                        localizationService.GetResource("Account.Fields.AdmCode.Length"));

                if (customerService.GetCustomerByAdmCode(x.AdmCode.Trim()) != null && x.Id==0)
                    return new ValidationFailure("AdmCode",
                        localizationService.GetResource("Account.Fields.AdmCode.IsRegister"));

                if (x.Dni == null || x.Dni.Length != 8)
                    return new ValidationFailure("Dni", localizationService.GetResource("Account.Fields.Dni.Length"));

                if (customerService.GetCustomerByDni(x.Dni.Trim()) != null && x.Id == 0)
                    return new ValidationFailure("Dni",
                        localizationService.GetResource("Account.Fields.Dni.IsRegister"));

                return null;
            });
        }
    }
}
