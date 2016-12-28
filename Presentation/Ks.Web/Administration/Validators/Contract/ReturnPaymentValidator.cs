using FluentValidation;
using Ks.Admin.Models.Contract;
using Ks.Services.Localization;
using Ks.Web.Framework.Validators;

namespace Ks.Admin.Validators.Contract
{
    public class ReturnPaymentValidator : BaseKsValidator<ReturnPaymentModel>
    {
        public ReturnPaymentValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.BankName)
                .NotEmpty()
                .WithMessage("Ingrese un banco antes de guardar");

            RuleFor(x => x.AccountNumber)
                .NotEmpty()
                .WithMessage("Ingrese un numero de cuenta antes de guardar");

            RuleFor(x => x.CheckNumber)
                .NotEmpty()
                .WithMessage("Ingrese un numero de cheque antes de guardar");
        }
    }
}