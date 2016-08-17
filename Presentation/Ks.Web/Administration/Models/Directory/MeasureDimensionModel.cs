using System.Web.Mvc;
using FluentValidation.Attributes;
using Ks.Admin.Validators.Directory;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Directory
{
    [Validator(typeof(MeasureDimensionValidator))]
    public partial class MeasureDimensionModel : BaseKsEntityModel
    {
        [KsResourceDisplayName("Admin.Configuration.Measures.Dimensions.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Measures.Dimensions.Fields.SystemKeyword")]
        [AllowHtml]
        public string SystemKeyword { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Measures.Dimensions.Fields.Ratio")]
        public decimal Ratio { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Measures.Dimensions.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Measures.Dimensions.Fields.IsPrimaryDimension")]
        public bool IsPrimaryDimension { get; set; }
    }
}