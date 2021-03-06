﻿using FluentValidation;
using Ks.Admin.Models.Systems;
using Ks.Core.Domain.System;
using Ks.Services.Localization;
using Ks.Web.Framework.Validators;

namespace Ks.Admin.Validators.Systems
{
    public class KsSystemValidator : BaseKsValidator<KsSystemModel>
    {
        public KsSystemValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Stores.Fields.Name.Required"));
            RuleFor(x => x.Url).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Stores.Fields.Url.Required"));
        }
    }
}