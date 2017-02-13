using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Settings
{
    public class SignatureSettingsModel : BaseKsModel
    {
        [KsResourceDisplayName("Admin.Configuration.Settings.Signature.DefaultName")]
        public string DefaultName { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Settings.Signature.BenefitLeftName")]
        public string BenefitLeftName { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.Signature.BenefitLeftPosition")]
        public string BenefitLeftPosition { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Settings.Signature.BenefitCenterName")]
        public string BenefitCenterName { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.Signature.BenefitCenterPosition")]
        public string BenefitCenterPosition { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.Signature.BenefitRightName")]
        public string BenefitRightName { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.Signature.BenefitRightPosition")]
        public string BenefitRightPosition { get; set; }

        [KsResourceDisplayName("Admin.Configuration.Settings.Signature.LoanRightName")]
        public string LoanRightName { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.Signature.LoanRightPosition")]
        public string LoanRightPosition { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.Signature.LoanLeftName")]
        public string LoanLeftName { get; set; }
        [KsResourceDisplayName("Admin.Configuration.Settings.Signature.LoanLeftPosition")]
        public string LoanLeftPosition { get; set; }
    }
}