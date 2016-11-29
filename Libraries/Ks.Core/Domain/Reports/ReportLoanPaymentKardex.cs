namespace Ks.Core.Domain.Reports
{
    public class ReportLoanPaymentKardex
    {
        public int Year { get; set; }
        public string MonthName { get; set; }
        public decimal MonthlyPayed { get; set; }
        public int StateId { get; set; }
        public int IsAutomatic { get; set; }
    }
}