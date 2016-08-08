using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Common
{
    public class SystemWarningModel : BaseKsModel
    {
        public SystemWarningLevel Level { get; set; }

        public string Text { get; set; }
    }

    public enum SystemWarningLevel
    {
        Pass,
        Warning,
        Fail
    }
}