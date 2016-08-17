using FluentValidation;
using Ks.Admin.Models.Customers;
using Ks.Services.Localization;
using Ks.Web.Framework.Validators;

namespace Ks.Admin.Validators.Customers
{
    public class CustomerRoleValidator : BaseKsValidator<CustomerRoleModel>
    {
        public CustomerRoleValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Customers.CustomerRoles.Fields.Name.Required"));
        }
    }

}
