using System.Net;
using Ks.Core;
using Ks.Services.Tasks;

namespace Ks.Services.Common
{
    /// <summary>
    /// Represents a task for keeping the site alive
    /// </summary>
    public partial class KeepAliveTask : ITask
    {
        private readonly IKsSystemContext _ksSystemContext;

        public KeepAliveTask(IKsSystemContext ksSystemContext)
        {
            _ksSystemContext = ksSystemContext;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            string url = _ksSystemContext.CurrentSystem.Url + "keepalive/index";
            using (var wc = new WebClient())
            {
                wc.DownloadString(url);
            }
        }
    }
}
