using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Contract
{
    public class BenefitModel : BaseKsEntityModel
    {
        public BenefitModel()
        {
            BenefitTypes = new List<SelectListItem>();
        }

        [KsResourceDisplayName("Admin.Configuration.Benefits.Fields.Name")]
        public string Name { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Benefits.Fields.Description")]
        [AllowHtml]
        public string Description { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Benefits.Fields.BenefitTypeId")]
        public int BenefitTypeId { get; set; }
        public string BenefitTypeName { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Benefits.Fields.Discount")]
        public double Discount { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Benefits.Fields.CancelLoans")]
        public bool CancelLoans { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Benefits.Fields.LetterDeclaratory")]
        public bool LetterDeclaratory { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Benefits.Fields.IsActive")]
        public bool IsActive { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Benefits.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Benefits.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Benefits.Fields.UpdatedOn")]
        public DateTime UpdatedOn { get; set; }

        public List<SelectListItem> BenefitTypes { get; set; }
    }
}