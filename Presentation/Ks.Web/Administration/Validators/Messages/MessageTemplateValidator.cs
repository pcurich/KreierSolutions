using FluentValidation;
using Ks.Admin.Models.Messages;
using Ks.Services.Localization;
using Ks.Web.Framework.Validators;

namespace Ks.Admin.Validators.Messages
{
    public class MessageTemplateValidator : BaseKsValidator<MessageTemplateModel>
    {
        public MessageTemplateValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Subject).NotEmpty().WithMessage(localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Fields.Subject.Required"));
            RuleFor(x => x.Body).NotEmpty().WithMessage(localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Fields.Body.Required"));
        }
    }
}