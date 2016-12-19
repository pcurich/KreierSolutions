using Ks.Core.Configuration;

namespace Ks.Core.Domain.Directory
{
    public class SignatureSettings : ISettings
    {
        public string DefaultName { get; set; }

        public string BenefitLeftName { get; set; }
        public string BenefitLeftPosition { get; set; }

        public string BenefitCenterName { get; set; }
        public string BenefitCenterPosition { get; set; }

        public string BenefitRightName { get; set; }
        public string BenefitRightPosition { get; set; }

    }
}