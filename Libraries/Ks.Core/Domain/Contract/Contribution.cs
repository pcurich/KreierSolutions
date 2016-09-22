using System;
using System.Collections;
using System.Collections.Generic;
using Ks.Core.Domain.Customers;

namespace Ks.Core.Domain.Contract
{
    public class Contribution : BaseEntity
    {
        private ICollection<ContributionPayment> _contributionPayments;

        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the letter number.
        /// </summary>
        public int LetterNumber { get; set; }

        /// <summary>
        ///     Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the updated on UTC.
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the amount total.
        /// </summary>
        public decimal AmountTotal { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Contribution"/> is active.
        /// </summary>
        public bool Active { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual ICollection<ContributionPayment> ContributionPayments
        {
            get { return _contributionPayments ?? (_contributionPayments = new List<ContributionPayment>()); }
            set { _contributionPayments = value; }
        }
    }
}