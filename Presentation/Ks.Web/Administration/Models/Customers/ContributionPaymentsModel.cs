using System;
using Ks.Web.Framework;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Customers
{
    public partial class ContributionPaymentsModel : BaseKsEntityModel
    {
        public int CustomerId { get; set; }
        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.Amount")]
        public decimal Amount { get; set; }
        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.ScheduledDateOnUtc")]
        public DateTime ScheduledDateOnUtc { get; set; }
        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.ProcessedDateOnUtc")]
        public DateTime? ProcessedDateOnUtc { get; set; }
        [KsResourceDisplayName("Admin.Contract.ContributionPayments.Fields.Active")]
        public string State { get; set; }
    }
}