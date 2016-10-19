using System;

namespace Ks.Core.Domain.Contract
{
    public class LoanPayment : BaseEntity
    {
        public int LoanId { get; set; }
        /// <summary>
        /// Gets or sets the quota.
        /// </summary>
        public int Quota { get; set; }
        /// <summary>
        /// Gets or sets the MonthlyQuota.
        /// </summary>
        public decimal MonthlyQuota { get; set; }
        /// <summary>
        /// Gets or sets the MonthlyFee.
        /// </summary>
        public decimal MonthlyFee { get; set; }
        /// <summary>
        /// Gets or sets the MonthlyCapital.
        /// </summary>
        public decimal MonthlyCapital { get; set; }
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

        public virtual Loan Loan { get; set; }

    }
}