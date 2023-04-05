namespace Ks.Batch.Util.Model.Summary
{
    public class SummaryMerge
    {
        public int FileContributionTotal { get; set; }
        public decimal FileContributionAmount { get; set; }
        public int FileContributionNoCashTotal { get; set; }
        public int FileContributionPayedCompleteTotal { get; set; }
        public int FileContributionIncompleteTotal { get; set; }

        public int FileLoanTotal { get; set; }
        public decimal FileLoanAmount { get; set; }
        public int FileLoanNoCashTotal { get; set; }
        public int FileLoanPayedCompleteTotal { get; set; }
        public int FileLoanIncompleteTotal { get; set; }

        public int DataBaseContributionTotal { get; set; }
        public decimal DataBaseContributionAmount { get; set; }
        public int DataBaseContributionNoCashTotal { get; set; }
        public int DataBaseContributionPayedCompleteTotal { get; set; }
        public int DataBaseContributionIncompleteTotal { get; set; }

        public int DataBaseLoanTotal { get; set; }
        public decimal DataBaseLoanAmount { get; set; }
        public int DataBaseLoanNextQuota { get; set; }
        public int DataBaseLoanNoCashTotal { get; set; }
        public int DataBaseLoanPayedCompleteTotal { get; set; }
        public int DataBaseLoanIncompleteTotal { get; set; }

    }
}
