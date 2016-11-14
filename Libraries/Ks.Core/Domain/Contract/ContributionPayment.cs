using System;

namespace Ks.Core.Domain.Contract
{
    public class ContributionPayment : BaseEntity
    {
        /// <summary>
        /// Gets or sets the number of qouta.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Gets or sets the number of old qouta.
        /// </summary>
        public int NumberOld { get; set; }
        /// <summary>
        /// Gets or sets the contribution identifier.
        /// </summary>
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
        /// Gets or sets the old amount with out cash.
        /// </summary>
        public decimal AmountOld { get; set; }

        /// <summary>
        /// Gets or sets the amount total, amount1+amount2+amount3.
        /// </summary>
        public decimal AmountTotal { get; set; }
        /// <summary>
        /// Gets or sets the amount payed.
        /// </summary>
        public decimal AmountPayed { get; set; }
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
        /// <summary>
        /// Gets or sets a value indicating whether this instance is automatic or manual.
        /// </summary>
        public bool IsAutomatic { get; set; }
        /// <summary>
        /// Gets or sets the name of the bank.
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// Gets or sets the account number of bank.
        /// </summary>
        public string AccountNumber { get; set; }
        /// <summary>
        /// Gets or sets the transaction number.
        /// </summary>
        public string TransactionNumber { get; set; }
        /// <summary>
        /// Gets or sets the reference.
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        public virtual Contribution Contribution { get; set; }

    }
}