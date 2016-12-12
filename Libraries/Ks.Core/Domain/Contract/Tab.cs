using System;
using System.Collections.Generic;
using System.IO;

namespace Ks.Core.Domain.Contract
{
    public class Tab : BaseEntity
    {
        private ICollection<TabDetail> _tabDetalis;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the amount base.
        /// </summary>
        public decimal AmountBase { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        ///     Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the updated on UTC.
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }

        public virtual ICollection<TabDetail> TabDetails
        {
            get { return _tabDetalis ?? (_tabDetalis = new List<TabDetail>()); }
            set { _tabDetalis = value; }
        }
    }
}