using FluentValidation;
using FluentValidation.Results;
using Ks.Admin.Models.Contract;
using Ks.Services.Contract;
using Ks.Services.Localization;
using Ks.Web.Framework.Validators;

namespace Ks.Admin.Validators.Contract
{
    public class LoanPaymentValidator : BaseKsValidator<LoanPaymentsModel>
    {
        public LoanPaymentValidator(ILocalizationService localizationService, ILoanService loanService)
        {
            Custom(x =>
            {
                if (!loanService.IsPaymentValid(x.AccountNumber, x.TransactionNumber))
                    return new ValidationFailure("TransactionNumber", localizationService.GetResource("Admin.Contract.LoanPayments.Fields.TransactionNumber.NoValid"));
                return null;
            });

            RuleFor(x => x.TransactionNumber)
                .NotEmpty()
                .WithMessage(
                    localizationService.GetResource("Admin.Contract.LoanPayments.Fields.TransactionNumber.IsRequired"));
        }
    }
}