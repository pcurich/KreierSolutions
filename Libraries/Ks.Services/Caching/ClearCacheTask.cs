using Ks.Core.Caching;
using Ks.Core.Infrastructure;
using Ks.Services.Tasks;

namespace Ks.Services.Caching
{
    /// <summary>
    /// Clear cache schedueled task implementation
    /// </summary>
    public partial class ClearCacheTask : ITask
    {
        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            var cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("ks_cache_static");
            cacheManager.Clear();
        }
    }
}
