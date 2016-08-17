using System.Web.Mvc;
using FluentValidation.Attributes;
using Ks.Admin.Validators.Directory;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Directory
{
    [Validator(typeof(MeasureWeightValidator))]
    public partial class MeasureWeightModel : BaseKsEntityModel
    {
        [KsResourceDisplayName("Admin.Configuration.Measures.Weights.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Measures.Weights.Fields.SystemKeyword")]
        [AllowHtml]
        public string SystemKeyword { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Measures.Weights.Fields.Ratio")]
        public decimal Ratio { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Measures.Weights.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Measures.Weights.Fields.IsPrimaryWeight")]
        public bool IsPrimaryWeight { get; set; }
    }
}