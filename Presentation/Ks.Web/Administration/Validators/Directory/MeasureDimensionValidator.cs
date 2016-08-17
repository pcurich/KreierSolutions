using FluentValidation;
using Ks.Admin.Models.Directory;
using Ks.Services.Localization;
using Ks.Web.Framework.Validators;

namespace Ks.Admin.Validators.Directory
{
    public class MeasureDimensionValidator : BaseKsValidator<MeasureDimensionModel>
    {
        public MeasureDimensionValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Measures.Dimensions.Fields.Name.Required"));
            RuleFor(x => x.SystemKeyword).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Measures.Dimensions.Fields.SystemKeyword.Required"));
        }
    }
}