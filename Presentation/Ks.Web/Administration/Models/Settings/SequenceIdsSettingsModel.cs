using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Settings
{
    public class SequenceIdsSettingsModel : BaseKsModel
    {
        [KsResourceDisplayName("Admin.Configuration.Settings.SequenceIds.DeclaratoryLetter")]
        public int DeclaratoryLetter { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.SequenceIds.AuthorizeDiscount")]
        public int AuthorizeDiscount { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.SequenceIds.RegistrationForm")]
        public int RegistrationForm { get; set; }
    }
}