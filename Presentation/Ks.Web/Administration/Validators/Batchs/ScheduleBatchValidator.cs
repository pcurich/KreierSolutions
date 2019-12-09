using FluentValidation;
using Ks.Admin.Models.Batchs;
using Ks.Services.Localization;
using Ks.Web.Framework.Validators;

namespace Ks.Admin.Validators.Batchs
{
    public class ScheduleBatchValidator : BaseKsValidator<ScheduleBatchModel>
    {
        public ScheduleBatchValidator(ILocalizationService localizationService)
        {
            //RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.System.ScheduleBatchs.Fields.Name.Required"));
            RuleFor(x => x.PeriodYear).GreaterThan(0).WithMessage(localizationService.GetResource("Admin.System.ScheduleBatchs.Fields.PeriodYear.GreaterThanOrEqualTo1"));
            RuleFor(x => x.PeriodMonth).GreaterThan(0).WithMessage(localizationService.GetResource("Admin.System.ScheduleBatchs.Fields.PeriodMonth.GreaterThanOrEqualTo1"));
        }
    }
}