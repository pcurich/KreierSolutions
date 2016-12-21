using System.Web.Mvc;
using Ks.Web.Framework.Security;

namespace Ks.Web.Controllers
{
    [Authorize]
    public class HomeController : BasePublicController
    {
        [KsHttpsRequirement(SslRequirement.No)]
        public ActionResult Index()
        {
            return RedirectToAction("Index","Home",new {Area="Admin"});
            //return View();
        }
    }
}