using System;
using FluentValidation;
using FluentValidation.Results;
using Ks.Admin.Models.Contract;
using Ks.Services.Localization;
using Ks.Web.Framework.Validators;

namespace Ks.Admin.Validators.Contract
{
    public class ContributionValidator : BaseKsValidator<ContributionModel>
    {
        public ContributionValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.LetterNumber)
                .GreaterThan(0)
                .WithMessage(
                    localizationService.GetResource(
                        "Admin.Contract.Contribution.Fields.LetterNumber.GreaterThanOrEqualTo1"));

            RuleFor(x => x.YearId)
                .GreaterThan(0)
                .WithMessage(
                    localizationService.GetResource("Admin.Contract.Contribution.Fields.YearId.GreaterThanOrEqualTo1"));

            RuleFor(x => x.MonthId)
                .GreaterThan(0)
                .WithMessage(
                    localizationService.GetResource("Admin.Contract.Contribution.Fields.MonthId.GreaterThanOrEqualTo1"));
        }
    }
}