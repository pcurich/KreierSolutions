using FluentValidation;
using Ks.Admin.Models.Messages;
using Ks.Services.Localization;
using Ks.Web.Framework.Validators;

namespace Ks.Admin.Validators.Messages
{
    public class EmailAccountValidator : BaseKsValidator<EmailAccountModel>
    {
        public EmailAccountValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Admin.Common.WrongEmail"));

            RuleFor(x => x.DisplayName).NotEmpty();
        }
    }
}