using FluentValidation;
using Ks.Admin.Models.Customers;
using Ks.Services.Localization;
using Ks.Web.Framework.Validators;

namespace Ks.Admin.Validators.Customers
{
    public class CustomerAttributeValidator : BaseKsValidator<CustomerAttributeModel>
    {
        public CustomerAttributeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Customers.CustomerAttributes.Fields.Name.Required"));
        }
    }
}