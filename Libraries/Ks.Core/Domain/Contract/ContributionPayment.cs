using System;

namespace Ks.Core.Domain.Contract
{
    public class ContributionPayment : BaseEntity
    {
        public int ContributionId { get; set; }
        /// <summary>
        /// Gets or sets the created record on UTC.
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the updated record on UTC.
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        public decimal Amount { get; set; }

        public virtual Contribution Contribution { get; set; }

    }
}