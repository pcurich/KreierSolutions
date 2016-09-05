using System.Web.Mvc;
using FluentValidation.Attributes;
using Ks.Admin.Validators.Localization;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Localization
{
    [Validator(typeof(LanguageResourceValidator))]
    public partial class LanguageResourceModel : BaseKsEntityModel
    {
        [KsResourceDisplayName("Admin.Configuration.Languages.Resources.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Languages.Resources.Fields.Value")]
        [AllowHtml]
        public string Value { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Languages.Resources.Fields.LanguageName")]
        [AllowHtml]
        public string LanguageName { get; set; }

        public int LanguageId { get; set; }
    }
}