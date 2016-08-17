using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Ks.Admin.Validators.Directory;
using Ks.Web.Framework;
using Ks.Web.Framework.Localization;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Directory
{
    [Validator(typeof(StateProvinceValidator))]
    public partial class StateProvinceModel : BaseKsEntityModel, ILocalizedModel<StateProvinceLocalizedModel>
    {
        public StateProvinceModel()
        {
            Locales = new List<StateProvinceLocalizedModel>();
        }
        public int CountryId { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Countries.States.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Countries.States.Fields.Abbreviation")]
        [AllowHtml]
        public string Abbreviation { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Countries.States.Fields.Published")]
        public bool Published { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Countries.States.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<StateProvinceLocalizedModel> Locales { get; set; }
    }

    public partial class StateProvinceLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }
        
        [KsResourceDisplayName("Admin.Configuration.Countries.States.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }
    }
}