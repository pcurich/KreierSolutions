using FluentValidation;
using Ks.Admin.Models.Directory;
using Ks.Services.Localization;
using Ks.Web.Framework.Validators;

namespace Ks.Admin.Validators.Directory
{
    public class MeasureWeightValidator : BaseKsValidator<MeasureWeightModel>
    {
        public MeasureWeightValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Measures.Weights.Fields.Name.Required"));
            RuleFor(x => x.SystemKeyword).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Measures.Weights.Fields.SystemKeyword.Required"));
        }
    }
}