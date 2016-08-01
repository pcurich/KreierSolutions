using System.Web.Mvc;
using Ks.Web.Framework.Security;

namespace Ks.Web.Controllers
{
    public class HomeController : BasePublicController
    {
        [KsHttpsRequirement(SslRequirement.No)]
        public ActionResult Index()
        {
            return View();
        }
    }
}