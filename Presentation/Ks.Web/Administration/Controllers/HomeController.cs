using System.Web.Mvc;

namespace Ks.Admin.Controllers
{

    public partial class HomeController : BaseAdminController
    {
        #region Methods

        public ActionResult Index()
        {

            return View();
        }
        #endregion
    }
}