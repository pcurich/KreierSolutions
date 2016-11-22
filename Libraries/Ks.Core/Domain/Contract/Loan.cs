using System;
using System.Collections.Generic;
using Ks.Core.Domain.Customers;

namespace Ks.Core.Domain.Contract
{
    public class Loan : BaseEntity
    {
        private ICollection<LoanPayment> _loanPayments;

        /// <summary>
        ///     Gets or sets the customer identifier.
        /// </summary>
        public int CustomerId { get; set; }
        /// <summary>
        ///     Gets or sets the customer identifier.
        /// </summary>
        public int WarrantyId { get; set; }
        /// <summary>
        ///     Gets or sets the letter number.
        /// </summary>
        public int LoanNumber { get; set; }
        /// <summary>
        ///     Gets or sets the period.
        /// </summary>
        public int Period { get; set; }
        /// <summary>
        /// Gets or sets the total of cycle.
        /// </summary>
        public int TotalOfCycle { get; set; }
        /// <summary>
        ///     Gets or sets the tea.
        /// </summary>
        public double Tea { get; set; }
        /// <summary>
        ///     Gets or sets the safe.
        /// </summary>
        public double Safe { get; set; }
        /// <summary>
        /// Gets or sets the amount loan.
        /// </summary>
        public decimal LoanAmount { get; set; }
        /// <summary>
        ///     Gets or sets the monthly fee.
        /// </summary>
        public decimal MonthlyQuota { get; set; } // (Amount +TotalFeed)/Period     
        /// <summary>
        ///     Gets or sets the total feed.
        /// </summary>
        public decimal TotalFeed { get; set; } // Amount*Period*Tea/12        

        /// <summary>
        ///     Gets or sets the total safe.
        /// </summary>
        public decimal TotalSafe { get; set; } // Amount*Safe        
        
        /// <summary>
        ///     Gets or sets the total amount.
        /// </summary>
        public decimal TotalAmount { get; set; } // Amount +  TotalFeed        

        /// <summary>
        ///     Gets or sets the total to pay.
        /// </summary>
        public decimal TotalToPay { get; set; } // Amount-TotalSafe

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is RequiereAuthorized.
        /// </summary>
        public bool IsAuthorized { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is delay.
        /// </summary>
        public bool IsDelay { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Loan"/> is active.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        ///     Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        ///     Gets or sets the updated on UTC.
        /// </summary>
        public DateTime? UpdatedOnUtc { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual ICollection<LoanPayment> LoanPayments
        {
            get { return _loanPayments ?? (_loanPayments = new List<LoanPayment>()); }
            set { _loanPayments = value; }
        }
    }
}