using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Contract
{
    public class CheckCompareModel : BaseKsEntityModel
    {

        public CheckModel After { get; set; }
        public CheckModel Before { get; set; }
    }
}