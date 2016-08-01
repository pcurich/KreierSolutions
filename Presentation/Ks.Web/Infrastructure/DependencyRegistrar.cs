using Autofac;
using Autofac.Core;
using Ks.Core.Caching;
using Ks.Core.Configuration;
using Ks.Core.Infrastructure;
using Ks.Core.Infrastructure.DependencyManagement;
using Ks.Web.Controllers;
using Ks.Web.Infrastructure.Installation;

namespace Ks.Web.Infrastructure
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
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, KsConfig config)
        {
            //we cache presentation models between requests
            //builder.RegisterType<BlogController>()
            //    .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("ks_cache_static"));
            //builder.RegisterType<CatalogController>()
            //    .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("ks_cache_static"));
            //builder.RegisterType<CountryController>()
            //    .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("ks_cache_static"));
            builder.RegisterType<CommonController>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("ks_cache_static"));
            
            //builder.RegisterType<NewsController>()
            //    .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("ks_cache_static"));
            //builder.RegisterType<PollController>()
            //    .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("ks_cache_static"));
            //builder.RegisterType<ProductController>()
            //    .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("ks_cache_static"));
            //builder.RegisterType<ReturnRequestController>()
            //    .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("ks_cache_static"));
            //builder.RegisterType<ShoppingCartController>()
            //    .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("ks_cache_static"));
            //builder.RegisterType<TopicController>()
            //    .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("ks_cache_static"));
            //builder.RegisterType<WidgetController>()
            //    .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("ks_cache_static"));

            //installation localization service
            builder.RegisterType<InstallationLocalizationService>().As<IInstallationLocalizationService>().InstancePerLifetimeScope();
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
