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
        /// Gets or sets the monthly payed.
        /// </summary>
        public decimal MonthlyPayed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        public int StateId { get; set; }
        /// <summary>
        /// Gets or sets the contribution satates.
        /// </summary>
        public LoanState LoanState
        {
            get { return (LoanState)StateId; }
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
        /// <summary>
        /// Gets or sets the scheduled date on UTC.
        /// </summary>
        public DateTime ScheduledDateOnUtc { get; set; }
        /// <summary>
        /// Gets or sets the processed date on UTC.
        /// </summary>
        public DateTime? ProcessedDateOnUtc { get; set; }

        public virtual Loan Loan { get; set; }

    }
}