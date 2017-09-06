using Ks.Core.Configuration;

namespace Ks.Core.Domain.Directory
{
    public class SignatureSettings : ISettings
    {
        public string DefaultName { get; set; }

        public bool ShowBenefitLeft { get; set; }
        public string BenefitLeftName { get; set; }
        public string BenefitLeftPosition { get; set; }

        public bool ShowBenefitCenter { get; set; }
        public string BenefitCenterName { get; set; }
        public string BenefitCenterPosition { get; set; }

        public bool ShowBenefitRigh { get; set; }
        public string BenefitRightName { get; set; }
        public string BenefitRightPosition { get; set; }

        public bool ShowLoanRight { get; set; }
        public string LoanRightName { get; set; }
        public string LoanRightPosition { get; set; }

        public bool ShowLoanLeft { get; set; }
        public string LoanLeftName { get; set; }
        public string LoanLeftPosition { get; set; }
    }
}