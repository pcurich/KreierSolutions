using System.Web.Mvc;
using FluentValidation.Attributes;
using Ks.Admin.Validators.Tasks;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Tasks
{
    [Validator(typeof(ScheduleTaskValidator))]
    public partial class ScheduleTaskModel : BaseKsEntityModel
    {
        [KsResourceDisplayName("Admin.System.ScheduleTasks.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [KsResourceDisplayName("Admin.System.ScheduleTasks.Seconds")]
        public int Seconds { get; set; }

        [KsResourceDisplayName("Admin.System.ScheduleTasks.Enabled")]
        public bool Enabled { get; set; }

        [KsResourceDisplayName("Admin.System.ScheduleTasks.StopOnError")]
        public bool StopOnError { get; set; }

        [KsResourceDisplayName("Admin.System.ScheduleTasks.LastStart")]
        public string LastStartUtc { get; set; }

        [KsResourceDisplayName("Admin.System.ScheduleTasks.LastEnd")]
        public string LastEndUtc { get; set; }

        [KsResourceDisplayName("Admin.System.ScheduleTasks.LastSuccess")]
        public string LastSuccessUtc { get; set; }
    }
}
