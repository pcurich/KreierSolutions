using Ks.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ks.Admin.Models.Contract
{
    public class ContributionBenefitBankModel : BaseKsEntityModel
    {
        public ContributionBenefitBankModel()
        {
            RelaTionShips = new List<SelectListItem>();
            Banks = new List<SelectListItem>();
        }

        public int ContributionBenefitId { get; set; }
        public string CompleteName { get; set; }
        public string Dni { get; set; }

        public string RelationShip { get; set; }
        public List<SelectListItem> RelaTionShips { get; set; }

        
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public List<SelectListItem> Banks { get; set; }
        
        public string CheckNumber { get; set; }
        public double Ratio { get; set; }
        public decimal AmountToPay { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}