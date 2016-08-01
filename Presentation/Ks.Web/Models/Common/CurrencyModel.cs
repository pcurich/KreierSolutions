using Ks.Web.Framework.Mvc;

namespace Ks.Web.Models.Common
{
    public partial class CurrencyModel : BaseKsEntityModel
    {
        public string Name { get; set; }

        public string CurrencySymbol { get; set; }
    }
}