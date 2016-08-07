using Ks.Web.Framework.Mvc;

namespace Ks.Web.Models.Common
{
    public partial class HeaderLinksModel : BaseKsModel
    {
        public bool IsAuthenticated { get; set; }
        public string CustomerEmailUsername { get; set; }
        public string AlertMessage { get; set; }
    }
}