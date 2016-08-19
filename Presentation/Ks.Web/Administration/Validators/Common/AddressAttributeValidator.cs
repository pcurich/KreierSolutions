using FluentValidation;
using Ks.Admin.Models.Common;
using Ks.Services.Localization;
using Ks.Web.Framework.Validators;

namespace Ks.Admin.Validators.Common
{
    public class AddressAttributeValidator : BaseKsValidator<AddressAttributeModel>
    {
        public AddressAttributeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Address.AddressAttributes.Fields.Name.Required"));
        }
    }
}