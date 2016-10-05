using System;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Customers
{
    public partial class ContributionPaymentsModel : BaseKsEntityModel
    {
        public int CustomerId { get; set; }
        public decimal Amount1 { get; set; }
        public decimal Amount2 { get; set; }
        public decimal Amount3 { get; set; }
        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.ScheduledDateOnUtc")]
        public DateTime ScheduledDateOnUtc { get; set; }
        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.ProcessedDateOnUtc")]
        public DateTime? ProcessedDateOnUtc { get; set; }
        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.Active")]
        public string State { get; set; }
    }
}