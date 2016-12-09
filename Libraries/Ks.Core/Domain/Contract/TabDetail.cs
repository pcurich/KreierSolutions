using System;

namespace Ks.Core.Domain.Contract
{
    public class TabDetail : BaseEntity
    {
        public int TabId { get; set; }
        /// <summary>
        /// Gets or sets the year in activity.
        /// </summary>
        public int YearInActivity { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public double Value { get; set; }
        /// <summary>
        ///     Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
        /// <summary>
        /// Gets or sets the updated on UTC.
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }
        public virtual Tab Tab { get; set; }
    }
}