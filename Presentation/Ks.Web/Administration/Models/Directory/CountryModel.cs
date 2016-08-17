using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Ks.Admin.Models.Customers;
using Ks.Admin.Validators.Directory;
using Ks.Web.Framework;
using Ks.Web.Framework.Localization;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Directory
{
    [Validator(typeof(CountryValidator))]
    public partial class CountryModel : BaseKsEntityModel, ILocalizedModel<CountryLocalizedModel>
    {
        public CountryModel()
        {
            Locales = new List<CountryLocalizedModel>();
        }
        [KsResourceDisplayName("Admin.Configuration.Countries.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Countries.Fields.AllowsBilling")]
        public bool AllowsBilling { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Countries.Fields.AllowsShipping")]
        public bool AllowsShipping { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Countries.Fields.TwoLetterIsoCode")]
        [AllowHtml]
        public string TwoLetterIsoCode { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Countries.Fields.ThreeLetterIsoCode")]
        [AllowHtml]
        public string ThreeLetterIsoCode { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Countries.Fields.NumericIsoCode")]
        public int NumericIsoCode { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Countries.Fields.SubjectToVat")]
        public bool SubjectToVat { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Countries.Fields.Published")]
        public bool Published { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Countries.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }




        [KsResourceDisplayName("Admin.Configuration.Countries.Fields.NumberOfStates")]
        public int NumberOfStates { get; set; }

        public IList<CountryLocalizedModel> Locales { get; set; }


        //Store mapping
        [KsResourceDisplayName("Admin.Configuration.Countries.Fields.LimitedToStores")]
        public bool LimitedToStores { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Countries.Fields.AvailableStores")]
        public List<CustomerModel.StoreModel> AvailableStores { get; set; }
        public int[] SelectedStoreIds { get; set; }
    }

    public partial class CountryLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Countries.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }
    }
}