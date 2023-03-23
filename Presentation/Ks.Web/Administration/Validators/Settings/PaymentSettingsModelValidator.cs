using FluentValidation.Results;
using Ks.Admin.Models.Settings;
using Ks.Services.Localization;
using Ks.Web.Framework.Validators;

namespace Ks.Admin.Validators.Settings
{
    public class PaymentSettingsModelValidator : BaseKsValidator<ContributionSettingsModel>
    {
        public PaymentSettingsModelValidator(ILocalizationService localizationService)
        {
            Custom(x =>
            {
                if (x.IsActiveAmount1)
                {
                    if (string.IsNullOrWhiteSpace(x.NameAmount1))
                        return new ValidationFailure("NameAmount1", localizationService.GetResource("Admin.Configuration.Settings.PaymentSettings.Fields.NameAmount1.Required"));
                    if (x.Amount1 <= 0)
                        return new ValidationFailure("Amount1", localizationService.GetResource("Admin.Configuration.Settings.PaymentSettings.Fields.Amount1.GreaterThanZero"));
                }

                if (x.IsActiveAmount2)
                {
                    if (string.IsNullOrWhiteSpace(x.NameAmount2))
                        return new ValidationFailure("NameAmount2", localizationService.GetResource("Admin.Configuration.Settings.PaymentSettings.Fields.NameAmount2.Required"));
                    if (x.Amount2 <= 0)
                        return new ValidationFailure("Amount2", localizationService.GetResource("Admin.Configuration.Settings.PaymentSettings.Fields.Amount2.GreaterThanZero"));
                }

                if (x.IsActiveAmount3)
                {
                    if (string.IsNullOrWhiteSpace(x.NameAmount3))
                        return new ValidationFailure("NameAmount3", localizationService.GetResource("Admin.Configuration.Settings.PaymentSettings.Fields.NameAmount3.Required"));
                    if (x.Amount3 <= 0)
                        return new ValidationFailure("Amount3", localizationService.GetResource("Admin.Configuration.Settings.PaymentSettings.Fields.Amount3.GreaterThanZero"));
                } 

                if (x.IsActiveAmount4)
                {
                    if (string.IsNullOrWhiteSpace(x.NameAmount4))
                        return new ValidationFailure("NameAmount4", localizationService.GetResource("Admin.Configuration.Settings.PaymentSettings.Fields.NameAmount1.Required"));
                    if (x.Amount4 <= 0)
                        return new ValidationFailure("Amount4", localizationService.GetResource("Admin.Configuration.Settings.PaymentSettings.Fields.Amount1.GreaterThanZero"));
                }

                if (x.IsActiveAmount5)
                {
                    if (string.IsNullOrWhiteSpace(x.NameAmount5))
                        return new ValidationFailure("NameAmount5", localizationService.GetResource("Admin.Configuration.Settings.PaymentSettings.Fields.NameAmount5.Required"));
                    if (x.Amount5 <= 0)
                        return new ValidationFailure("Amount5", localizationService.GetResource("Admin.Configuration.Settings.PaymentSettings.Fields.Amount5.GreaterThanZero"));
                }

                if (x.IsActiveAmount6)
                {
                    if (string.IsNullOrWhiteSpace(x.NameAmount6))
                        return new ValidationFailure("NameAmount6", localizationService.GetResource("Admin.Configuration.Settings.PaymentSettings.Fields.NameAmount6.Required"));
                    if (x.Amount6 <= 0)
                        return new ValidationFailure("Amount6", localizationService.GetResource("Admin.Configuration.Settings.PaymentSettings.Fields.Amount6.GreaterThanZero"));
                }

                return null;
            });

 
        }
    }
}