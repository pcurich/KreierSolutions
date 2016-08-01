using Autofac;
using Autofac.Core;
using Ks.Admin.Controllers;
using Ks.Core.Caching;
using Ks.Core.Configuration;
using Ks.Core.Infrastructure;
using Ks.Core.Infrastructure.DependencyManagement;

namespace Ks.Admin.Infrastructure
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder,KsConfig config)
        {
            //we cache presentation models between requests
            builder.RegisterType<HomeController>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("ks_cache_static"));
            //builder.RegisterType<ProductController>()
            //    .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("ks_cache_static"));
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 2; }
        }
    }
}