﻿using Ks.Core.Configuration;

namespace Ks.Core.Domain.Contract
{
    public class PaymentSettings:ISettings
    {
        /// <summary>
        /// Gets or sets the total cycle.
        /// </summary>
        public int TotalCycle { get; set; }
        /// <summary>
        /// Gets or sets the day of payment.
        /// </summary>
        public int DayOfPayment { get; set; }

        /// <summary>
        /// Gets or sets the name amount1.
        /// </summary>
        public string NameAmount1 { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active amount2.
        /// </summary>
        public bool IsActiveAmount1 { set; get; }

        /// <summary>
        /// Gets or sets the amount of discount.
        /// </summary>
        public decimal Amount1 { get; set; }

        /// <summary>
        /// Gets or sets the name amount2.
        /// </summary>
        public string NameAmount2 { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active amount2.
        /// </summary>
        public bool IsActiveAmount2 { set; get; }

        /// <summary>
        /// Gets or sets the amount of discount.
        /// </summary>
        public decimal Amount2 { get; set; }

        /// <summary>
        /// Gets or sets the name amount3.
        /// </summary>
        public string NameAmount3 { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active amount3.
        /// </summary>
        public bool IsActiveAmount3 { set; get; }

        /// <summary>
        /// Gets or sets the amount of discount.
        /// </summary>
        public decimal Amount3 { get; set; }
        
    }
}