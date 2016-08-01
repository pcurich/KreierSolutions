using System.Web.Routing;
using Ks.Web.Framework.Mvc.Routes;
using Ks.Web.Framework.Seo;

namespace Ks.Web.Infrastructure
{
    public partial class GenericUrlRouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //generic URLs
            routes.MapGenericPathRoute("GenericUrl",
                "{generic_se_name}",
                new {controller = "Common", action = "GenericUrl"},
                new[] {"Ks.Web.Controllers"});
        }

        public int Priority
        {
            get
            {
                //it should be the last route
                //we do not set it to -int.MaxValue so it could be overridden (if required)
                return -1000000;

            }
        }
    }
}