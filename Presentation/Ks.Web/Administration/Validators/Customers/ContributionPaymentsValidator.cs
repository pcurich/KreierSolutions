using FluentValidation;
using Ks.Admin.Models.Customers;
using Ks.Services.Localization;
using Ks.Web.Framework.Validators;

namespace Ks.Admin.Validators.Customers
{
    public class ContributionPaymentsValidator : BaseKsValidator<ContributionPaymentsModel>
    {
        public ContributionPaymentsValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.AccountNumber).NotEmpty().WithMessage(localizationService.GetResource("Admin.Customers.CustomerAttributes.Fields.AccountNumber.Required"));
            RuleFor(x => x.TransactionNumber).NotEmpty().WithMessage(localizationService.GetResource("Admin.Customers.CustomerAttributes.Fields.TransactionNumber.Required"));
        }

    }
}