using Ks.Core.Configuration;

namespace Ks.Core.Domain.Batchs
{
    public class ScheduleBatchsSetting : ISettings
    {
        public string ServiceName1 { get; set; }
        public int DayOfProcess1 { get; set; }
        public string ServiceName2 { get; set; }
        public int DayOfProcess2 { get; set; }

        public string ServicePreContributionNameNoCash { get; set; }
        public string ServicePreContributionSourceNoCash { get; set; }
        
        public string ServicePreContributionNamePayedComplete { get; set; }
        public string ServicePreContributionSourcePayedComplete { get; set; }

        public string ServicePreContributionNameIncomplete { get; set; }
        public string ServicePreContributionSourceIncomplete { get; set; }

        public string ServicePreLoanNameNoCash { get; set; }
        public string ServicePreLoanSourceNoCash { get; set; }

        public string ServicePreLoanNamePayedComplete { get; set; }
        public string ServicePreLoanSourcePayedComplete { get; set; }

        public string ServicePreLoanNameIncomplete { get; set; }
        public string ServicePreLoanSourceIncomplete { get; set; }


    }
}