using FluentValidation;
using Ks.Admin.Models.Contract;
using Ks.Web.Framework.Validators;

namespace Ks.Admin.Validators.Contract
{
    public class CheckValidator : BaseKsValidator<CheckModel>
    {
        public CheckValidator()
        {
            RuleFor(x => x.Reason).NotEmpty().WithMessage("Ingrese un motivo por el cual realiza el cambio");
            RuleFor(x => x.CheckNumber).NotEmpty().WithMessage("Ingrese un numero de cheque");
            RuleFor(x => x.AccountNumber).NotEmpty().WithMessage("Seleccione una cuenta valida");
        }
    }
}