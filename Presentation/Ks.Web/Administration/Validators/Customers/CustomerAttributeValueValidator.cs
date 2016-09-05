using FluentValidation;
using Ks.Admin.Models.Customers;
using Ks.Services.Localization;
using Ks.Web.Framework.Validators;

namespace Ks.Admin.Validators.Customers
{
    public class CustomerAttributeValueValidator : BaseKsValidator<CustomerAttributeValueModel>
    {
        public CustomerAttributeValueValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Customers.CustomerAttributes.Values.Fields.Name.Required"));
        }
    }
}