﻿using System;
using System.Collections.Generic;

namespace Ks.Batch.Util.Model
{
    [Serializable]
    public class Info
    {
        public Info()
        {
            InfoContribution = new InfoContribution();
            InfoLoans = new List<InfoLoan>();
        }
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
        public bool IsUnique { get; set; }
    }
}