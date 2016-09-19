using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Settings
{
    public class LetterSettingsModel : BaseKsModel
    {
        [KsResourceDisplayName("Admin.Configuration.Settings.LetterSettings.IsAutogenerate")]
        public bool IsAutogenerate { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.LetterSettings.FromNumber")]
        public int FromNumber { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.LetterSettings.LastNumber")]
        public int LastNumber { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.LetterSettings.StepNumber")]
        public int StepNumber { get; set; }
    }
}