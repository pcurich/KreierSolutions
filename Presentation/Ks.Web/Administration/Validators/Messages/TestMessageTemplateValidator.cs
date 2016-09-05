using FluentValidation;
using Ks.Admin.Models.Messages;
using Ks.Services.Localization;
using Ks.Web.Framework.Validators;

namespace Ks.Admin.Validators.Messages
{
    public class TestMessageTemplateValidator : BaseKsValidator<TestMessageTemplateModel>
    {
        public TestMessageTemplateValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.SendTo).NotEmpty();
            RuleFor(x => x.SendTo).EmailAddress().WithMessage(localizationService.GetResource("Admin.Common.WrongEmail"));
        }
    }
}