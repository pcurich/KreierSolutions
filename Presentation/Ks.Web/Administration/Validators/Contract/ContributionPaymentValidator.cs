using FluentValidation;
using FluentValidation.Results;
using Ks.Admin.Models.Contract;
using Ks.Services.Contract;
using Ks.Services.Localization;
using Ks.Web.Framework.Validators;

namespace Ks.Admin.Validators.Contract
{
    public class ContributionPaymentValidator : BaseKsValidator<ContributionPaymentsModel>
    {
        public ContributionPaymentValidator(ILocalizationService localizationService, 
            IContributionService contributionService)
        {
            Custom(x=>
            {
                if(!contributionService.IsPaymentValid(x.AccountNumber,x.TransactionNumber))
                    return new ValidationFailure("TransactionNumber", localizationService.GetResource("Admin.Contract.ContributionPayments.Fields.TransactionNumber.NoValid"));
                return null;
            });

            RuleFor(x => x.TransactionNumber)
                .NotEmpty()
                .WithMessage(
                    localizationService.GetResource("Admin.Contract.ContributionPayments.Fields.TransactionNumber.IsRequired"));
        }
    }
}