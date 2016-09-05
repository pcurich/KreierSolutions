using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Ks.Admin.Validators.Customers;
using Ks.Web.Framework;
using Ks.Web.Framework.Localization;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Customers
{
    [Validator(typeof(CustomerAttributeValueValidator))]
    public partial class CustomerAttributeValueModel : BaseKsEntityModel, ILocalizedModel<CustomerAttributeValueLocalizedModel>
    {
        public CustomerAttributeValueModel()
        {
            Locales = new List<CustomerAttributeValueLocalizedModel>();
        }

        public int CustomerAttributeId { get; set; }

        [KsResourceDisplayName("Admin.Customers.CustomerAttributes.Values.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [KsResourceDisplayName("Admin.Customers.CustomerAttributes.Values.Fields.IsPreSelected")]
        public bool IsPreSelected { get; set; }

        [KsResourceDisplayName("Admin.Customers.CustomerAttributes.Values.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<CustomerAttributeValueLocalizedModel> Locales { get; set; }

    }

    public partial class CustomerAttributeValueLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [KsResourceDisplayName("Admin.Customers.CustomerAttributes.Values.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }
    }
}