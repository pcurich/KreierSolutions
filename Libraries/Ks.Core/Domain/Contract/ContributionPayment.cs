using System;

namespace Ks.Core.Domain.Contract
{
    public class ContributionPayment : BaseEntity
    {
        public int Number { get; set; }
        public int ContributionId { get; set; }
        /// <summary>
        /// Gets or sets the amount1.
        /// </summary>
        public decimal Amount1 { get; set; }
        /// <summary>
        /// Gets or sets the amount2.
        /// </summary>
        public decimal Amount2 { get; set; }
        /// <summary>
        /// Gets or sets the amount3.
        /// </summary>
        public decimal Amount3 { get; set; }
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
        public int StateId { get; set; }

        /// <summary>
        /// Gets or sets the contribution satates.
        /// </summary>
        public ContributionState ContributionState
        {
            get { return (ContributionState)StateId; }
            set { StateId = (int)value; }
        }

        public bool IsAutomatic { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string TransactionNumber { get; set; }
        public string Reference { get; set; }

        public virtual Contribution Contribution { get; set; }

    }
}