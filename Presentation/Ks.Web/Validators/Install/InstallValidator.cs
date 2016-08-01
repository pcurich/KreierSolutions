using FluentValidation;
using Ks.Web.Framework.Validators;
using Ks.Web.Infrastructure.Installation;
using Ks.Web.Models.Install;

namespace Ks.Web.Validators.Install
{
    public class InstallValidator: BaseKsValidator<InstallModel>
    {
        public InstallValidator(IInstallationLocalizationService locService)
        {
            RuleFor(x => x.AdminEmail).NotEmpty().WithMessage(locService.GetResource("AdminEmailRequired"));
            RuleFor(x => x.AdminEmail).EmailAddress();
            RuleFor(x => x.AdminPassword).NotEmpty().WithMessage(locService.GetResource("AdminPasswordRequired"));
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage(locService.GetResource("ConfirmPasswordRequired"));
            RuleFor(x => x.AdminPassword).Equal(x => x.ConfirmPassword).WithMessage(locService.GetResource("PasswordsDoNotMatch"));
            RuleFor(x => x.DataProvider).NotEmpty().WithMessage(locService.GetResource("DataProviderRequired"));
        }
    }
}