using FluentValidation;
using Ks.Admin.Models.Tasks;
using Ks.Services.Localization;
using Ks.Web.Framework.Validators;

namespace Ks.Admin.Validators.Tasks
{
    public class ScheduleTaskValidator : BaseKsValidator<ScheduleTaskModel>
    {
        public ScheduleTaskValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.System.ScheduleTasks.Name.Required"));
            RuleFor(x => x.Seconds).GreaterThan(0).WithMessage(localizationService.GetResource("Admin.System.ScheduleTasks.Seconds.Positive"));
        }
    }
}