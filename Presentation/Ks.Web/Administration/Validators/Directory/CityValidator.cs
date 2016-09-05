using FluentValidation;
using Ks.Admin.Models.Directory;
using Ks.Services.Localization;
using Ks.Web.Framework.Validators;

namespace Ks.Admin.Validators.Directory
{
    public class CityValidator : BaseKsValidator<CityModel>
    {
        public CityValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Countries.States.City.Fields.Name.Required"));
        }
    }
}