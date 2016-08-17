using FluentValidation;
using Ks.Admin.Models.Directory;
using Ks.Services.Localization;
using Ks.Web.Framework.Validators;

namespace Ks.Admin.Validators.Directory
{
    public class StateProvinceValidator : BaseKsValidator<StateProvinceModel>
    {
        public StateProvinceValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Countries.States.Fields.Name.Required"));
        }
    }
}