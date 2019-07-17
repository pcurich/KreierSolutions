using System;
using System.Collections.Generic;

namespace Ks.Batch.Util.Model.Summary
{
    public class SummaryOut
    {
        public int CustomerId { get; set; }
        public string AdmCode { get; set; }
        public string Contribution { get; set; }
        public List<String> Loans { get; set; }
    }
}
