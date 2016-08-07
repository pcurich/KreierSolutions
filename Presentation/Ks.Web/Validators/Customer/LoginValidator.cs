using FluentValidation;
using Ks.Core.Domain.Customers;
using Ks.Services.Localization;
using Ks.Web.Framework.Validators;
using Ks.Web.Models.Customer;

namespace Ks.Web.Validators.Customer
{
    public class LoginValidator : BaseKsValidator<LoginModel>
    {
        public LoginValidator(ILocalizationService localizationService, CustomerSettings customerSettings)
        {
            if (!customerSettings.UsernamesEnabled)
            {
                //login by email
                RuleFor(x => x.Email)
                    .NotEmpty()
                    .WithMessage(localizationService.GetResource("Account.Login.Fields.Email.Required"));
                RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
            }
        }
    }
}