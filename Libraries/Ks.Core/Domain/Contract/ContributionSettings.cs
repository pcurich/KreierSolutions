using Ks.Core.Configuration;

namespace Ks.Core.Domain.Contract
{
    public class ContributionSettings:ISettings
    {
        /// <summary>
        /// Gets or sets the total cycle.
        /// </summary>s
        public int TotalCycle { get; set; }
        /// <summary>
        /// Gets or sets the day of payment.
        /// </summary>
        public int DayOfPaymentContribution { get; set; }
        /// <summary>
        /// Gets or sets the cycle of delay.
        /// </summary>
        public int CycleOfDelay { get; set; }
        public decimal AmountMeta { get; set; }
        /// <summary>
        /// Gets or sets the maximum charge.
        /// </summary>
        public decimal MaximumChargeCaja { get; set; }
        public decimal MaximumChargeCopere { get; set; }

        #region Amount 1
        /// <summary>
        /// Gets or sets the name amount1.
        /// </summary>
        public string NameAmount1 { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is active amount2.
        /// </summary>
        public bool IsActiveAmount1 { set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether this value show in report
        /// </summary>
        public bool Is1OnReport { get; set; }
        /// <summary>
        /// Gets or sets the amount of discount.
        /// </summary>
        public decimal Amount1 { get; set; }
        #endregion

        #region Amount 2

        /// <summary>
        /// Gets or sets the name amount2.
        /// </summary>
        public string NameAmount2 { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is active amount2.
        /// </summary>
        public bool IsActiveAmount2 { set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether this value show in report
        /// </summary>
        public bool Is2OnReport { get; set; }
        /// <summary>
        /// Gets or sets the amount of discount.
        /// </summary>
        public decimal Amount2 { get; set; }

        #endregion

        #region Amount 3

        /// <summary>
        /// Gets or sets the name amount3.
        /// </summary>
        public string NameAmount3 { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is active amount3.
        /// </summary>
        public bool IsActiveAmount3 { set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether this value show in report
        /// </summary>
        public bool Is3OnReport { get; set; }
        /// <summary>
        /// Gets or sets the amount of discount.
        /// </summary>
        public decimal Amount3 { get; set; }

        #endregion

        #region Amount 4

        /// <summary>
        /// Gets or sets the name amount4.
        /// </summary>
        public string NameAmount4 { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is active amount4.
        /// </summary>
        public bool IsActiveAmount4 { set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether this value show in report
        /// </summary>
        public bool Is4OnReport { get; set; }
        /// <summary>
        /// Gets or sets the amount of discount.
        /// </summary>
        public decimal Amount4 { get; set; }

        #endregion

        #region Amount 5

        /// <summary>
        /// Gets or sets the name amount5.
        /// </summary>
        public string NameAmount5 { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is active amount5.
        /// </summary>
        public bool IsActiveAmount5 { set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether this value show in report
        /// </summary>
        public bool Is5OnReport { get; set; }
        /// <summary>
        /// Gets or sets the amount of discount.
        /// </summary>
        public decimal Amount5 { get; set; }

        #endregion

        #region Amount 6

        /// <summary>
        /// Gets or sets the name amount6.
        /// </summary>
        public string NameAmount6 { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is active amount6.
        /// </summary>
        public bool IsActiveAmount6 { set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether this value show in report
        /// </summary>
        public bool Is6OnReport { get; set; }
        /// <summary>
        /// Gets or sets the amount of discount.
        /// </summary>
        public decimal Amount6 { get; set; }

        #endregion

        public int Amount1Source { get; set; }
        public int Amount2Source { get; set; }
        public int Amount3Source { get; set; }

        public int Amount4Source { get; set; }
        public int Amount5Source { get; set; }
        public int Amount6Source { get; set; }
    }
}