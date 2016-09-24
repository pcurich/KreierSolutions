using System;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Customers
{
    public partial class ContributionPaymentsModel : BaseKsEntityModel
    {
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateOfPaymentOn { get; set; }
    }
}