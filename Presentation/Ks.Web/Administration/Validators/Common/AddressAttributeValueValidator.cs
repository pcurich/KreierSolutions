using FluentValidation;
using Ks.Admin.Models.Common;
using Ks.Services.Localization;
using Ks.Web.Framework.Validators;

namespace Ks.Admin.Validators.Common
{
    public class AddressAttributeValueValidator : BaseKsValidator<AddressModel.AddressAttributeValueModel>
    {
        public AddressAttributeValueValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Address.AddressAttributes.Values.Fields.Name.Required"));
        }
    }
}