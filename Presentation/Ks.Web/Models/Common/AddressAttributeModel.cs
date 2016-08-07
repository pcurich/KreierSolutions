using System.Collections.Generic;
using Ks.Core.Domain.Catalog;
using Ks.Web.Framework.Mvc;

namespace Ks.Web.Models.Common
{
    public partial class AddressAttributeModel : BaseKsEntityModel
    {
        public AddressAttributeModel()
        {
            Values = new List<AddressAttributeValueModel>();
        }

        public string Name { get; set; }

        public bool IsRequired { get; set; }

        /// <summary>
        /// Default value for textboxes
        /// </summary>
        public string DefaultValue { get; set; }

        public AttributeControlType AttributeControlType { get; set; }

        public IList<AddressAttributeValueModel> Values { get; set; }
    }

    public partial class AddressAttributeValueModel : BaseKsEntityModel
    {
        public string Name { get; set; }

        public bool IsPreSelected { get; set; }
    }
}