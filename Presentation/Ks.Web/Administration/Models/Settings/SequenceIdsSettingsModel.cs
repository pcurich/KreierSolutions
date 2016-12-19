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
        [KsResourceDisplayName("Admin.Configuration.Settings.SequenceIds.AuthorizeLoan")]
        public int AuthorizeLoan { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.SequenceIds.RegistrationCash")]
        public int RegistrationCash { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.SequenceIds.RegistrationForm")]
        public int RegistrationForm { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Settings.SequenceIds.NumberOfLiquidation")]
        public int NumberOfLiquidation { get; set; }
    }
}