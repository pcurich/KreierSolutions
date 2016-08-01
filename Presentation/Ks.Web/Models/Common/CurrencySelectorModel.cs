using System.Collections.Generic;
using Ks.Web.Framework.Mvc;

namespace Ks.Web.Models.Common
{
    public class CurrencySelectorModel : BaseKsModel
    {
        public CurrencySelectorModel()
        {
            AvailableCurrencies = new List<CurrencyModel>();
        }

        public IList<CurrencyModel> AvailableCurrencies { get; set; }

        public int CurrentCurrencyId { get; set; }
    }
}