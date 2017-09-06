using System.IO;
using System.Web.Mvc;
using Ks.Core;
using Ks.Core.Caching;
using Ks.Core.Domain.Customers;
using Ks.Services.Common;
using Ks.Services.Directory;
using Ks.Services.Localization;
using Ks.Services.Logging;
using Ks.Services.Security;
using Ks.Web.Models.Common;

namespace Ks.Web.Controllers
{
    public class CommonController : BasePublicController
    {
        #region Ctor

        public CommonController(IKsSystemContext storeContext, IWebHelper webHelper)
        {
            _storeContext = storeContext;
            _webHelper = webHelper;
        }

        #endregion

        #region Fields

        private readonly IKsSystemContext _storeContext;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Methods

        //page not found
        public ActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;

            return View();
        }

        //favicon
        [ChildActionOnly]
        public ActionResult Favicon()
        {
            //try loading a store specific favicon
            var faviconFileName = string.Format("favicon-{0}.ico", _storeContext.CurrentSystem.Id);
            var localFaviconPath = Path.Combine(Request.PhysicalApplicationPath, faviconFileName);
            if (!System.IO.File.Exists(localFaviconPath))
            {
                //try loading a generic favicon
                faviconFileName = "favicon.ico";
                localFaviconPath = Path.Combine(Request.PhysicalApplicationPath, faviconFileName);
                if (!System.IO.File.Exists(localFaviconPath))
                {
                    return Content("");
                }
            }

            var model = new FaviconModel
            {
                FaviconUrl = _webHelper.GetStoreLocation() + faviconFileName
            };
            return PartialView(model);
        }

        #endregion
    }
}