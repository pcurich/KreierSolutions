using System;
using System.Linq;
using Ks.Core;
using Ks.Core.Domain.System;
using Ks.Services.KsSystems;

namespace Ks.Web.Framework
{
    /// <summary>
    ///     KsSystem context for web application
    /// </summary>
    public partial class WebKsSystemContext : IKsSystemContext
    {
        private KsSystem _cachedKsSystem;
        private readonly IKsSystemService _ksSystemService;
        private readonly IWebHelper _webHelper;

        public WebKsSystemContext(IKsSystemService ksSystemService, IWebHelper webHelper)
        {
            _ksSystemService = ksSystemService;
            _webHelper = webHelper;
        }

        /// <summary>
        ///     Gets or sets the current store
        /// </summary>
        public virtual KsSystem CurrentSystem
        {
            get
            {
                if (_cachedKsSystem != null)
                    return _cachedKsSystem;

                //ty to determine the current store by HTTP_HOST
                var host = _webHelper.ServerVariables("HTTP_HOST");
                var allKsSytem = _ksSystemService.GetAllKsSystems();
                var ksSystem = allKsSytem.FirstOrDefault(s => s.ContainsHostValue(host));

                if (ksSystem == null)
                {
                    //load the first found store
                    ksSystem = allKsSytem.FirstOrDefault();
                }
                if (ksSystem == null)
                    throw new Exception("No ksSystem could be loaded");

                _cachedKsSystem = ksSystem;
                return _cachedKsSystem;
            }
        }
    }
}