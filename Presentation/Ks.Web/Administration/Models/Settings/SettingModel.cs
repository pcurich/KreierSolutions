using System.Web.Mvc;
using FluentValidation.Attributes;
using Ks.Admin.Validators.Settings;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Settings
{
    [Validator(typeof(SettingValidator))]
    public partial class SettingModel : BaseKsEntityModel
    {
        [KsResourceDisplayName("Admin.Configuration.Settings.AllSettings.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Settings.AllSettings.Fields.Value")]
        [AllowHtml]
        public string Value { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Settings.AllSettings.Fields.KsSystemName")]
        public string KsSystem { get; set; }
        public int KsSystemId { get; set; }
    }
}