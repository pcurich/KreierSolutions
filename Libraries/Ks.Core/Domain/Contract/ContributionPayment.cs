using System;

namespace Ks.Core.Domain.Contract
{
    public class ContributionPayment : BaseEntity
    {
        public int ContributionId { get; set; }
        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Gets or sets the scheduled date on UTC.
        /// </summary>
        public DateTime ScheduledDateOnUtc { get; set; }
        /// <summary>
        /// Gets or sets the processed date on UTC.
        /// </summary>
        public DateTime? ProcessedDateOnUtc { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        public bool Active { get; set; }

        public virtual Contribution Contribution { get; set; }

    }
}