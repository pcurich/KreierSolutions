using System.Collections.Generic;

namespace Ks.Core.Domain.Reports
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
        public decimal TotalContribution { get; set; }
        public decimal TotalPayed { get; set; }
        public InfoContribution InfoContribution { get; set; }
        public decimal TotalLoan { get; set; }
        public List<InfoLoan> InfoLoans { get; set; } 
    }
}