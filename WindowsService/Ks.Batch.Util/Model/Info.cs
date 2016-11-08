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
        public decimal Total { get; set; }
    }
}