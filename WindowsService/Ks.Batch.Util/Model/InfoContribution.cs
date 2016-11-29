namespace Ks.Batch.Util.Model
{
    public class InfoContribution
    {
        public int ContributionPaymentId { get; set; }
        public int Number { get; set; }
        public int NumberOld { get; set; }
        public int ContributionId { get; set; }
        public decimal Amount1 { get; set; }
        public decimal Amount2 { get; set; }
        public decimal Amount3 { get; set; }
        public decimal AmountOld { get; set; }
        public decimal AmountTotal { get; set; }
        public decimal AmountPayed { get; set; }
        public int StateId { get; set; }
        public bool IsAutomatic { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string TransactionNumber { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }
    }
}