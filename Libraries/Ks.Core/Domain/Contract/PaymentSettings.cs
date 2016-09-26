using Ks.Core.Configuration;

namespace Ks.Core.Domain.Contract
{
    public class PaymentSettings:ISettings
    {
        /// <summary>
        /// Gets or sets the amount of discount.
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Gets or sets the total cycle.
        /// </summary>
        public int TotalCycle { get; set; }
        /// <summary>
        /// Gets or sets the day of payment.
        /// </summary>
        public int DayOfPayment { get; set; }
    }
}