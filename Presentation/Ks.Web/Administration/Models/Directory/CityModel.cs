using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Ks.Admin.Validators.Directory;
using Ks.Web.Framework;
using Ks.Web.Framework.Localization;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Directory
{
    [Validator(typeof(CityValidator))]
    public class CityModel : BaseKsEntityModel, ILocalizedModel<CityLocalizedModel>
    {
        public CityModel()
        {
            Locales = new List<CityLocalizedModel>();
        }

        public int StateProvinceId { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Countries.States.City.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Countries.States.City.Fields.Ubigeo")]
        [AllowHtml]
        public string Ubigeo { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Countries.States.City.Fields.Published")]
        public bool Published { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Countries.States.City.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<CityLocalizedModel> Locales { get; set; }
    }

    public partial class CityLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Countries.States.City.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }
    }
}