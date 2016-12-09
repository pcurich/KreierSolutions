using System;
using System.Collections.Generic;
using Ks.Core.Domain.Customers;

namespace Ks.Core.Domain.Contract
{
    public class Contribution : BaseEntity
    {
        private ICollection<ContributionPayment> _contributionPayments;

        /// <summary>
        /// Gets or sets the Authorized Discount
        /// </summary>
        public int AuthorizeDiscount { get; set; }

        /// <summary>
        /// Gets or sets the amount meta
        /// </summary>
        public decimal AmountMeta { get; set; }

        /// <summary>
        /// Gets or sets the amount total.
        /// </summary>
        public decimal AmountPayed { get; set; }

        /// <summary>
        /// Gets or sets the total Of Cycles
        /// </summary>
        public int TotalOfCycles { get; set; }

        /// <summary>
        /// Gets or sets the cycle of payed.
        /// </summary>
        public int PayedCycles { get; set; }

        /// <summary>
        /// Gets or sets the cycle of partial.
        /// </summary>
        public int PartialCycles { get; set; }
        /// <summary>
        /// Gets or sets the cycle of delay.
        /// </summary>
        public int DelayCycles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is delay.
        /// </summary>
        public bool IsDelay { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Contribution"/> is active.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        ///     Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the updated on UTC.
        /// </summary>
        public DateTime? UpdatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public virtual ICollection<ContributionPayment> ContributionPayments
        {
            get { return _contributionPayments ?? (_contributionPayments = new List<ContributionPayment>()); }
            set { _contributionPayments = value; }
        }
    }
}