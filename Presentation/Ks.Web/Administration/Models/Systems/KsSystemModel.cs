using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Ks.Admin.Validators.Systems;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Systems
{
    [Validator(typeof(KsSystemValidator))]
    public partial class KsSystemModel : BaseKsEntityModel
    {
        public KsSystemModel()
        {
            AvailableLanguages = new List<SelectListItem>();
        }

        [KsResourceDisplayName("Admin.Configuration.Stores.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Stores.Fields.Url")]
        [AllowHtml]
        public string Url { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Stores.Fields.SslEnabled")]
        public virtual bool SslEnabled { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Stores.Fields.SecureUrl")]
        [AllowHtml]
        public virtual string SecureUrl { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Stores.Fields.Hosts")]
        [AllowHtml]
        public string Hosts { get; set; }

        //default language
        [KsResourceDisplayName("Admin.Configuration.Stores.Fields.DefaultLanguage")]
        [AllowHtml]
        public int DefaultLanguageId { get; set; }
        public IList<SelectListItem> AvailableLanguages { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Stores.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Stores.Fields.CompanyName")]
        [AllowHtml]
        public string CompanyName { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Stores.Fields.CompanyAddress")]
        [AllowHtml]
        public string CompanyAddress { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Stores.Fields.CompanyPhoneNumber")]
        [AllowHtml]
        public string CompanyPhoneNumber { get; set; }
    }

}