using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Ks.Admin.Validators.Customers;
using Ks.Web.Framework;
using Ks.Web.Framework.Localization;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Customers
{
    [Validator(typeof(CustomerAttributeValidator))]
    public partial class CustomerAttributeModel : BaseKsEntityModel, ILocalizedModel<CustomerAttributeLocalizedModel>
    {
        public CustomerAttributeModel()
        {
            Locales = new List<CustomerAttributeLocalizedModel>();
        }

        [KsResourceDisplayName("Admin.Customers.CustomerAttributes.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [KsResourceDisplayName("Admin.Customers.CustomerAttributes.Fields.IsRequired")]
        public bool IsRequired { get; set; }

        [KsResourceDisplayName("Admin.Customers.CustomerAttributes.Fields.AttributeControlType")]
        public int AttributeControlTypeId { get; set; }
        [KsResourceDisplayName("Admin.Customers.CustomerAttributes.Fields.AttributeControlType")]
        [AllowHtml]
        public string AttributeControlTypeName { get; set; }

        [KsResourceDisplayName("Admin.Customers.CustomerAttributes.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }


        public IList<CustomerAttributeLocalizedModel> Locales { get; set; }

    }

    public partial class CustomerAttributeLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [KsResourceDisplayName("Admin.Customers.CustomerAttributes.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

    }
}