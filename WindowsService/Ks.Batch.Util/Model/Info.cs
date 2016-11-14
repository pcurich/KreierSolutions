namespace Ks.Batch.Util.Model
{
    public class Info
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int CustomerId { get; set; }
        public string CompleteName { get; set; }
        public bool HasAdminCode { get; set; }
        public string AdminCode { get; set; }
        public bool HasDni { get; set; }
        public string Dni { get; set; }
        public decimal Amount1 { get; set; }
        public decimal Amount2 { get; set; }
        public decimal Amount3 { get; set; }
        public decimal AmountTotal { get; set; }
        public decimal AmountPayed { get; set; }
        public int StateId { get; set; }
        public string BankName { get; set; }
        public string Description { get; set; }
        public int Number { get; set; }
    }
}