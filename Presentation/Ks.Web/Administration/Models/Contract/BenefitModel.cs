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
            States = new List<SelectListItem>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public int BenefitTypeId { get; set; }
        public int BenefitTypeName { get; set; }
        public double Discount { get; set; }
        public bool CancelLoans { get; set; }
        public bool LetterDeclaratory { get; set; }
        public bool Published { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        public List<SelectListItem> States { get; set; }
    }
}