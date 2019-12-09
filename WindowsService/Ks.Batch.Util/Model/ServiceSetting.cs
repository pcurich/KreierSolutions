using Ks.Batch.Util.Model.Summary;

namespace Ks.Batch.Util.Model
{
    public  class ServiceSetting
    {
        public string Path { get; set; }
        public string Connection { get; set; }
        public string SysName { get; set; }
        public string DefaultCulture { get; set; }
        public bool IsUnique { get; set; } 
        public string ContributionCode { get; set; }
        public string LoanCode { get; set; }

        public string FileYear { get; set; }
        public string FileMonth { get; set; }
        public string FileCode { get; set; }

        public bool IsPre { get; set; }
        public string ContributionPayedComplete { get; set; }
        public string ContributionIncomplete { get; set; }
        public string ContributionNoCash { get; set; }
        public string ContributionNextQuota { get; set; }
        public string LoanPayedComplete { get; set; }
        public string LoanIncomplete { get; set; }
        public string LoanNoCash { get; set; }
        public string LoanNextQuota { get; set; }

        public SummaryMerge SummaryMerge { get; set; }

        public ServiceSetting()
        {
            SummaryMerge = new SummaryMerge();
        } 
    }
}
