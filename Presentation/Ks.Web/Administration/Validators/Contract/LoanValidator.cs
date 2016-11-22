using FluentValidation;
using Ks.Admin.Models.Contract;
using Ks.Services.Localization;
using Ks.Web.Framework.Validators;

namespace Ks.Admin.Validators.Contract
{
    public class LoanValidator : BaseKsValidator<LoanModel>
    {
        public LoanValidator(ILocalizationService localizationService)
        {

            RuleFor(x => x.LoanAmount)
                .GreaterThan(0)
                .WithMessage(
                    localizationService.GetResource("Admin.Contract.Loan.Fields.LoanAmount.GreaterThanOrEqualTo1"));

            RuleFor(x => x.CashFlow)
                .GreaterThan(0)
                .WithMessage(
                    localizationService.GetResource("Admin.Contract.Loan.Fields.CashFlow.GreaterThanOrEqualTo1"));
        }
    }
}