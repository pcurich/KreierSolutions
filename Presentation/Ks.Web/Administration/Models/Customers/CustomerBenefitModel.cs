using System.Collections.Generic;
using System.Web.Mvc;
using Ks.Admin.Models.Contract;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Customers
{
    public class CustomerBenefitModel : BaseKsEntityModel
    {
        public CustomerBenefitModel()
        {
            Banks = new List<SelectListItem>();
            BenefitModels =new List<SelectListItem>();
        }

        #region Customer?

        public int CustomerId { get; set; }
        public string CustomerCompleteName { get; set; }
        public string CustomerAdmCode { get; set; }
        public string CustomerDni { get; set; }
        #endregion

        #region Payment

        public string BankName { get; set; }
        public List<SelectListItem> Banks { get; set; }
        public string CheckNumber { get; set; }

        public decimal AmountToPay { get; set; }

        #endregion

        public decimal AmountBaseOfBenefit { get; set; }

        public int BenefitId { get; set; }
        public List<SelectListItem> BenefitModels { get; set; }

        public List<TabModel> TabModels { get; set; }
    }
}