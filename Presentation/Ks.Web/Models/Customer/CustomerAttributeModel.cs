using System.Collections.Generic;
using Ks.Core.Domain.Catalog;
using Ks.Web.Framework.Mvc;

namespace Ks.Web.Models.Customer
{
    public partial class CustomerAttributeModel : BaseKsEntityModel
    {
        public CustomerAttributeModel()
        {
            Values = new List<CustomerAttributeValueModel>();
        }

        public string Name { get; set; }

        public bool IsRequired { get; set; }

        /// <summary>
        /// Default value for textboxes
        /// </summary>
        public string DefaultValue { get; set; }

        public AttributeControlType AttributeControlType { get; set; }

        public IList<CustomerAttributeValueModel> Values { get; set; }

    }

    public partial class CustomerAttributeValueModel : BaseKsEntityModel
    {
        public string Name { get; set; }

        public bool IsPreSelected { get; set; }
    }
}