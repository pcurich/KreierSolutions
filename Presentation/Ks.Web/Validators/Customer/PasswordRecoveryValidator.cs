using FluentValidation;
using Ks.Services.Localization;
using Ks.Web.Framework.Validators;
using Ks.Web.Models.Customer;

namespace Ks.Web.Validators.Customer
{
    public class PasswordRecoveryValidator : BaseKsValidator<PasswordRecoveryModel>
    {
        public PasswordRecoveryValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("Account.PasswordRecovery.Email.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
        }
    }
}