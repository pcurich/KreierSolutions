using FluentValidation.Results;
using Ks.Admin.Models.Settings;
using Ks.Services.Localization;
using Ks.Web.Framework.Validators;

namespace Ks.Admin.Validators.Settings
{
    public class BankSettingsModelValidator : BaseKsValidator<BankSettingsModel>
    {
        public BankSettingsModelValidator(ILocalizationService localizationService)
        {
            Custom(x =>
            {
                if (x.IsActive1)
                {
                    if (string.IsNullOrWhiteSpace(x.NameBank1))
                        return new ValidationFailure("NameBank1",
                            localizationService.GetResource(
                                "Admin.Configuration.Settings.BankSettings.Fields.NameBank.Required"));
                    if (string.IsNullOrWhiteSpace(x.AccountNumber1))
                        return new ValidationFailure("AccountNumber1",
                            localizationService.GetResource(
                                "Admin.Configuration.Settings.BankSettings.Fields.AccountNumber.Required"));
                }

                if (x.IsActive2)
                {
                    if (string.IsNullOrWhiteSpace(x.NameBank2))
                        return new ValidationFailure("NameBank2",
                            localizationService.GetResource(
                                "Admin.Configuration.Settings.BankSettings.Fields.NameBank.Required"));
                    if (string.IsNullOrWhiteSpace(x.AccountNumber2))
                        return new ValidationFailure("AccountNumber2",
                            localizationService.GetResource(
                                "Admin.Configuration.Settings.BankSettings.Fields.AccountNumber.Required"));
                }

                if (x.IsActive3)
                {
                    if (string.IsNullOrWhiteSpace(x.NameBank3))
                        return new ValidationFailure("NameBank3",
                            localizationService.GetResource(
                                "Admin.Configuration.Settings.BankSettings.Fields.NameBank.Required"));
                    if (string.IsNullOrWhiteSpace(x.AccountNumber3))
                        return new ValidationFailure("AccountNumber3",
                            localizationService.GetResource(
                                "Admin.Configuration.Settings.BankSettings.Fields.AccountNumber.Required"));
                }

                if (x.IsActive4)
                {
                    if (string.IsNullOrWhiteSpace(x.NameBank4))
                        return new ValidationFailure("NameBank4",
                            localizationService.GetResource(
                                "Admin.Configuration.Settings.BankSettings.Fields.NameBank.Required"));
                    if (string.IsNullOrWhiteSpace(x.AccountNumber4))
                        return new ValidationFailure("AccountNumber4",
                            localizationService.GetResource(
                                "Admin.Configuration.Settings.BankSettings.Fields.AccountNumber.Required"));
                }

                if (x.IsActive5)
                {
                    if (string.IsNullOrWhiteSpace(x.NameBank5))
                        return new ValidationFailure("NameBank5",
                            localizationService.GetResource(
                                "Admin.Configuration.Settings.BankSettings.Fields.NameBank.Required"));
                    if (string.IsNullOrWhiteSpace(x.AccountNumber5))
                        return new ValidationFailure("AccountNumber5",
                            localizationService.GetResource(
                                "Admin.Configuration.Settings.BankSettings.Fields.AccountNumber.Required"));
                }

                return null;
            });
        }
    }
}
 