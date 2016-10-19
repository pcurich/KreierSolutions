using System;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Customers
{
    public partial class LoanPaymentsModel:BaseKsEntityModel
    {
        public int LoanId { get; set; }
        public int Quota { get; set; }
        public decimal MonthlyQuota { get; set; }
        public decimal MonthlyFee { get; set; }
        public decimal MonthlyCapital { get; set; }
        public DateTime ScheduledDateOnUtc { get; set; }
        public DateTime? ProcessedDateOnUtc { get; set; }
        public string State { get; set; }
    }
}